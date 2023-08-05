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

        public AutoMovementManager AutoMovement { get; set; }

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
            bool inMovement = false;

            if (AutoMovement != null)
            {
                if (AutoMovement.IsDone)
                {
                    AutoMovement = null;
                    anim.SetDirection(Vector2.zero);
                }
                else
                {
                    AutoMovement.Update(Time.fixedDeltaTime);
                    Vector2 worldPos = AutoMovement.CurrentIsoPosition().FromIsoToWorld();

                    Vector2 oldPos = rb.position;
                    rb.MovePosition(worldPos);
                    anim.SetDirection((worldPos - oldPos).normalized);
                }
            } else
            {

                Vector2 currentPosition = rb.position;
                Vector2 inputVector = (Vector2.up * fixedJoystick.Vertical + Vector2.right * fixedJoystick.Horizontal).normalized;
                anim.SetDirection(inputVector);

                inMovement = inputVector != Vector2.zero;
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

                    Vector2 directionalVector = moveSpeed * Time.fixedDeltaTime * PositionHelpers.ToDirectionalVector(ThrottleMovementHandler.Angle.Value);
                    Vector2 newPosition = currentPosition + directionalVector;
                    rb.MovePosition(newPosition);
                }
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



        private void PacketEventHandler_AutoWalkEvent(object sender, Client.Models.AutoWalkModel e)
        {
            if(e.EntityId == State.LoggedCharacter.EntityId)
            {
                AutoMovement = new AutoMovementManager(e.StartTime, e.WalkDuration, e.Moves);
                ThrottleMovementHandler.Restart();
            }
        }

        async void SendMovePacket(ThrottleMovementHandler.MovementPacket packet)
        {
            var response = await ClientManager.SendMovePacketAsync(packet.Angle, packet.ElapsedSeconds, destroyCancellationToken);
            Vector3 worldPosition = response.IsoPosition.FromIsoToWorld();
            if(PositionHelpers.Approximately((Vector2)worldPosition, packet.PredictedIsoPosition))
            {
                rb.MovePosition(worldPosition);
            }
        }

        async void AttackEnemy(uint entityId)
        {
            Debug.Log($"Attacking enemy {entityId}");
            await ClientManager.AttackEnemy(entityId, destroyCancellationToken);
        }

        private void OnEnable()
        {
            PacketEventHandler.AutoWalkEvent += PacketEventHandler_AutoWalkEvent;
        }

        private void OnDisable()
        {
            PacketEventHandler.AutoWalkEvent -= PacketEventHandler_AutoWalkEvent;
        }
    }
}