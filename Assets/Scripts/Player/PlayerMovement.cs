using MMO.Game.Handlers;
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
            fixedJoystick.SnapX = true;
            fixedJoystick.SnapY = true;

            ThrottleMovementHandler.Restart();

            rb = GetComponent<Rigidbody2D>();
            anim = FindObjectOfType<PlayerAnimation>();

            if(State.LoggedCharacter != null)
            {
                Vector2 startPosition = IsoToScreen(State.LoggedCharacter.IsoPosition);
                Debug.Log($"{State.LoggedCharacter.IsoPosition} = {startPosition}");
                rb.position = startPosition;
            }
        }

        private void FixedUpdate()
        {
            
            Vector2 currentPosition = rb.position;
            Vector2 inputVector = Vector2.up * fixedJoystick.Vertical + Vector2.right * fixedJoystick.Horizontal;
            anim.SetDirection(inputVector);

            //Debug.Log($"{fixedJoystick.Vertical} - {fixedJoystick.Horizontal}");
            //inputVector = Vector2.ClampMagnitude(inputVector, 1);

            if (inputVector != Vector2.zero)
            {
                Vector2 movement = moveSpeed * Time.fixedDeltaTime * inputVector;
                Vector2 newPosition = currentPosition + movement;
                rb.MovePosition(newPosition);

                if (!ThrottleMovementHandler.IsInMovement)
                {
                    ThrottleMovementHandler.Start(Angle(inputVector));
                }
            } else
            {
                ThrottleMovementHandler.Stop();
            }

            if (ThrottleMovementHandler.PollPacket(Time.fixedDeltaTime, rb.position, out ThrottleMovementHandler.MovementPacket packet))
            {
                //Debug.Log($"{packet.PredictedIsoPosition} - {packet.ElapsedMillis}");
            }
        }

        static readonly Vector2 TILE_SIZE = new(0.5f, 0.5f);
        internal static Vector2 ScreenToIso(Vector2 position)
        {
            float x = position.y / TILE_SIZE.y + position.x / TILE_SIZE.x;
            float y = position.y / TILE_SIZE.y - position.x / TILE_SIZE.x;

            return - new Vector2(x, y);
        }

        internal static Vector2 IsoToScreen(Vector2 isoPosition)
        {
            float x = isoPosition.x * TILE_SIZE.x * 0.5f - isoPosition.y * TILE_SIZE.x * 0.5f;
            float y = isoPosition.x * TILE_SIZE.y * 0.5f + isoPosition.y * TILE_SIZE.y * 0.5f;
            
            return new(x, -y);
        }

        public static float Angle(Vector2 vector2)
        {
            if (vector2.x < 0)
            {
                return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * -1);
            }
            else
            {
                return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
            }
        }
    }
}