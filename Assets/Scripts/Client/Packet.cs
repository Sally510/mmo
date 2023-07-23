using System;
using System.Text;
using Unity.VisualScripting;

namespace Assets.Scripts.Client
{
public enum PacketType : byte
{
    Walk = 1,
    AutoWalk = 2,
    StartAttack = 3,
    Login = 4,
    Logout = 5,
    CharacterCreate = 6,
    CharacterLogin = 7,
    CharacterLogout = 8,
    CharacterDelete = 9,
    PlayerMove = 10,
    PlayerInfo = 11,
    PlayerRemove = 12,
    Chat = 13,
    MonsterInfo = 14,
    MonsterChange = 15,
    Change = 16,
    NewChestDrop = 17,     //server broadcast that an chest was dropped
    OpenChest = 18,        //client side request to open the chest
    ItemDrop = 19,         //client side request to drop item
    ChestItems = 20,       //server send to client what items are in the chest
    NewInventoryItem = 21, //server broadcast to single client of new inventory item
    GetInventoryItems = 22,
    CommitInventoryState = 23,
    Buff = 24,
    TargetedSpell = 25,
    PickupItem = 26,       //client side request to pick up item
    EquipItem = 27,        //client side request to equip item
    GetEquippedItems = 28, //client side request to get all equiped items
    ExperienceChange = 29, //server sends to client experience
    RequestRespawn = 30,   //client side request to respawn
}

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

internal static class PacketTypeExtensions
{
    public static bool HasPacketId(this PacketType type)
    {
        return type switch
        {
            PacketType.CharacterCreate => true,
            PacketType.CharacterLogin => true,
            PacketType.CharacterLogout => true,
            PacketType.CharacterDelete => true,
            PacketType.Login => true,
            PacketType.Logout => true,
            PacketType.Walk => true,
            PacketType.GetInventoryItems => true,
            PacketType.CommitInventoryState => true,
            PacketType.ItemDrop => true,
            PacketType.PickupItem => true,
            PacketType.EquipItem => true,
            PacketType.GetEquippedItems => true,
            _ => false
        };
    }

    public static PacketType CastToPacketType(this byte value)
    {
        if (Enum.IsDefined(typeof(PacketType), value))
        {
            return (PacketType)value;
        }

        throw new Exception("Not a valid value for PacketType");
    }
}
}