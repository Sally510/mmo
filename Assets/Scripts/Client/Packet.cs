using Assets.Scripts.Client.Interfaces;
using Assets.Scripts.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public sealed class Packet : IDisposable
    {
        private readonly byte[] _data;
        //the data can be larger than read because we're using an ArrayPool<byte>
        private readonly int _dataLength;

        private int _readPosition;
        public PacketType PacketType { get; set; }
        public int? PacketId { get; set; }

        public int Position { get => _readPosition; }
        public bool AnySpaceLeft { get => _dataLength - _readPosition > 0; }

        public Packet(byte[] data, int length)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _dataLength = length;
            _readPosition = 0;

            PacketType = GetByte().CastToPacketType();
            if (PacketType.HasPacketId())
            {
                PacketId = GetInt();
            }
        }

        public byte[] TakeDataSlice(int length, bool peek)
        {
            int targetLen = _readPosition + length;
            if (targetLen <= _dataLength)
            {
                byte[] res = _data[_readPosition..targetLen];

                if (!peek)
                {
                    _readPosition += length;
                }

                return res;
            }
            throw new Exception("no more buffer left to read");
        }

        public bool GetBoolean(bool peek = false) => BitConverter.ToBoolean(TakeDataSlice(1, peek), 0);
        public byte GetByte(bool peek = false) => TakeDataSlice(1, peek)[0];
        public short GetShort(bool peek = false) => BitConverter.ToInt16(TakeDataSlice(2, peek));
        public int GetInt(bool peek = false) => BitConverter.ToInt32(TakeDataSlice(4, peek));
        public uint GetUInt(bool peek = false) => (uint)GetInt(peek);
        public float GetFloat(bool peek = false) => BitConverter.ToSingle(TakeDataSlice(4, peek));
        public Vector2 GetVector2(bool peek = false) => new Vector2(GetFloat(peek), GetFloat(peek));
        public long GetLong(bool peek = false) => BitConverter.ToInt64(TakeDataSlice(8, peek));
        public byte[] GetByteArray(int length, bool peek = false) => TakeDataSlice(length, peek);
        public string GetFixedString(int length, bool peek = false) => Encoding.UTF8.GetString(TakeDataSlice(length, peek));
        public string GetBreakString(bool peek = false)
        {
            bool found = false;
            int i = _readPosition;
            for (; i < _dataLength; i++)
            {
                if (_data[i] == 0)
                {
                    found = true;
                    i++; //to count the null char
                    break;
                }
            }
            if (!found)
            {
                throw new Exception("Didn't find null character");
            }
            int length = i - _readPosition;
            byte[] bytes = TakeDataSlice(length, peek);
            return Encoding.UTF8.GetString(bytes[0..(length - 1)]);
        }

        public void Dispose()
        {
            ArrayPool<byte>.Free(_data);
        }
    }

    public static class PacketExtensions 
    {
        public static IEnumerable<T> ToSerializedPackets<T>(this IEnumerable<Packet> packets)
                where T : IPacketSerializable, new()
        {
            if (packets == null)
                throw new ArgumentNullException(nameof(packets));

            if (packets.Any())
            {
                return packets.Select(x => x.ToSerializedPacket<T>());
            }
            return Enumerable.Empty<T>();
        }

        public static T ToSerializedPacket<T>(this Packet packet)
                where T : IPacketSerializable, new()
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            T res = new();
            res.Deserialize(packet);
            return res;
        }
    }
}