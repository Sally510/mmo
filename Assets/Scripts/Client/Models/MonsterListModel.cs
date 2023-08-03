using Assets.Scripts.Client.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Client.Models
{
    public class MonsterListModel : IPacketSerializable
    {
        public List<MonsterModel> Monsters { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            while (packet.AnySpaceLeft)
            {
                Monsters.Add(new MonsterModel
                {
                    EntityId = packet.GetUInt(),
                    Position = packet.GetVector2(),
                    Health = packet.GetInt(),
                    MaxHealth = packet.GetInt(),
                    Direction = packet.GetFloat()
                });
            }
        }
    }

    public class MonsterModel
    {
        public uint EntityId { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public Vector2 Position { get; set; }
        public float Direction { get; set; }
    }
}
