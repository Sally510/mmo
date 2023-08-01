using Assets.Scripts.Client.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Client
{
    public static class ClientManager
    {
        public static Task<LoginModel> LoginAsync(string access, CancellationToken token)
        {
            return Client.Instance.BiSendAsync<LoginModel>(PacketBuilder.Create(PacketType.Login)
                .SetBreakString(access), token);
        }

        public static Task<LoginCharacterModel> LoginCharacterAsync(long characterId, CancellationToken token)
        {
            return Client.Instance.BiSendAsync<LoginCharacterModel>(PacketBuilder.Create(PacketType.CharacterLogin)
                .SetLong(characterId), token);
        }

        public static Task<CreateCharacterModel> CreateCharacterAsync(string name, CancellationToken token)
        {
            return Client.Instance.BiSendAsync<CreateCharacterModel>(PacketBuilder.Create(PacketType.CharacterCreate)
                .SetBreakString(name), token);
        }

        public static Task<DeleteCharacterModel> DeleteCharacterAsync(long characterId, CancellationToken token)
        {
            return Client.Instance.BiSendAsync<DeleteCharacterModel>(PacketBuilder.Create(PacketType.CharacterDelete)
               .SetLong(characterId), token);
        }

        public static Task<WalkModel> SendMovePacketAsync(float angle, float elapsedSeconds, CancellationToken token)
        {
            return Client.Instance.BiSendAsync<WalkModel>(PacketBuilder.Create(PacketType.Walk)
                .SetFloat(angle)
                .SetFloat(elapsedSeconds), token);
        }

        public static Task<InventoryItemListModel> GetInventoryItems(CancellationToken token)
        {
            return Client.Instance.BiSendAsync<InventoryItemListModel>(new PacketBuilder(PacketType.GetInventoryItems), token);
        }

        public static Task<EquippedItemListModel> GetEquippedItems(CancellationToken token)
        {
            return Client.Instance.BiSendAsync<EquippedItemListModel>(new PacketBuilder(PacketType.GetEquippedItems), token);
        }
    }
}