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
        public FixedJoystick variableJoystick;
        [SerializeField] private float moveSpeed = 1.0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 currentPosition = rb.position;
            Vector3 direction = Vector2.up * variableJoystick.Vertical + Vector2.right * variableJoystick.Horizontal;

            FindObjectOfType<PlayerAnimation>().SetDirection(direction);

            Vector2 movement = direction * moveSpeed;
            Vector2 newPosition = currentPosition + movement * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            //Debug.Log($"{newPosition * 100} = {ScreenToIso(newPosition * 100, new(64, 32))}");
        }

        internal static Vector2 ScreenToIso(Vector2 position, Vector2 size)
        {
            float x = position.y / size.y + position.x / size.x;
            float y = position.y / size.y - position.x / size.x;

            return - new Vector2(x, y);
        }
    }
}