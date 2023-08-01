using Assets.Scripts.Client.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class DeleteCharacterModel : IPacketSerializable
    {
        public bool Success { get; set; }
        public List<CharacterOptionModel> CharacterList { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            Success = packet.GetBoolean();

            if (Success)
            {
                while (packet.AnySpaceLeft)
                {
                    CharacterList.Add(CharacterOptionModel.Parse(packet));
                }
            }
        }
    }
}
