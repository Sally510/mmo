namespace Assets.Scripts.Client.Models
{
    public class EquippedItemModel
    {
        public long ItemId { get; set; }
        public EquipmentSlotType Slot { get; set; }

        public static EquippedItemModel Parse(Packet packet)
        {
            return new EquippedItemModel
            {
                ItemId = packet.GetLong(),
                Slot = (EquipmentSlotType)packet.GetByte(),
            };
        }
    }
}
