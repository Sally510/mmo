using Assets.Scripts.Client.Models;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public static class ClientManager
    {
        public static IEnumerator Login(string access, Action<LoginModel> callback)
        {
            yield return Client.Instance.BiSend(PacketBuilder.Create(PacketType.Login)
                .SetBreakString(access),
                packet =>
                {
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

                    callback?.Invoke(model);
                }
            );
        }

        public static IEnumerator LoginCharacter(long characterId, Action<LoginCharacterModel> callback)
        {
            yield return Client.Instance.BiSend(PacketBuilder.Create(PacketType.CharacterLogin)
                .SetLong(characterId),
                packet =>
                {
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

                    callback?.Invoke(model);
                }
            );
        }

        public static IEnumerator CreateCharacter(string name, Action<CreateCharacterModel> callback)
        {
            yield return Client.Instance.BiSend(PacketBuilder.Create(PacketType.CharacterCreate)
                .SetBreakString(name),
                packet =>
                {
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

                    callback?.Invoke(model);
                }
            );
        }

        public static IEnumerator DeleteCharacter(long characterId, Action<DeleteCharacterModel> callback)
        {
            yield return Client.Instance.BiSend(PacketBuilder.Create(PacketType.CharacterDelete)
               .SetLong(characterId),
               packet =>
               {
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

                   callback?.Invoke(model);
               }
           );
        }
    }
}