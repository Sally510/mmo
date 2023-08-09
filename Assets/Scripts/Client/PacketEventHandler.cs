using Assets.Scripts.Client.Models;
using Assets.Scripts.Client.Types;
using System;

namespace Assets.Scripts.Client
{
    public static class PacketEventHandler
    {
        public static event EventHandler<MonsterListModel> MonsterInfoEvent;
        public static event EventHandler<MonsterChangeListModel> MonsterChangeEvent;
        public static event EventHandler<AutoWalkModel> AutoWalkEvent;
        public static event EventHandler<ChestDropModel> ChestDropEvent;
        public static event EventHandler<ChestItemsModel> ChestItemsEvent;

        public static void RaiseEvent(Packet[] packets)
        {
            foreach (Packet packet in packets)
            {
                switch (packet.PacketType)
                {
                    case PacketType.MonsterInfo:
                        MonsterInfoEvent?.Invoke(typeof(PacketEventHandler), packet.ToSerializedPacket<MonsterListModel>());
                        break;
                    case PacketType.MonsterChange:
                        MonsterChangeEvent?.Invoke(typeof(PacketEventHandler), packet.ToSerializedPacket<MonsterChangeListModel>());
                        break;
                    case PacketType.AutoWalk:
                        AutoWalkEvent?.Invoke(typeof(PacketEventHandler), packet.ToSerializedPacket<AutoWalkModel>());
                        break;
                    case PacketType.ChestDrop:
                        ChestDropEvent?.Invoke(typeof(PacketEventHandler), packet.ToSerializedPacket<ChestDropModel>());
                        break;
                    case PacketType.ChestItems:
                        ChestItemsEvent?.Invoke(typeof(PacketEventHandler), packet.ToSerializedPacket<ChestItemsModel>());
                        break;
                }
            }
        }
    }
}
