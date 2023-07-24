using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private string[] staticDirections =
    {
        "Static N",
        "Static NW",
        "Static W",
        "Static SW",
        "Static S",
        "Static SE",
        "Static E",
        "Static NE",
    };

    private string[] runDirections =
    {
        "Run N",
        "Run NW",
        "Run W",
        "Run SW",
        "Run S",
        "Run SE",
        "Run E",
        "Run NE",
    };

    int lastDirection;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetDirection(Vector2 direction)
    {
        string[] directionArray = null;
        
        if(direction.magnitude < 0.01)
        {
            directionArray = staticDirections;
        } else
        {
            directionArray = runDirections;
            lastDirection = DirectionToIndex(direction);
        }

        anim.Play(directionArray[lastDirection]);
    }

    private int DirectionToIndex(Vector2 direction)
    {
        Vector2 normalDirection = direction.normalized;
        float step = 360 / 8;
        float offset = step / 2;
        float angle = Vector2.SignedAngle(Vector2.up, normalDirection);

        angle += offset;
        if(angle < 0)
        {
            angle += 360;
        }

        float stepCount = angle / step;
        return Mathf.FloorToInt(stepCount);
    }

}
}