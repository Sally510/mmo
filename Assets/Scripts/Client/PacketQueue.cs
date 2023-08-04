using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.Client
{
    public class PacketQueue
    {
        private readonly SemaphoreSlim _packetLock = new(1, 1);
        private readonly Dictionary<PacketType, List<Packet>> _packets;

        public PacketQueue()
        {
            _packets = new();
            foreach (PacketType type in Enum
                .GetValues(typeof(PacketType))
                .Cast<PacketType>()
                .Where(x => !x.HasPacketId()))
            {
                _packets.Add(type, new List<Packet>());
            }
        }

        public async Task AddPacketAsync(Packet packet)
        {
            if (packet.PacketType.HasPacketId())
                throw new Exception("Can not save client side packets");

            await _packetLock.WaitAsync();
            try
            {
                _packets[packet.PacketType].Add(packet);
            }
            finally
            {
                _packetLock.Release();
            }
        }

        public async Task<PacketMap> GetPacketMapAsync(CancellationToken token)
        {
            await _packetLock.WaitAsync(token);
            try
            {
                PacketMap packetMap = null;

                foreach (var packet in _packets.Where(x => x.Value.Count > 0))
                {
                    packetMap ??= new();
                    packetMap.Add(packet.Key, packet.Value);
                    packet.Value.Clear();
                }

                return packetMap ?? PacketMap.Empty;
            }
            finally
            {
                _packetLock.Release();
            }
        }
    }

    public sealed class PacketMap : IDisposable
    {
        public static readonly PacketMap Empty = new();

        private readonly Dictionary<PacketType, Packet[]> _map = new();
        public Dictionary<PacketType, Packet[]> Map { get => _map; }
        public bool IsEmpty => Map.Count == 0;

        public void Add(PacketType type, List<Packet> packets)
        {
            _map.Add(type, packets.ToArrayPooled());
        }

        public void Dispose()
        {
            if (_map != null && _map.Count > 0)
            {
                foreach (Packet[] packet in _map.Values)
                {
                    packet?.Free();
                }
            }
        }
    }
}
