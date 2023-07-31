using Assets.Scripts.Helpers;
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
                Vector2 startPosition = PositionHelpers.IsoToScreen(State.LoggedCharacter.IsoPosition);
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
                Debug.Log($"{directionalVector.x} {directionalVector.y}");
                Vector2 newPosition = currentPosition + directionalVector;
                rb.MovePosition(newPosition);

                Debug.Log(PositionHelpers.ScreenToIso(newPosition));
            }

            if (ThrottleMovementHandler.PollPacket(inMovement ? Time.fixedDeltaTime : .0f, rb.position, out ThrottleMovementHandler.MovementPacket packet))
            {
                //SendMovePacket(packet);
            }
        }

        async void SendMovePacket(ThrottleMovementHandler.MovementPacket packet)
        {
            var response = await Client.ClientManager.SendMovePacketAsync(packet.Angle, packet.ElapsedSeconds, destroyCancellationToken);

        }
    }
}