using Assets.Scripts.Client.Models;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Client.PacketQueue;

namespace Assets.Scripts
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemyPrefab;
        private Dictionary<uint, GameObject> monsters = new();


        async void Update()
        {
            await CheckMonsterInfo();
            await CheckMonsterChange();

            
        }

        async Task CheckMonsterChange()
        {
            PacketList changePacketList = await Client.Client.Instance.GetPacketQueueAsync(Client.PacketType.MonsterChange, destroyCancellationToken);
            if (changePacketList.Packets.Count > 0)
            {
                foreach (MonsterChangeListModel changes in changePacketList.ToDeserializedList<MonsterChangeListModel>())
                {
                    foreach (MonsterChangeListModel.MonsterChange change in changes.Changes)
                    {
                        if (change.HasFlag(ChangeState.Spawn))
                        {
                            Debug.Log($"Spawn: {change.EntityId}");
                            if (monsters.TryGetValue(change.EntityId, out GameObject monster))
                            {
                                monster.GetComponent<Rigidbody2D>().MovePosition(change.Position.FromIsoToWorld());

                                //monster.Direction = change.Direction;
                                //monster.Heal(change.Health);
                            }
                        }
                        if (change.HasFlag(ChangeState.Damaged))
                        {
                            Debug.Log($"Damaged: {change.EntityId}");
                        }
                        if (change.HasFlag(ChangeState.Died))
                        {
                            Debug.Log($"Died: {change.EntityId}");
                        }
                        if (change.HasFlag(ChangeState.Attack))
                        {
                            Debug.Log($"Entity {change.EntityId} attacked!");
                        }
                    }
                }
            }
        }

        async Task CheckMonsterInfo()
        {
            PacketList packetList = await Client.Client.Instance.GetPacketQueueAsync(Client.PacketType.MonsterInfo, destroyCancellationToken);
            if (packetList.Packets.Count > 0)
            {
                foreach (MonsterListModel monsterList in packetList.ToDeserializedList<MonsterListModel>())
                {
                    foreach (MonsterListModel.MonsterModel monster in monsterList.Monsters)
                    {
                        GameObject enemy = Instantiate(enemyPrefab, monster.Position.FromIsoToWorld(), Quaternion.identity);
                        monsters.Add(monster.EntityId, enemy);
                    }
                }
            }
        }
    }
}