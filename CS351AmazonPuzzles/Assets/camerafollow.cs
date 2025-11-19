using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerafollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The player transform to follow")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("Offset from the target position")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    [Tooltip("How smoothly the camera follows (lower = smoother)")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    [Header("Optional: Look At Target")]
    [Tooltip("Enable if you want camera to always look at the target")]
    public bool lookAtTarget = true;

    [Tooltip("Offset for the look-at point")]
    public Vector3 lookAtOffset = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera has no target assigned!");
            return;
        }

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between current position and desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update camera position
        transform.position = smoothedPosition;

        // Optionally make camera look at the target
        if (lookAtTarget)
        {
            transform.LookAt(target.position + lookAtOffset);
        }
    }
}