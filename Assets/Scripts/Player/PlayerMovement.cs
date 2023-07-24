using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Vector2 inputVector = new Vector2(moveH, moveV);
        FindObjectOfType<PlayerAnimation>().SetDirection(inputVector);

        Vector2 movement = inputVector * moveSpeed;
        Vector2 newPosition = currentPosition + movement * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}
}