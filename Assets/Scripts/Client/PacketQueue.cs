using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.Client
{
    public class PacketQueue
    {
        private readonly SemaphoreSlim _packetLock = new(1, 1);
        private readonly Dictionary<PacketType, Queue<Packet>> _packets;

        public PacketQueue()
        {
            _packets = new();
            foreach (PacketType type in Enum
                .GetValues(typeof(PacketType))
                .Cast<PacketType>()
                .Where(x => !x.HasPacketId()))
            {
                _packets.Add(type, new Queue<Packet>());
            }
        }

        public async Task AddPacketAsync(Packet packet)
        {
            if (packet.PacketType.HasPacketId())
                throw new Exception("Can not save client side packets");

            await _packetLock.WaitAsync();
            try
            {
                _packets[packet.PacketType].Enqueue(packet);
            }
            finally
            {
                _packetLock.Release();
            }
        }

        public async Task<PacketList> GetPacketQueueAsync(PacketType type)
        {
            if (type.HasPacketId())
                throw new Exception("Can not get client side packets");

            await _packetLock.WaitAsync();
            try
            {
                return PacketList.EmptyQueue(_packets[type]);
            }
            finally
            {
                _packetLock.Release();
            }
        }

        public sealed class PacketList : IDisposable
        {
            private readonly bool _empty;
            private readonly List<Packet> _packets;
            public List<Packet> Packets { get => _packets; }

            private static readonly PacketList Empty = new(new List<Packet>(), true);
   
            private PacketList(List<Packet> packets, bool empty)
            {
                _packets = packets;
                _empty = empty;
            }

            public static PacketList EmptyQueue(Queue<Packet> packets)
            {
                if(packets.Count > 0)
                {
                    PacketList list = new(packets.ToListPooled(), false);
                    packets.Clear();
                    return list;
                }
                return Empty;
            }

            public void Dispose()
            {
                if (!_empty)
                {
                    _packets?.Free();
                }
            }
        }
    }
}
