using Assets.Scripts.Client.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Client.Models
{
    public class MonsterChangeListModel : IPacketSerializable
    {
        public List<MonsterChange> Changes { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            while (packet.AnySpaceLeft)
            {
                Changes.Add(MonsterChange.Parse(packet));
            }
        }

        public class MonsterChange
        {
            private byte _flag;

            public uint EntityId { get; set; }
            public int Health { get; set; }
            public Vector2 Position { get; set; }
            public float Direction { get; set; }

            public bool HasFlag(ChangeState flag)
            {
                return (_flag & (byte)flag) > 0;
            }

            public static MonsterChange Parse(Packet packet)
            {
                var change = new MonsterChange
                {
                    _flag = packet.GetByte(),
                    EntityId = packet.GetUInt(),
                };

                byte healthMask = (byte)ChangeState.Damaged | (byte)ChangeState.Spawn;
                if ((change._flag & healthMask) > 0)
                {
                    change.Health = packet.GetInt();
                }

                byte mask = (byte)ChangeState.Spawn | (byte)ChangeState.Moved | (byte)ChangeState.Died;
                if ((change._flag & mask) > 0)
                {
                    change.Position = packet.GetVector2();
                    change.Direction = packet.GetFloat();
                }

                return change;
            }
        }
    }
}
