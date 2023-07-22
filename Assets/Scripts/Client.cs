using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using Unity.VisualScripting;
using System.Collections;
using System.Threading;
using UnityEngine.UIElements;
using UnityEditor.Sprites;
using System.IO;
using System.Linq;

public class Client : MonoBehaviour
{
    public static Client Instance;
    
    public string host = "localhost";
    public int port = 13551;
    private TCP _tcp;
    
    void Awake()
    {
        Debug.Log("Waking client instance.");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log($"Set new client instance");
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start()
    {
        Debug.Log("Creating new TCP instance.");
        _tcp = new TCP(host, port);
        StartCoroutine(_tcp.Connect());
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

        private NetworkStream stream;

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
            stream = _socket.GetStream();

            //here we constantly read incoming packets
            while(true)
            {
                yield return Read();
            }
        }

        public IEnumerator UniSend(byte[] data, int offset, int size)
        {
            IAsyncResult result = stream.BeginWrite(data, offset, size, null, null);

            //TODO: add timeout
            while(!result.IsCompleted)
            {
                yield return null;
            }

            stream.EndWrite(result);
        }

        public IEnumerator BiSend(byte[] data, int offset, int size, int packetId, Action<Packet> callback)
        {
            IAsyncResult result = stream.BeginWrite(data, offset, size, null, null);

            //TODO: add timeout
            while (!result.IsCompleted)
            {
                yield return null;
            }

            stream.EndWrite(result);

            //TODO: timeout
            do
            {
                lock(_packets)
                {
                    LinkedListNode<Packet> node = _packets.First;
                    while (node is not null)
                    {
                        if(node.Value.PacketId == packetId)
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
                IAsyncResult result = stream.BeginRead(_frameBuffer, 0, _frameBuffer.Length, null, null);
                while (!result.IsCompleted)
                {
                    yield return null;
                }
                int n = stream.EndRead(result);
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
                result = stream.BeginRead(packetData, 0, packetLength, null, null);
                while (!result.IsCompleted)
                {
                    yield return null;
                }
                n = stream.EndRead(result);
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
            stream?.Close();
            stream?.Dispose();
            stream = null;

            _socket.Close();
            _socket.Dispose();
        }
    }
}