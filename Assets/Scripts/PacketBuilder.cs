using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class PacketBuilder
{
    private static int _packet_id_counter = 0;

    private readonly List<byte> _data;
    public int? PacketId { get; set; }

    public PacketBuilder(PacketType packetType)
    {
        _data = new() { 0, 0, 0, 0 };
        SetByte((byte)packetType);

        if (packetType.HasPacketId())
        {
            PacketId = Interlocked.Increment(ref _packet_id_counter);
            SetInt(PacketId.Value);
        }
    }

    private void SetLength()
    {
        byte[] arr = BitConverter.GetBytes(_data.Count - 4);
        _data[0] = arr[0];
        _data[1] = arr[1];
        _data[2] = arr[2];
        _data[3] = arr[3];
    }

    public PacketBuilder SetByte(byte value)
    {
        _data.Add(value);
        return this;
    }
    public PacketBuilder SetInt(int value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
        return this;
    }

    public PacketBuilder SetFloat(float value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
        return this;
    }

    public PacketBuilder SetLong(long value)
    {
        _data.AddRange(BitConverter.GetBytes(value));
        return this;
    }

    public PacketBuilder SetByteArray(byte[] value)
    {
        _data.AddRange(value);
        return this;
    }
    public PacketBuilder SetFixedString(string value)
    {
        _data.AddRange(Encoding.UTF8.GetBytes(value));
        return this;
    }
    public PacketBuilder SetBreakString(string value)
    {
        _data.AddRange(Encoding.UTF8.GetBytes(value));
        _data.Add((byte)'\0');
        return this;
    }

    public List<byte> Data
    {
        get
        {
            SetLength();
            return _data;
        }
    }
}

