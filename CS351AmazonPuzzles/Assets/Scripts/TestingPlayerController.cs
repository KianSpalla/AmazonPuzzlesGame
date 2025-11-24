using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPlayerController : MonoBehaviour
{
    [Header("Input (per player)")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public float moveSpeed = 5f;

    public float jumpForce = 10f;

    public LayerMask groundLayer;

    public Transform groundCheck;

    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;

    private bool isGrounded;

    private float horizontalInput;

    public AudioClip jumpSound;
    private AudioSource playerAudio;

    private Animator animator;

    public LayerMask wallLayer;
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public float wallJumpForce = 10f;
    private bool onWall;

    private float defaultGravityScale;

    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        //Get the Rigidbody2D component attached to the gameObject
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        defaultGravityScale = rb.gravityScale;

        //Ensure the groundCheck variable is assigned
        if (groundCheck == null)
        {
            Debug.LogError("groundCheck not assigned to the player controller");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Get input values for horizontal movement using custom keys
        horizontalInput = 0f;
        if (Input.GetKey(leftKey)) horizontalInput -= 1f;
        if (Input.GetKey(rightKey)) horizontalInput += 1f;

        //Check for jump input
        if (Input.GetKeyDown(jumpKey) && (isGrounded || onWall))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            playerAudio.PlayOneShot(jumpSound, 0.5f);
        }

        // Variable jump: increase gravity when jump is released while going up
        if (Input.GetKeyUp(jumpKey) && rb.velocity.y > 0f)
        {
            rb.gravityScale = defaultGravityScale * 2f;
        }
    }

    void FixedUpdate()
    {
        //Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        onWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);

        // Reset gravity when on the ground
        if (isGrounded)
        {
            rb.gravityScale = defaultGravityScale;
        }

        //When Sprint key is held, increase move speed
        if (Input.GetKey(sprintKey))
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed * 1.5f, rb.velocity.y);
        }
        else
        {

            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }

        //TODO: Optionally we can add animations here later
        animator.SetFloat("xVelocityAbs", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetBool("onGround", isGrounded);

        //Ensure the player is facing the direction of the movement
        if (horizontalInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (horizontalInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

}
