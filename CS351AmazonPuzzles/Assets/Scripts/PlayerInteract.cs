using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}
public class PlayerInteract : MonoBehaviour
{
    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    // The thing we’re currently standing next to
    private IInteractable currentInteractable;

    private void Update()
    {
        // When player presses E and something is in range, interact with it
        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Does the thing we bumped into implement IInteractable?
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            // Here you could show "Press E to interact" UI
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable) &&
            currentInteractable == interactable)
        {
            currentInteractable = null;
            // Here you could hide the "Press E" UI
        }
    }
}
