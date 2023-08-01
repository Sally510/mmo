namespace Assets.Scripts.Client.Interfaces
{
    public interface IPacketSerializable
    {
        void Deserialize(Packet packet);
    }
}
