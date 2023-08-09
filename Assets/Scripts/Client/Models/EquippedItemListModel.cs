using Assets.Scripts.Client.Interfaces;
using Assets.Scripts.Client.Types;
using System.Collections.Generic;

namespace Assets.Scripts.Client.Models
{
    public class EquippedItemListModel : IPacketSerializable
    {
        public List<EquippedItemModel> Items { get; set; } = new();

        public void Deserialize(Packet packet)
        {
            while (packet.AnySpaceLeft)
            {
                Items.Add(EquippedItemModel.Parse(packet));
            }
        }

        public class EquippedItemModel
        {
            public long ItemId { get; set; }
            public EquipmentSlotType Slot { get; set; }

            public static EquippedItemModel Parse(Packet packet)
            {
                return new EquippedItemModel
                {
                    ItemId = packet.GetLong(),
                    Slot = (EquipmentSlotType)packet.GetShort(),
                };
            }
        }
    }
}
