using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorInteract : MonoBehaviour, IActivatable
{
    [Header("References")]
    public Animator animator;
    public Collider2D doorCollider;
    [Tooltip("SpriteRenderer for changing opacity. Leave null if you don't want opacity changes.")]
    public Tilemap tilemap; // Changed from TilemapRenderer to Tilemap

    [Header("Behavior")]
    [Tooltip("If true, door will open for a set time and then close by itself.\nIf false, door just toggles open/closed each time.")]
    public bool useTimedMode = true;

    [Tooltip("How long the door stays open in timed mode.")]
    public float openDuration = 3f;

    [Tooltip("Should the door start open? (Controls whether the sequence is open->closed->open or closed->open->closed.)")]
    public bool startOpen = false;

    [Header("Opacity")]
    [Range(0f, 1f)]
    public float closedAlpha = 1f;
    [Range(0f, 1f)]
    public float openAlpha = 0.3f;

    private bool isOpen;
    private Coroutine openRoutine;

    private void Awake()
    {
        // Initialize the door state in Start/Awake
        SetDoorState(startOpen, true);
    }

    public void Activate()
    {
        if (useTimedMode)
        {
            // Timed mode:
            // Every time it's activated, open and then close after openDuration.
            if (openRoutine != null)
            {
                StopCoroutine(openRoutine);
            }
            openRoutine = StartCoroutine(TimedSequence());
        }
        else
        {
            // Toggle mode:
            // Just flip between open and closed each time.
            if (openRoutine != null)
            {
                StopCoroutine(openRoutine);
                openRoutine = null;
            }

            SetDoorState(!isOpen);
        }
    }

    private IEnumerator TimedSequence()
    {
        if (isOpen)
        {
            // If currently open, close then open after timer
            SetDoorState(false);
            yield return new WaitForSeconds(openDuration);
            SetDoorState(true);
        }
        else
        {
            // If currently closed, open then close after timer
            SetDoorState(true);
            yield return new WaitForSeconds(openDuration);
            SetDoorState(false);
        }
        openRoutine = null;
    }

    private void SetDoorState(bool open, bool isInitialSetup = false)
    {
        isOpen = open;

        // Animator
        if (animator != null)
        {
            animator.SetBool("IsOpen", isOpen);
        }

        // Collider: disabled when open so player can walk through
        if (doorCollider != null)
        {
            doorCollider.enabled = !isOpen;
        }

        // Opacity via Tilemap
        if (tilemap != null)
        {
            var color = tilemap.color;
            color.a = isOpen ? openAlpha : closedAlpha;
            tilemap.color = color;
        }

        if (!isInitialSetup)
        {
            Debug.Log("Door " + (isOpen ? "opened" : "closed"));
        }
    }
}
