using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorInteract : MonoBehaviour, IActivatable
{
    public Collider2D doorCollider;
    public bool isOpen = false;
    public float openDuration = 3f;
    public AudioClip doorSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!isOpen)
        {
            SetDoorOpacity(1f);
            doorCollider.enabled = true;
        }
        else
        {
            SetDoorOpacity(0.5f);
            doorCollider.enabled = false;
        }
    }
    public void Activate()
    {
        //open door for a certain amount of time
        //play audio clip
        audioSource.PlayOneShot(doorSound);
        StartCoroutine(ChangeDoor());
    }
    private IEnumerator ChangeDoor()
    {
        if (!isOpen) { 
        isOpen = true;
        doorCollider.enabled = false;
        SetDoorOpacity(0.5f);

        yield return new WaitForSeconds(openDuration);

        isOpen = false;
        doorCollider.enabled = true;
        SetDoorOpacity(1f);
        }
        else
        {
            isOpen = false;
            doorCollider.enabled = true;
            SetDoorOpacity(1f);

            yield return new WaitForSeconds(openDuration);

            isOpen = true;
            doorCollider.enabled = false;
            SetDoorOpacity(0.5f);
        }
    }
    private void SetDoorOpacity(float alpha)
    {
        var tilemap = GetComponent<Tilemap>();
        if (tilemap != null)
        {
            var color = tilemap.color;
            color.a = alpha;
            tilemap.color = color;
        }
    }

}
