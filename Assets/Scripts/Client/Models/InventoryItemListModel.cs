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
                Items.Add(InventoryItemModel.Parse(packet));
            }
        }

        public class InventoryItemModel
        {
            public long ItemId { get; set; }
            public int Quantity { get; set; }
            public byte Slot { get; set; }

            public static InventoryItemModel Parse(Packet packet)
            {
                return new InventoryItemModel
                {
                    ItemId = packet.GetLong(),
                    Quantity = packet.GetInt(),
                    Slot = packet.GetByte(),
                };
            }
        }
    }
}
