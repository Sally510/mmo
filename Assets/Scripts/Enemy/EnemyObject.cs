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
        public Rigidbody2D Rigidbody { get => _rigidBody; }
        public PlayerAnimation Animation { get => _animation; }

        public EnemyObject(MonsterModel monster, GameObject gameObject)
        {
            _monster = monster;
            _gameObject = gameObject;
            _rigidBody = gameObject.GetComponent<Rigidbody2D>();
            _animation = gameObject.GetComponentInChildren<PlayerAnimation>();
        }
    }
}
