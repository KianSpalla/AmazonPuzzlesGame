using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;
    private int i;
    private Vector3 previousPosition;
    private List<Transform> passengersOnPlatform = new List<Transform>();

    void Start()
    {
        transform.position = points[startingPoint].position;
        previousPosition = transform.position;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < .02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        // Store previous position before moving
        previousPosition = transform.position;

        // Move platform
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        // Calculate how much the platform moved
        Vector3 platformMovement = transform.position - previousPosition;

        // Move all passengers by the same amount
        foreach (Transform passenger in passengersOnPlatform)
        {
            if (passenger != null)
            {
                passenger.position += platformMovement;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Add passenger to list if not already there
        if (!passengersOnPlatform.Contains(collision.transform))
        {
            passengersOnPlatform.Add(collision.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Remove passenger from list
        passengersOnPlatform.Remove(collision.transform);
    }
}