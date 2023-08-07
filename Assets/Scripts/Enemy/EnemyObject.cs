using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using Assets.Scripts.Helpers;
using Assets.Scripts.Player;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyObject : MonoBehaviour
    {
        public MonsterModel Monster { get; private set; }
        private Rigidbody2D _rigidBody;
        private PlayerAnimation _animation;
        private HealthBar _healthBar;
        private AutoMovementManager _autoMovement;

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _animation = GetComponentInChildren<PlayerAnimation>();
        }

        public void Initialize(MonsterModel monster)
        {
            Monster = monster;

            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.SetMaxHealth(monster.MaxHealth);
            _healthBar.SetHealth(monster.Health);
        }

        void FixedUpdate()
        {
            if (_autoMovement != null)
            {
                if (_autoMovement.IsDone)
                {
                    _autoMovement = null;
                    StopMove();
                }
                else
                {
                    _autoMovement.Update(Time.fixedDeltaTime);
                    Vector3 worldPos = _autoMovement.CurrentIsoPosition().FromIsoToWorld();
                    MoveTo(worldPos);
                }
            }
        }

        public void MoveTo(Vector2 worldPosition)
        {
            Vector2 oldPos = _rigidBody.position;
            _rigidBody.MovePosition(worldPosition);
            _animation.SetDirection((worldPosition - oldPos).normalized);
        }

        public void StopMove()
        {
            _animation.SetDirection(Vector2.zero);
        }

        void OnEnable()
        {
            PacketEventHandler.MonsterChangeEvent += PacketEventHandler_MonsterChangeEvent;
            PacketEventHandler.AutoWalkEvent += PacketEventHandler_AutoWalkEvent;
        }

        private void OnDisable()
        {
            PacketEventHandler.MonsterChangeEvent -= PacketEventHandler_MonsterChangeEvent;
            PacketEventHandler.AutoWalkEvent -= PacketEventHandler_AutoWalkEvent;
        }

        private void PacketEventHandler_AutoWalkEvent(object sender, AutoWalkModel e)
        {
            if (Monster.EntityId == e.EntityId)
            {
                _autoMovement = new AutoMovementManager(e.StartTime, e.WalkDuration, e.Moves);
            }
        }

        private void PacketEventHandler_MonsterChangeEvent(object sender, MonsterChangeListModel e)
        {
            foreach (MonsterChangeListModel.MonsterChange change in e.Changes.Where(x => x.EntityId == Monster.EntityId))
            {
                if (change.HasFlag(ChangeState.Spawn))
                {
                    Debug.Log($"Spawn: {change.EntityId}");
                    _healthBar.SetHealth(change.Health);
                    _rigidBody.MovePosition(change.Position.FromIsoToWorld());
                }
                if (change.HasFlag(ChangeState.Damaged))
                {
                    Debug.Log($"Damaged: {change.EntityId}");
                    _healthBar.SetHealth(change.Health);
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
