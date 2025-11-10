using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteract : MonoBehaviour, IActivatable
{
    public Collider2D doorCollider;
    public bool isOpen = false;
    public float openDuration = 3f;
    public void Activate()
    {
        //open door for a certain amount of time
        if(!isOpen)
        {
            StartCoroutine(OpenDoor());
        }
    }
    private IEnumerator OpenDoor()
    {
        isOpen = true;
        doorCollider.enabled = false;
        SetDoorOpacity(0.5f);

        yield return new WaitForSeconds(openDuration);

        isOpen = false;
        doorCollider.enabled = true;
        SetDoorOpacity(1f);
    }
    private void SetDoorOpacity(float alpha)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

}
