using Assets.Scripts.Client.Models;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyObject
    {
        private readonly MonsterModel _monster;
        private readonly GameObject _gameObject;
        private readonly Rigidbody2D _rigidBody;
        private readonly PlayerAnimation _animation;
        public AutoMovementManager AutoMovement { get; set; }

        public EnemyObject(MonsterModel monster, GameObject gameObject)
        {
            _monster = monster;
            _gameObject = gameObject;
            _rigidBody = gameObject.GetComponent<Rigidbody2D>();
            _animation = gameObject.GetComponentInChildren<PlayerAnimation>();
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
