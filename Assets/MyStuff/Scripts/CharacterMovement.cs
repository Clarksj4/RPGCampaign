using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Repeat for each step in path:
    // Step 1: Get position to move to
    // Step 2: Rotate towards
    // Step 3: Move there

    public float Speed;
    public Collider floorCollider;

    Vector3 destination;
    bool moving;
    bool rotating;

	// Update is called once per frame
	void Update ()
    {
        // Get position of mouse click on floor
        if (Input.GetMouseButtonDown(0))    // Left click
        {
            // Cast ray from camera through mouse cursor 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Information about the hit
            RaycastHit hit;

            // Check if ray has contacted the floor - don't care about other collders
            if (floorCollider.Raycast(ray, out hit, 100f))
            {
                destination = hit.point;
                moving = true;
                rotating = true;
            }
        }

        if (moving)
        {
            if (rotating)
            {
                Rotate(destination);
                rotating = false;
            }

            Move(destination);
        }
	}

    void Rotate(Vector3 target)
    {
        transform.LookAt(target);
    }

    void Move(Vector3 destination)
    {
        Vector3 facing = destination - transform.position;
        facing.Normalize();
        float distance = Speed * Time.deltaTime;

        float remainingDistance = Vector3.Distance(destination, transform.position);
        if (remainingDistance < distance)
        {
            transform.Translate(facing * remainingDistance, Space.World);
            moving = false;
        }
            
        else
            transform.Translate(facing * distance, Space.World);
    }
}
