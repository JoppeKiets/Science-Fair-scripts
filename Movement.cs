using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Vector3 playerVelocity;
    public float speed = 10.0F;
    public float rotateSpeed = 10.0F;
    public float FB = 0;
    public float LR = 0;

    public float minX = -30;
    public float maxX = 30;
    public float minY = -20;
    public float maxY = 20;

    private ObjectTracker objectTracker;

    void Awake()
    {
        objectTracker = FindObjectOfType<ObjectTracker>();
    }

    public void Move(float LR, float FB)
    {
        //clamp the values of X and Y       
        FB = Mathf.Clamp(FB, -0.3f, 1);
        LR = Mathf.Clamp(LR, -1, 1);



        // Rotate around z-axis 
        transform.Rotate(0, 0, LR * rotateSpeed * Time.deltaTime);

        // Move forward / backward 
        Vector3 movement = transform.right * FB * speed * Time.deltaTime;

        // Calculate the potential new position
        Vector3 newPosition = transform.position + (Vector3)movement;

        // Clamp the new position within boundaries
        float clampedX = Mathf.Clamp(newPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(newPosition.y, minY, maxY);

        // Update the position if within boundaries
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
