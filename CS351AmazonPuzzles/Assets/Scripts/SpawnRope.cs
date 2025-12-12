using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRope : MonoBehaviour
{
    public GameObject existingRope;

    public GameObject player1Icon;
    public GameObject player1IconPressed;

    public KeyCode player1Key = KeyCode.E;
    public bool onlyOnce = true;

    bool player1InRange;
    bool used;

    void Start()
    {
        if (existingRope) existingRope.SetActive(false);
        if (player1Icon) player1Icon.SetActive(false);
        if (player1IconPressed) player1IconPressed.SetActive(false);
    }

    void Update()
    {
        if (!player1InRange) return;
        if (onlyOnce && used) return;

        if (Input.GetKeyDown(player1Key))
        {
            if (player1Icon) player1Icon.SetActive(false);
            if (player1IconPressed) player1IconPressed.SetActive(true);

            if (existingRope) existingRope.SetActive(true);
            used = true;
        }

        if (Input.GetKeyUp(player1Key))
        {
            if (player1IconPressed) player1IconPressed.SetActive(false);
            if (player1Icon) player1Icon.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player2")) return;

        player1InRange = true;

        if (!used || !onlyOnce)
        {
            if (player1Icon) player1Icon.SetActive(true);
            if (player1IconPressed) player1IconPressed.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player2")) return;

        player1InRange = false;

        if (player1Icon) player1Icon.SetActive(false);
        if (player1IconPressed) player1IconPressed.SetActive(false);
    }
}
