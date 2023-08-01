using Assets.Scripts.Client.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class LoginCharacterModel : IPacketSerializable
    {
        public bool Success { get; set; }

        public CharacterModel SelectedCharacter { get; set; }
        public List<CharacterModel> MapCharacter { get; set; } = new List<CharacterModel>();

        public void Deserialize(Packet packet)
        {
            Success = packet.GetBoolean();

            if (Success)
            {
                SelectedCharacter = new CharacterModel
                {
                    MapId = packet.GetInt(),
                    Experience = packet.GetUInt(),
                    EntityId = packet.GetUInt(),
                    IsoPosition = packet.GetVector2(),
                    Angle = packet.GetFloat(),
                    Health = packet.GetInt(),
                    MaxHealth = packet.GetInt(),
                };

                while (packet.AnySpaceLeft)
                {
                    MapCharacter.Add(new CharacterModel
                    {
                        EntityId = packet.GetUInt(),
                        Experience = packet.GetUInt(),
                        MapId = packet.GetInt(),
                        IsoPosition = packet.GetVector2(),
                        Angle = packet.GetFloat(),
                        Health = packet.GetInt(),
                        MaxHealth = packet.GetInt(),
                    });
                }
            }
        }
    }
}
