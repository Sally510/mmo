using Assets.Scripts.Client.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class CreateCharacterModel : IPacketSerializable
    {
        public StatusType Status { get; set; }
        public List<CharacterOptionModel> CharacterList { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            Status = (StatusType)packet.GetByte();

            if (Status == StatusType.Success)
            {
                while (packet.AnySpaceLeft)
                {
                    CharacterList.Add(CharacterOptionModel.Parse(packet));
                }
            }
        }

        public enum StatusType
        {
            Success = 1,
            NameExists = 2,
            TooManyCharacters = 3,
            Unknown = 4,
        }
    }
}
