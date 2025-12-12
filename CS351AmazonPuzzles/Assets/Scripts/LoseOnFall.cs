using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseOnFall : MonoBehaviour
{
    public float lowestY = -10f;
    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position at the beginning
        startPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.y < lowestY)
        {
            // Reset position to starting position
            transform.position = startPosition;
        }
    }
}
