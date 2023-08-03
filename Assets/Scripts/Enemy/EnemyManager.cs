using Assets.Scripts.Client.Models;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Client.PacketQueue;

namespace Assets.Scripts.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemyPrefab;
        private Dictionary<uint, EnemyObject> enemies = new();

        void FixedUpdate()
        {
            foreach (EnemyObject obj in enemies.Values)
            {
                if (obj.AutoMovement != null)
                {
                    if (obj.AutoMovement.IsDone)
                    {
                        obj.AutoMovement = null;
                        obj.StopMove();
                    }
                    else
                    {
                        obj.AutoMovement.Update(Time.fixedDeltaTime);
                        Vector3 worldPos = obj.AutoMovement.CurrentIsoPosition().FromIsoToWorld();
                        obj.MoveTo(worldPos);
                    }
                }
            }
        }

        async void Update()
        {
            await CheckMonsterInfo();
            await CheckMonsterChange();
            await CheckAutoWalks();


        }

        async Task CheckAutoWalks()
        {
            PacketList packetList = await Client.Client.Instance.GetPacketQueueAsync(Client.PacketType.AutoWalk, destroyCancellationToken);

            foreach (AutoWalkModel autowalk in packetList.ToDeserializedList<AutoWalkModel>())
            {
                if (enemies.TryGetValue(autowalk.EntityId, out EnemyObject enemy))
                {
                    enemy.AutoMovement = new AutoMovementManager(autowalk.StartTime, autowalk.WalkDuration, autowalk.Moves);
                }
            }
        }

        async Task CheckMonsterChange()
        {
            PacketList changePacketList = await Client.Client.Instance.GetPacketQueueAsync(Client.PacketType.MonsterChange, destroyCancellationToken);

            foreach (MonsterChangeListModel changes in changePacketList.ToDeserializedList<MonsterChangeListModel>())
            {
                foreach (MonsterChangeListModel.MonsterChange change in changes.Changes)
                {
                    if (change.HasFlag(ChangeState.Spawn))
                    {
                        Debug.Log($"Spawn: {change.EntityId}");
                        if (enemies.TryGetValue(change.EntityId, out EnemyObject enemy))
                        {
                            //monster.GetComponent<Rigidbody2D>().MovePosition(change.Position.FromIsoToWorld());

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

        async Task CheckMonsterInfo()
        {
            PacketList packetList = await Client.Client.Instance.GetPacketQueueAsync(Client.PacketType.MonsterInfo, destroyCancellationToken);

            foreach (MonsterListModel monsterList in packetList.ToDeserializedList<MonsterListModel>())
            {
                foreach (MonsterModel monster in monsterList.Monsters)
                {
                    GameObject enemyObject = Instantiate(enemyPrefab, monster.Position.FromIsoToWorld(), Quaternion.identity);
                    enemies.Add(monster.EntityId, new EnemyObject(monster, enemyObject));
                }
            }
        }
    }
}