
using Assets.Scripts.Client.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Client.Models
{
    public class WalkModel : IPacketSerializable
    {
        public Vector2 IsoPosition { get; set; }
        public float Angle { get; set; }

        public void Deserialize(Packet packet)
        {
            IsoPosition = packet.GetVector2();
            Angle = packet.GetFloat();
        }
    }
}
