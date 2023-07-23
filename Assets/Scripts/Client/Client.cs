using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;

        public string host = ConfigurationManager.Config.Host;
        public int port = ConfigurationManager.Config.Port;
        private TCP _tcp;

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

        void Start()
        {
            Debug.Log("Creating new TCP instance.");
            if (_tcp == null)
            {
                _tcp = new(host, port);
                StartCoroutine(_tcp.Connect());
            }
        }

        void Reset()
        {
            Instance = null;
        }

        public IEnumerator UniSend(PacketBuilder packetBuilder)
        {
            if (packetBuilder is null)
                throw new ArgumentNullException(nameof(packetBuilder));

            byte[] data = packetBuilder.Data.ToArray();
            yield return _tcp.UniSend(data, 0, data.Length);
        }

        public IEnumerator BiSend(PacketBuilder packetBuilder, Action<Packet> callback)
        {
            if (packetBuilder is null)
                throw new ArgumentNullException(nameof(packetBuilder));
            if (!packetBuilder.PacketId.HasValue)
                throw new ArgumentException("Packets without a packet ID cannot dont have reponses.", nameof(packetBuilder));

            byte[] data = packetBuilder.Data.ToArray();
            yield return _tcp.BiSend(data, 0, data.Length, packetBuilder.PacketId.Value, callback);
        }


        public sealed class TCP : IDisposable
        {
            public const int MAX_PACKET_SIZE = 1024 * 10;
            private readonly byte[] _frameBuffer = new byte[] { 0, 0, 0, 0 };

            private readonly string _host;
            private readonly int _port;
            private readonly TcpClient _socket;
            private readonly LinkedList<Packet> _packets;

            private NetworkStream _stream;

            public TCP(string host, int port)
            {
                _host = host;
                _port = port;
                _socket = new TcpClient();
                _packets = new();
            }

            public IEnumerator Connect()
            {
                IAsyncResult result = _socket.BeginConnect(_host, _port, null, null);

                //TODO: add timeout
                while (!result.IsCompleted)
                {
                    yield return null;
                }

                _socket.EndConnect(result);
                _stream = _socket.GetStream();

                //here we constantly read incoming packets
                while (true)
                {
                    yield return Read();
                }
            }

            public IEnumerator UniSend(byte[] data, int offset, int size)
            {
                IAsyncResult result = _stream.BeginWrite(data, offset, size, null, null);

                //TODO: add timeout
                while (!result.IsCompleted)
                {
                    yield return null;
                }

                _stream.EndWrite(result);
            }

            public IEnumerator BiSend(byte[] data, int offset, int size, int packetId, Action<Packet> callback)
            {
                IAsyncResult result = _stream.BeginWrite(data, offset, size, null, null);

                //TODO: add timeout
                while (!result.IsCompleted)
                {
                    yield return null;
                }

                _stream.EndWrite(result);

                //TODO: timeout
                do
                {
                    lock (_packets)
                    {
                        LinkedListNode<Packet> node = _packets.First;
                        while (node is not null)
                        {
                            if (node.Value.PacketId == packetId)
                            {
                                _packets.Remove(node);
                                callback.Invoke(node.Value);
                                yield break;
                            }

                            node = node.Next;
                        }
                    }

                    yield return null;
                } while (true);
            }

            private IEnumerator Read()
            {
                byte[] packetData = null;
                bool freePacketData = false;

                try
                {
                    IAsyncResult result = _stream.BeginRead(_frameBuffer, 0, _frameBuffer.Length, null, null);
                    while (!result.IsCompleted)
                    {
                        yield return null;
                    }
                    int n = _stream.EndRead(result);
                    if (n == 0)
                    {
                        //TODO: disconnect
                        yield break;
                    }

                    int length = BitConverter.ToInt32(_frameBuffer);
                    if (length > MAX_PACKET_SIZE)
                        throw new Exception($"Packet size cannot be larger than {MAX_PACKET_SIZE}");
                    int packetLength = length;
                    packetData = ArrayPool<byte>.New(length);
                    freePacketData = true;

                    //read the next packet header
                    result = _stream.BeginRead(packetData, 0, packetLength, null, null);
                    while (!result.IsCompleted)
                    {
                        yield return null;
                    }
                    n = _stream.EndRead(result);
                    if (n == 0)
                    {
                        //TODO: disconnect
                        yield break;
                    }

                    //enqueue the packet
                    lock (_packets)
                    {
                        _packets.AddFirst(new Packet(packetData, packetLength));
                        freePacketData = false;
                    }

                }
                finally
                {
                    if (freePacketData && packetData != null)
                    {
                        ArrayPool<byte>.Free(packetData);
                    }
                }
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