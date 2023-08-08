using Assets.Scripts.Client.Interfaces;
using Assets.Scripts.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        ChestDrop = 17,     //server broadcast that an chest was dropped
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
