using MMO.Game.Handlers;
using System.Collections;
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
            anim = FindObjectOfType<PlayerAnimation>();

            if (State.LoggedCharacter != null)
            {
                Vector2 startPosition = IsoToScreen(State.LoggedCharacter.IsoPosition);
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

                Vector2 directionalVector = moveSpeed * Time.fixedDeltaTime * ToDirectionalVector(ThrottleMovementHandler.Angle.Value);
                Debug.Log($"{directionalVector.x} {directionalVector.y}");
                Vector2 newPosition = currentPosition + directionalVector;
                rb.MovePosition(newPosition);

                Debug.Log(ScreenToIso(newPosition));
            }

            if (ThrottleMovementHandler.PollPacket(inMovement ? Time.fixedDeltaTime : .0f, rb.position, out ThrottleMovementHandler.MovementPacket packet))
            {
                SendMovePacket(packet);
            }
        }

        async void SendMovePacket(ThrottleMovementHandler.MovementPacket packet)
        {
            var response = await Client.ClientManager.SendMovePacketAsync(packet.Angle, packet.ElapsedSeconds, destroyCancellationToken);

        }

        static readonly Vector2 TILE_SIZE = new(0.5f, 0.5f);
        internal static Vector2 ScreenToIso(Vector2 position)
        {
            float x = position.y / TILE_SIZE.y + position.x / TILE_SIZE.x;
            float y = position.y / TILE_SIZE.y - position.x / TILE_SIZE.x;

            return -new Vector2(x, y);
        }

        internal static Vector2 IsoToScreen(Vector2 isoPosition)
        {
            float x = isoPosition.x * TILE_SIZE.x * 0.5f - isoPosition.y * TILE_SIZE.x * 0.5f;
            float y = isoPosition.x * TILE_SIZE.y * 0.5f + isoPosition.y * TILE_SIZE.y * 0.5f;

            return new(x, -y);
        }


        public static Vector2 ToDirectionalVector(float angle)
        {
            return new(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}