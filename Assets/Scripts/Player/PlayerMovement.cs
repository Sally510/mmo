using Assets.Scripts.Client;
using Assets.Scripts.Enemy;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private PlayerAnimation anim;
        public FixedJoystick fixedJoystick;
        [SerializeField] private float moveSpeed = 3.0f;

        private void Awake()
        {
            ThrottleMovementHandler.Restart();

            rb = GetComponent<Rigidbody2D>();
            anim = GetComponentInChildren<PlayerAnimation>();

            if (State.LoggedCharacter != null)
            {
                Vector2 startPosition = PositionHelpers.IsoToWorld(State.LoggedCharacter.IsoPosition);
                Debug.Log($"Spawning character at: {State.LoggedCharacter.IsoPosition}");
                rb.position = startPosition;
            }
        }

        private void FixedUpdate()
        {
            //inputVector = Vector2.ClampMagnitude(inputVector, 1);

            Vector2 currentPosition = rb.position;
            Vector2 inputVector = (Vector2.up * fixedJoystick.Vertical + Vector2.right * fixedJoystick.Horizontal).normalized;
            anim.SetDirection(inputVector);

            bool inMovement = inputVector != Vector2.zero;
            if (!inMovement)
            {
                ThrottleMovementHandler.Stop();
            }
            else
            {
                if (!ThrottleMovementHandler.IsInMovement)
                {
                    ThrottleMovementHandler.Start(Mathf.Atan2(inputVector.y, inputVector.x));
                }

                //Debug.Log($"{ToDirectionalVector(ThrottleMovementHandler.Angle.Value)} - {inputVector}");

                Vector2 directionalVector = moveSpeed * Time.fixedDeltaTime * PositionHelpers.ToDirectionalVector(ThrottleMovementHandler.Angle.Value);
                //Debug.Log($"{directionalVector.x} {directionalVector.y}");
                Vector2 newPosition = currentPosition + directionalVector;
                rb.MovePosition(newPosition);

                //Debug.Log(PositionHelpers.WorldToIso(newPosition));
            }

            if (ThrottleMovementHandler.PollPacket(inMovement ? Time.fixedDeltaTime : .0f, rb.position, out ThrottleMovementHandler.MovementPacket packet))
            {
                SendMovePacket(packet);
            }
        }

        void Update()
        {
            Vector3? worldPos = null;
            //foreach (Touch touch in Input.touches)
            //{
            //    if(touch.phase == TouchPhase.Ended)
            //    {
            //        worldPos = Camera.main.ScreenToWorldPoint(touch.position);
            //        break;
            //    }
            //}
            if (Input.GetButtonDown("Fire1"))
            {
                worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (worldPos.HasValue)
            {
                var collider = Physics2D.OverlapPoint(worldPos.Value);
                if(collider != null)
                {
                    if(collider.TryGetComponent(out EnemyObject enemy))
                    {
                        AttackEnemy(enemy.Monster.EntityId);
                    }
                }
            }
        }

        async void SendMovePacket(ThrottleMovementHandler.MovementPacket packet)
        {
            var response = await ClientManager.SendMovePacketAsync(packet.Angle, packet.ElapsedSeconds, destroyCancellationToken);
            //TODO: handle conflicts
        }

        async void AttackEnemy(uint entityId)
        {
            Debug.Log($"Attacking enemy {entityId}");
            await ClientManager.AttackEnemy(entityId, destroyCancellationToken);
        }
    }
}