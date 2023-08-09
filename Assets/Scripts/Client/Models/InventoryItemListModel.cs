using Assets.Scripts.Client.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class InventoryItemListModel : IPacketSerializable
    {
        public List<InventoryItemModel> Items { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            while (packet.AnySpaceLeft)
            {
                Items.Add(packet.ToSerializedPacket<InventoryItemModel>());
            }
        }
    }
}
