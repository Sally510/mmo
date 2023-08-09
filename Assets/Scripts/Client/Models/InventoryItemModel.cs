using Assets.Scripts.Client.Interfaces;

namespace Assets.Scripts.Client.Models
{
    public class InventoryItemModel : IPacketSerializable
    {
        public long ItemId { get; set; }
        public int Quantity { get; set; }
        public byte Slot { get; set; }

        public void Deserialize(Packet packet)
        {
            ItemId = packet.GetLong();
            Quantity = packet.GetInt();
            Slot = packet.GetByte();
        }
    }
}
