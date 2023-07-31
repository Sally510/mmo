namespace Assets.Scripts.Client.Models
{
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
