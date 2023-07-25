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
        private float moveH, moveV;
        [SerializeField] private float moveSpeed = 1.0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 currentPosition = rb.position;

            moveH = Input.GetAxis("Horizontal");
            moveV = Input.GetAxis("Vertical");

            Vector2 inputVector = new(moveH, moveV);
            FindObjectOfType<PlayerAnimation>().SetDirection(inputVector);

            Vector2 movement = inputVector * moveSpeed;
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