using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{   
    public float angularSpeed = 100f;
    public float horizontalInput;
    public float angle = 270f; //start at bottom

    public float radius = 4f; //radius of circle players moves around

    public Vector2 circleCenter; //centerpoint of circle player moves around

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime; //shorten variable name for brevity
        horizontalInput = GetHorizontalInput();

        //calculate angle
        angle += angularSpeed * horizontalInput * dt;
        angle %= 360f;
        float angleRadians = angle * Mathf.Deg2Rad;

        float x = circleCenter.x + Mathf.Cos(angleRadians) * radius;
        float y = circleCenter.y + Mathf.Sin(angleRadians) * radius;

        transform.position = new Vector2(x, y);

        // Calculate direction toward circle center
        Vector2 directionToCenter = circleCenter - (Vector2)transform.position;

        // Calculate rotation angle
        float rotationAngle = Mathf.Atan2(
            directionToCenter.y,
            directionToCenter.x
        ) * Mathf.Rad2Deg;

        // Rotate sprite so its top points toward the center
        transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle - 90f);
    }

    float GetHorizontalInput (){
        float hInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed)
            {
                hInput = -1f;
            }

            if (Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed)
            {
                hInput = 1f;
            }
        }
        return hInput;
    }
}
