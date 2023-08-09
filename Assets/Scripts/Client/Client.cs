﻿using Assets.Scripts.Client.Interfaces;
using Assets.Scripts.Client.Types;
using Assets.Scripts.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;

        private TCP _tcp;
        private bool _processPackets;

        void Awake()
        {
            //Let the gameobject persist over the scenes
            DontDestroyOnLoad(gameObject);
            //Check if the control instance is null
            if (Instance == null)
            {
                //This instance becomes the single instance available
                Instance = this;
            }
            //Otherwise check if the control instance is not this one
            else if (Instance != this)
            {
                //In case there is a different instance destroy this one.
                Destroy(gameObject);
            }
        }

        async void Update()
        {
            if (_processPackets && _tcp != null)
            {
                PacketMap packetMap = await _tcp.GetPacketMapAsync(destroyCancellationToken);
                if (!packetMap.IsEmpty)
                {
                    foreach (var packetEntry in packetMap.Map)
                    {
                        PacketEventHandler.RaiseEvent(packetEntry.Value);
                    }
                }
            }
        }
        void OnEnable()
        {
            _tcp = new(ConfigurationManager.Config.Host, ConfigurationManager.Config.Port);
            _processPackets = false;
            _tcp.ConnectAsync(destroyCancellationToken);
        }

        void OnDisable()
        {
            _tcp?.Dispose();
            _tcp = null;
        }

        public void SetProcessPacketState(bool enabled)
        {
            _processPackets = enabled;
        }

        public Task UniSendAsync(PacketBuilder packetBuilder, CancellationToken token)
        {
            return _tcp.UniSendAsync(packetBuilder, token);
        }

        public Task<Packet> BiSendAsync(PacketBuilder packetBuilder, CancellationToken token)
        {
            return _tcp.BiSendAsync(packetBuilder, token);
        }

        public async Task<T> BiSendAsync<T>(PacketBuilder packetBuilder, CancellationToken token)
            where T : IPacketSerializable, new()
        {
            Packet packet = await _tcp.BiSendAsync(packetBuilder, token);
            return packet.ToSerializedPacket<T>();
        }

        public sealed class TCP : IDisposable
        {
            public const int MAX_PACKET_SIZE = 1024 * 10;
            private readonly Memory<byte> _frameBuffer = new byte[] { 0, 0, 0, 0 };

            private readonly string _host;
            private readonly int _port;
            private readonly TcpClient _socket;
            private readonly LinkedList<Packet> _bi_packets;
            private readonly PacketQueue _packetQueue;

            private NetworkStream _stream;
            private bool _dead = false;
            public bool IsDead { get => _dead; }

            public TCP(string host, int port)
            {
                _host = host;
                _port = port;
                _socket = new TcpClient();
                _bi_packets = new();
                _packetQueue = new();
            }

            public async void ConnectAsync(CancellationToken token)
            {
                await _socket.ConnectAsync(_host, _port);
                _stream = _socket.GetStream();

                while (!_dead && !token.IsCancellationRequested)
                {
                    Packet packet = await ReadPacketAsync(token);
                    //Debug.Log($"new packet of type: {packet.PacketType}");
                    if (packet.PacketType.HasPacketId())
                    {
                        _bi_packets.AddLast(packet);
                    }
                    else
                    {
                        await _packetQueue.AddPacketAsync(packet);
                    }
                }
            }

            public Task<PacketMap> GetPacketMapAsync(CancellationToken token)
            {
                return _packetQueue.GetPacketMapAsync(token);
            }

            public async Task UniSendAsync(PacketBuilder packetBuilder, CancellationToken token)
            {
                try
                {
                    byte[] data = packetBuilder.Data.ToArray();
                    if (data.Length > MAX_PACKET_SIZE)
                    {
                        throw new Exception($"Trying to send a packet thats too big. ({packetBuilder.Data.Count}) - {data[4]}");
                    }

                    await _stream.WriteAsync(data, token);
                }
                catch (Exception ex)
                {
                    KillConnection(ex);
                    throw;
                }
            }

            public async Task<Packet> BiSendAsync(PacketBuilder packetBuilder, CancellationToken token)
            {
                int packetId = packetBuilder.PacketId.Value;

                await UniSendAsync(packetBuilder, token);

                while (!token.IsCancellationRequested)
                {
                    LinkedListNode<Packet> node = _bi_packets.First;
                    while (node is not null)
                    {
                        LinkedListNode<Packet> next = node.Next;

                        if (node.Value.PacketId == packetId)
                        {
                            _bi_packets.Remove(node.Value);
                            return node.Value;
                        }

                        node = next;
                    }

                    await Task.Yield();
                }

                throw new TimeoutException("Waiting for packet timed out.");
            }

            private async Task<int> ReadBufferAsync(Memory<byte> buffer, CancellationToken token)
            {
                int n = await _stream.ReadAsync(buffer);
                if (n == 0)
                {
                    //TODO: close connection
                    throw new Exception("Server closed the connection");
                }
                return n;
            }

            private async Task<Packet> ReadPacketAsync(CancellationToken token)
            {
                bool returnArray = false;
                byte[] data = null;

                try
                {
                    int n = await ReadBufferAsync(_frameBuffer, token);
                    if (n != sizeof(int))
                        throw new Exception($"Frame header of size {n} is invalid");

                    int length = BitConverter.ToInt32(_frameBuffer.Span);
                    if (length > MAX_PACKET_SIZE)
                        throw new Exception($"Packet size cannot be larger than {MAX_PACKET_SIZE}");

                    //borrow byte array
                    data = ArrayPool<byte>.New(length);
                    n = await ReadBufferAsync(data.AsMemory()[..length], token);
                    if (n != length)
                        throw new Exception($"Frame header of size {n} is invalid");

                    return new Packet(data, length);
                }
                catch (Exception ex)
                {
                    KillConnection(ex);
                    throw;
                }
                finally
                {
                    //return the data
                    if (returnArray && data is not null)
                    {
                        ArrayPool<byte>.Free(data);
                    }
                }
            }
            private void KillConnection(Exception ex)
            {
                Debug.Log(ex);
                _dead = true;
            }


            public void Dispose()
            {
                _stream?.Close();
                _stream?.Dispose();
                _stream = null;

                _socket.Close();
                _socket.Dispose();
            }
        }
    }
}