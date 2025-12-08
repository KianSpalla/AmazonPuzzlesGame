using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2SuperJump : MonoBehaviour
{
    [Header("Auto-found if left empty")]
    public PlayerController player1;
    public PlayerController player2;

    private float p1LastJumpTime = -999f;
    private float p2LastJumpTime = -999f;

    [Header("Timing Assist")]
    public float stackJumpWindow = 0.15f; // seconds (try 0.12 - 0.2)
    public float jumpBuffer = 0.15f;      // optional extra forgiveness


    [Header("Tuning")]
    [Range(0.1f, 1f)]
    public float player1JumpMultiplier = 0.5f; // half jump
    public float player2ExtraBoost = 6f;       // added to player2 jump

    private Rigidbody2D rb1;
    private Rigidbody2D rb2;

    private bool player2StandingOnPlayer1;
    private bool doStackJumpThisFrame;

    private void Awake()
    {
        // player2 is the parent of this StandCheck
        if (player2 == null)
            player2 = GetComponentInParent<PlayerController>();

        // find player1 by tag
        if (player1 == null)
        {
            GameObject p1Obj = GameObject.FindGameObjectWithTag("Player1");
            if (p1Obj) player1 = p1Obj.GetComponent<PlayerController>();
        }

        if (player1) rb1 = player1.GetComponent<Rigidbody2D>();
        if (player2) rb2 = player2.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        doStackJumpThisFrame = false;

        if (player1 == null || player2 == null || rb1 == null || rb2 == null)
            return;

        // Track jump presses with timestamps
        if (Input.GetKeyDown(player1.jumpKey))
            p1LastJumpTime = Time.time;

        if (Input.GetKeyDown(player2.jumpKey))
            p2LastJumpTime = Time.time;

        // Only attempt stack jump when Player2 is on Player1
        if (!player2StandingOnPlayer1) return;

        // If both jumps happened within the window, trigger
        float dt = Mathf.Abs(p1LastJumpTime - p2LastJumpTime);

        // You can also require "recentness" so old presses don't count
        bool p1Recent = (Time.time - p1LastJumpTime) <= jumpBuffer;
        bool p2Recent = (Time.time - p2LastJumpTime) <= jumpBuffer;

        if (dt <= stackJumpWindow && p1Recent && p2Recent)
        {
            doStackJumpThisFrame = true;

            // Clear times so one press doesn't trigger multiple frames
            p1LastJumpTime = -999f;
            p2LastJumpTime = -999f;
        }
    }


    private void LateUpdate()
    {
        if (!doStackJumpThisFrame) return;

        // Override vertical velocities AFTER PlayerController runs
        float p1V = player1.jumpForce * player1JumpMultiplier;
        float p2V = player2.jumpForce + player2ExtraBoost;

        rb1.velocity = new Vector2(rb1.velocity.x, p1V);
        rb2.velocity = new Vector2(rb2.velocity.x, p2V);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
            player2StandingOnPlayer1 = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
            player2StandingOnPlayer1 = false;
    }
}
