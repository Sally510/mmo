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


        void OnEnable()
        {
            PacketEventHandler.MonsterInfoEvent += PacketEventHandler_MonsterInfoEvent;
        }

        private void OnDisable()
        {
            PacketEventHandler.MonsterInfoEvent -= PacketEventHandler_MonsterInfoEvent;
        }

        private void PacketEventHandler_MonsterInfoEvent(object sender, MonsterListModel e)
        {
            foreach (MonsterModel monster in e.Monsters)
            {
                GameObject enemyObject = Instantiate(enemyPrefab, monster.Position.FromIsoToWorld(), Quaternion.identity);
                var obj = enemyObject.GetComponent<EnemyObject>();
                obj.Initialize(monster);
                enemies.Add(monster.EntityId, obj);
            }
        }
    }
}