using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using Unity.VisualScripting;

public class Client : MonoBehaviour
{
    public static Client Instance;
    

    public string host = "89.142.170.95";
    public int port = 13551;
    public TCP tcp;
    private readonly Queue<Packet> _packets = new();

    private void Awake()
    {
        Debug.Log("Waking client instance.");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log($"Set new client instance {tcp != null}");
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //this gets called by unity
    private void Start()
    {
        Debug.Log("Creating new TCP instance.");
        tcp = new TCP();
        ConnectToServer();
    }

    public void ConnectToServer()
    {
        tcp.Connect(host, port);
    }

    public void Send(PacketBuilder builder)
    {
        tcp.Send(builder.Data.ToArray(), 0, builder.Data.Count);
    }

    

    public sealed class TCP : IDisposable
    {
        public const int MAX_PACKET_SIZE = 1024 * 10;
        private readonly byte[] _frameBuffer = new byte[] { 0, 0, 0, 0 };

        public TcpClient socket;

        private NetworkStream stream;

        private byte[] packetData = null;
        private int packetLength = 0;

        public void Connect(string host, int port)
        {
            socket = new TcpClient();
            socket.BeginConnect(host, port, ConnectCallback, socket);
        }

        public void Send(byte[] data, int offset, int size)
        {
            stream.BeginWrite(data, offset, size, WriteCallback, null);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            //start the first read
            stream.BeginRead(_frameBuffer, 0, _frameBuffer.Length, ReceiveCallback, ReadType.FrameHeader);
        }

        private void WriteCallback(IAsyncResult result)
        {
            try
            {
                stream.EndWrite(result);
            }
            catch
            {
                // TODO: disconnect
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int _byteLength = stream.EndRead(result);
                if (_byteLength <= 0)
                {
                    // TODO: disconnect
                    return;
                }

                ReadType readType = (ReadType)result.AsyncState;
                if (readType == ReadType.FrameHeader)
                {
                    int length = BitConverter.ToInt32(_frameBuffer);
                    if (length > MAX_PACKET_SIZE)
                        throw new Exception($"Packet size cannot be larger than {MAX_PACKET_SIZE}");
                    packetLength = length;
                    packetData = ArrayPool<byte>.New(length);

                    //read data
                    stream.BeginRead(packetData, 0, length, ReceiveCallback, ReadType.Data);
                }
                else if (readType == ReadType.Data)
                {
                    lock(Instance._packets)
                    {
                        Instance._packets.Enqueue(new Packet(packetData, packetLength));
                    }

                    packetData = null;
                    packetLength = 0;

                    //read the next packet header
                    stream.BeginRead(_frameBuffer, 0, _frameBuffer.Length, ReceiveCallback, ReadType.FrameHeader);
                }
            }
            catch
            {
                // TODO: disconnect
            }
        }

        public void Dispose()
        {
            if (packetData != null)
            {
                ArrayPool<byte>.Free(packetData);
            }
        }
    }

    public enum ReadType
    {
        FrameHeader,
        Data
    }
}