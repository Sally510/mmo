using Assets.Scripts.Client.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class ChestItemsModel : IPacketSerializable
    {
        public long DroppedChestId { get; set; }
        public List<DroppedItemModel> Items { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            DroppedChestId = packet.GetLong();

            while (packet.AnySpaceLeft)
            {
                Items.Add(new DroppedItemModel
                {
                    DroppedItemId = packet.GetLong(),
                    ItemId = packet.GetLong(),
                    Quantity = packet.GetInt()
                });
            }
        }

        public class DroppedItemModel
        {
            public long DroppedItemId { get; set; }
            public long ItemId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
