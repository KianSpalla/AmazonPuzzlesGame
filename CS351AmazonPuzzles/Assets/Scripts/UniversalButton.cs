using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable
{
    void Activate();
}

public class UniversalButton : MonoBehaviour, IInteractable
{
    public GameObject[] targets;

    public GameObject player1Icon;
    public GameObject player2Icon;

    public GameObject player1IconPressed;
    public GameObject player2IconPressed;

    public KeyCode player1Key = KeyCode.E;
    public KeyCode player2Key = KeyCode.O;

    public void Interact()
    {
        foreach (var target in targets)
        {
            if (target == null) continue;

            var activatables = target.GetComponents<IActivatable>();
            foreach (var activatable in activatables)
            {
                activatable.Activate();
            }
        }
    }

    private void Update()
    {
        // PLAYER 1
        if (Input.GetKeyDown(player1Key) && player1Icon != null && player1Icon.activeSelf)
        {
            Interact();
            player1Icon.SetActive(false);
            if (player1IconPressed != null) player1IconPressed.SetActive(true);
        }

        if (Input.GetKeyUp(player1Key) && player1IconPressed != null && player1IconPressed.activeSelf)
        {
            player1IconPressed.SetActive(false);
            if (player1Icon != null) player1Icon.SetActive(true);
        }

        // PLAYER 2
        if (Input.GetKeyDown(player2Key) && player2Icon != null && player2Icon.activeSelf)
        {
            Interact();
            player2Icon.SetActive(false);
            if (player2IconPressed != null) player2IconPressed.SetActive(true);
        }

        if (Input.GetKeyUp(player2Key) && player2IconPressed != null && player2IconPressed.activeSelf)
        {
            player2IconPressed.SetActive(false);
            if (player2Icon != null) player2Icon.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player1"))
        {
            if (player1Icon != null) player1Icon.SetActive(true);
            if (player1IconPressed != null) player1IconPressed.SetActive(false);
        }
        else if (collision.CompareTag("Player2"))
        {
            if (player2Icon != null) player2Icon.SetActive(true);
            if (player2IconPressed != null) player2IconPressed.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player1"))
        {
            if (player1Icon != null) player1Icon.SetActive(false);
            if (player1IconPressed != null) player1IconPressed.SetActive(false);
        }
        else if (collision.CompareTag("Player2"))
        {
            if (player2Icon != null) player2Icon.SetActive(false);
            if (player2IconPressed != null) player2IconPressed.SetActive(false);
        }
    }
}
