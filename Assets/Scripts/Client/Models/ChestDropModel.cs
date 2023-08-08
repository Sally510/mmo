using Assets.Scripts.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client.Models
{
    public class ChestDropModel : IPacketSerializable
    {
        public long DroppedChestId { get; set; }
        public Vector2 IsoPosition { get; set; }
        public DateTime Expiry { get; set; }

        public void Deserialize(Packet packet)
        {
            DroppedChestId = packet.GetLong();
            IsoPosition = new(packet.GetFloat(), packet.GetFloat());
            Expiry = packet.GetUtcDateTime();
        }
    }
}
