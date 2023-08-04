using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject enemyPrefab;
        private readonly Dictionary<uint, EnemyObject> enemies = new();

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

        void OnEnable()
        {
            PacketEventHandler.MonsterInfoEvent += PacketEventHandler_MonsterInfoEvent;
            PacketEventHandler.MonsterChangeEvent += PacketEventHandler_MonsterChangeEvent;
            PacketEventHandler.AutoWalkEvent += PacketEventHandler_AutoWalkEvent;
        }

        private void OnDisable()
        {
            PacketEventHandler.MonsterInfoEvent -= PacketEventHandler_MonsterInfoEvent;
            PacketEventHandler.MonsterChangeEvent -= PacketEventHandler_MonsterChangeEvent;
            PacketEventHandler.AutoWalkEvent -= PacketEventHandler_AutoWalkEvent;
        }

        private void PacketEventHandler_AutoWalkEvent(object sender, AutoWalkModel e)
        {
            if (enemies.TryGetValue(e.EntityId, out EnemyObject enemy))
            {
                enemy.AutoMovement = new AutoMovementManager(e.StartTime, e.WalkDuration, e.Moves);
            }
        }

        private void PacketEventHandler_MonsterChangeEvent(object sender, MonsterChangeListModel e)
        {
            foreach (MonsterChangeListModel.MonsterChange change in e.Changes)
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

        private void PacketEventHandler_MonsterInfoEvent(object sender, MonsterListModel e)
        {
            foreach (MonsterModel monster in e.Monsters)
            {
                GameObject enemyObject = Instantiate(enemyPrefab, monster.Position.FromIsoToWorld(), Quaternion.identity);
                enemies.Add(monster.EntityId, new EnemyObject(monster, enemyObject));
            }
        }
    }
}