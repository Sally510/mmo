using Assets.Scripts.Client.Models;
using Assets.Scripts.Client.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Client
{
    public static class ClientManager
    {
        public static Task AttackEnemy(uint entityId, CancellationToken token)
        {
            return Client.Instance.UniSendAsync(PacketBuilder.Create(PacketType.StartAttack)
                .SetInt((int)entityId), token);
        }

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

        public static Task<InventoryItemListModel> GetInventoryItemsAsync(CancellationToken token)
        {
            return Client.Instance.BiSendAsync<InventoryItemListModel>(PacketBuilder.Create(PacketType.GetInventoryItems), token);
        }

        public static async Task<bool> CommitInventoryStateAsync(List<(byte, byte)> swaps, CancellationToken token)
        {
            var builder = PacketBuilder.Create(PacketType.CommitInventoryState);
            foreach (var swap in swaps)
            {
                builder.SetByte(swap.Item1);
                builder.SetByte(swap.Item2);
            }

            return (await Client.Instance.BiSendAsync(builder, token)).GetBoolean();
        }

        public static Task<EquippedItemListModel> GetEquippedItemsAsync(CancellationToken token)
        {
            return Client.Instance.BiSendAsync<EquippedItemListModel>(PacketBuilder.Create(PacketType.GetEquippedItems), token);
        }

        public static Task OpenChestAsync(long droppedChestId, CancellationToken token)
        {
            return Client.Instance.UniSendAsync(PacketBuilder.Create(PacketType.OpenChest).SetLong(droppedChestId), token);
        }

        public static async Task<PickupStatusType> PickupItemAsync(long droppedItemId, CancellationToken token)
        {
            Packet packet = await Client.Instance.BiSendAsync(PacketBuilder.Create(PacketType.PickupItem).SetLong(droppedItemId), token);
            return (PickupStatusType)packet.GetByte();
        }
    }
}