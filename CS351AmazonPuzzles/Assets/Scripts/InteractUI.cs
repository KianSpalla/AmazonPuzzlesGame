using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [Header("Icon References")]
    public GameObject player1Icon;
    public GameObject player2Icon;
    public GameObject player1IconPressed;
    public GameObject player2IconPressed;
    public GameObject LeverNeutral;
    public GameObject LeverNeutralPressed;

    [Header("Key Bindings")]
    public KeyCode player1Key = KeyCode.E;
    public KeyCode player2Key = KeyCode.RightControl;

    private bool player1InRange = false;
    private bool player2InRange = false;

    private void Update()
    {
        // Player 1 icon switching
        if (player1InRange)
        {
            if (Input.GetKeyDown(player1Key))
            {
                if (player1Icon != null) player1Icon.SetActive(false);
                if (player1IconPressed != null) player1IconPressed.SetActive(true);
            }
            if (Input.GetKeyUp(player1Key))
            {
                if (player1IconPressed != null) player1IconPressed.SetActive(false);
                if (player1Icon != null) player1Icon.SetActive(true);
            }
        }

        // Player 2 icon switching
        if (player2InRange)
        {
            if (Input.GetKeyDown(player2Key))
            {
                if (player2Icon != null) player2Icon.SetActive(false);
                if (player2IconPressed != null) player2IconPressed.SetActive(true);
            }
            if (Input.GetKeyUp(player2Key))
            {
                if (player2IconPressed != null) player2IconPressed.SetActive(false);
                if (player2Icon != null) player2Icon.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player1"))
        {
            player1InRange = true;
            if (player1Icon != null) player1Icon.SetActive(true);
            if (player1IconPressed != null) player1IconPressed.SetActive(false);
        }
        else if (collision.CompareTag("Player2"))
        {
            player2InRange = true;
            if (player2Icon != null) player2Icon.SetActive(true);
            if (player2IconPressed != null) player2IconPressed.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player1"))
        {
            player1InRange = false;
            if (player1Icon != null) player1Icon.SetActive(false);
            if (player1IconPressed != null) player1IconPressed.SetActive(false);
        }
        else if (collision.CompareTag("Player2"))
        {
            player2InRange = false;
            if (player2Icon != null) player2Icon.SetActive(false);
            if (player2IconPressed != null) player2IconPressed.SetActive(false);
        }
    }
}
