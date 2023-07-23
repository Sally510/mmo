using Assets.Scripts.Client.Models;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor.Compilation;

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
}
}