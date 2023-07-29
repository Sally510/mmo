using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Device;

namespace Assets.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private PlayerAnimation anim;
        public FixedJoystick variableJoystick;
        [SerializeField] private float moveSpeed = 3.0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = FindObjectOfType<PlayerAnimation>();
        }

        private void FixedUpdate()
        {
            Vector2 currentPosition = rb.position;
            Vector3 inputVector = Vector2.up * variableJoystick.Vertical + Vector2.right * variableJoystick.Horizontal;
            inputVector = Vector2.ClampMagnitude(inputVector, 1);


            Vector2 movement = moveSpeed * Time.fixedDeltaTime * inputVector;
            Vector2 newPosition = currentPosition + movement;
            //Debug.Log(Time.fixedDeltaTime);

            anim.SetDirection(inputVector);
            rb.MovePosition(newPosition);
        }

        internal static Vector2 ScreenToIso(Vector2 position, Vector2 size)
        {
            float x = position.y / size.y + position.x / size.x;
            float y = position.y / size.y - position.x / size.x;

            return - new Vector2(x, y);
        }
    }
}