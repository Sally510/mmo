using Assets.Scripts.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Client
{
    public static class ClientManager
    {
        public static async Task<LoginModel> LoginAsync(string access, CancellationToken token)
        {
            Packet packet = await Client.Instance.BiSendAsync(PacketBuilder.Create(PacketType.Login)
                .SetBreakString(access), token);

            LoginModel model = new()
            {
                Status = (LoginModel.StatusType)packet.GetByte()
            };

            if (model.Status == LoginModel.StatusType.Success)
            {
                while (packet.AnySpaceLeft)
                {
                    model.CharacterList.Add(CharacterOptionModel.Parse(packet));
                }
            }

            return model;
        }

        public static async Task<LoginCharacterModel> LoginCharacterAsync(long characterId, CancellationToken token)
        {
            Packet packet = await Client.Instance.BiSendAsync(PacketBuilder.Create(PacketType.CharacterLogin)
                .SetLong(characterId), token);

            LoginCharacterModel model = new()
            {
                Success = packet.GetBoolean()
            };

            if (model.Success)
            {
                model.SelectedCharacter = new CharacterModel
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
                    model.MapCharacter.Add(new CharacterModel
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

            return model;
        }

        public static async Task<CreateCharacterModel> CreateCharacterAsync(string name, CancellationToken token)
        {
            Packet packet = await Client.Instance.BiSendAsync(PacketBuilder.Create(PacketType.CharacterCreate)
                .SetBreakString(name), token);

            CreateCharacterModel model = new()
            {
                Status = (CreateCharacterModel.StatusType)packet.GetByte()
            };

            if (model.Status == CreateCharacterModel.StatusType.Success)
            {
                while (packet.AnySpaceLeft)
                {
                    model.CharacterList.Add(CharacterOptionModel.Parse(packet));
                }
            }

            return model;
        }

        public static async Task<DeleteCharacterModel> DeleteCharacterAsync(long characterId, CancellationToken token)
        {
            Packet packet = await Client.Instance.BiSendAsync(PacketBuilder.Create(PacketType.CharacterDelete)
               .SetLong(characterId), token);

            DeleteCharacterModel model = new()
            {
                Success = packet.GetBoolean()
            };

            if (model.Success)
            {
                while (packet.AnySpaceLeft)
                {
                    model.CharacterList.Add(CharacterOptionModel.Parse(packet));
                }
            }

            return model;
        }

        public static async Task<WalkModel> SendMovePacketAsync(float angle, float elapsedSeconds, CancellationToken token)
        {
            Packet packet = await Client.Instance.BiSendAsync(PacketBuilder.Create(PacketType.Walk)
                .SetFloat(angle)
                .SetFloat(elapsedSeconds), token);

            return new WalkModel
            {
                IsoPosition = packet.GetVector2(),
                Angle = packet.GetFloat()
            };
        }
    }
}