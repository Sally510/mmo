using Assets.Scripts.Client.Models;
using Assets.Scripts.Player;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyObject: MonoBehaviour
    {
        public MonsterModel Monster { get; private set; }
        private Rigidbody2D _rigidBody;
        private PlayerAnimation _animation;
        public AutoMovementManager AutoMovement { get; set; }

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _animation = GetComponentInChildren<PlayerAnimation>();
        }

        public void Initialize(MonsterModel monster)
        {
            Monster = monster;
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
    }
}
