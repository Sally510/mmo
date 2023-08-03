using Assets.Scripts.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client.Models
{
    public class AutoWalkModel: IPacketSerializable
    {
        public uint EntityId { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan WalkDuration { get; set; }
        public List<Vector2> Moves { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            uint entityId = packet.GetUInt();
            long startTime = packet.GetLong();

            EntityId = entityId;
            StartTime = DateTimeOffset.FromUnixTimeMilliseconds(startTime).UtcDateTime;
            WalkDuration = TimeSpan.FromMilliseconds(packet.GetLong());
            Moves = new List<Vector2>();

            while (packet.AnySpaceLeft)
            {
                Moves.Add(new Vector2(packet.GetFloat(), packet.GetFloat()));
            }
        }
    }
}
