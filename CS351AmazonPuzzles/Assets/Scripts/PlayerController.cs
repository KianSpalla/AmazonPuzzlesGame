using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input (per player)")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public LayerMask WallLayer;
    public Transform wallCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Sprint")]
    public float sprintMultiplier = 1.5f;

    [Header("Feel / Control")]
    public float maxSpeed = 8f;
    public float groundAcceleration = 60f;
    public float airAcceleration = 20f;
    public float groundLinearDrag = 8f;
    public float airLinearDrag = 1f;

    // --- WALL JUMP SETTINGS ---
    [Header("Wall Slide / Jump")]
    public float wallCheckDistance = 0.5f;          // how far to raycast left/right to find a wall
    public float wallSlideMaxFallSpeed = 3.5f;      // how fast you slide down the wall
    public Vector2 wallJumpForce = new Vector2(12f, 14f); // x = push away, y = up
    public float wallJumpControlLock = 0.18f;       // time to ignore horizontal steering after wall jump

    //Audio
    public AudioClip jumpSound;
    public AudioSource playerAudio;

    //Internals
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool onWall;
    private bool wallLeft;
    private bool wallRight;
    private bool isWallSliding;
    private float horizontalInput;
    private bool isSlowed = false;

    private float wallJumpTimer; // counts down while steering is locked

    //Animations
    [Header("Animations")]
    private Animator Animator1;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    void Start()
    {
        Animator1 = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (!playerAudio) playerAudio = GetComponent<AudioSource>();
        if (!groundCheck) Debug.LogError("groundCheck not assigned to the player controller on " + name);
        if (!wallCheck) Debug.LogError("wallCheck not assigned to the player controller on " + name);
        if (!rb) Debug.LogError("Rigidbody2D missing on " + name);
    }

    void Update()
    {
        // --- Ground & Wall checks ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Use short left/right raycasts to know which side the wall is on.
        wallLeft = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, WallLayer);
        wallRight = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, WallLayer);
        onWall = (wallLeft || wallRight) && !isGrounded;

        Animator1.SetBool("IsGrounded", isGrounded);
        Animator1.SetFloat("YVelocity", rb.velocity.y);

        // --- Build horizontal input (-1,0,+1) ---
        int dir = 0;
        if (Input.GetKey(leftKey)) dir -= 1;
        if (Input.GetKey(rightKey)) dir += 1;
        horizontalInput = dir;

        // --- JUMP INPUT ---
        if (Input.GetKeyDown(jumpKey))
        {
            if (onWall)
            {
                // WALL JUMP: push away from the wall
                wallJumpTimer = wallJumpControlLock;
                float away = wallLeft ? 1f : -1f; // jump opposite the wall
                rb.velocity = new Vector2(away * wallJumpForce.x, wallJumpForce.y);

                if (playerAudio && jumpSound) playerAudio.PlayOneShot(jumpSound, 0.5f);
                Animator1.SetTrigger("Jump");

                // small quality-of-life: flip to face away from wall immediately
                transform.localScale = new Vector3(away, 1f, 1f);
            }
            else if (coyoteTimeCounter > 0f)
            {
                // NORMAL JUMP
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                if (playerAudio && jumpSound) playerAudio.PlayOneShot(jumpSound, 0.5f);
                Animator1.SetTrigger("Jump");
            }
        }

        // --- Coyote time handling (ground only) ---
        if (isGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;

        // --- Wall slide state (handled here for responsive anims; clamped in FixedUpdate) ---
        isWallSliding = onWall && rb.velocity.y < 0f;
        Animator1.SetBool("IsWallSliding", isWallSliding);

        // decrement control lock timer
        if (wallJumpTimer > 0f) wallJumpTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // --- Drag swap ---
        rb.drag = isGrounded ? groundLinearDrag : airLinearDrag;

        // --- Target speed (sprint only on ground) ---
        float speedCap = moveSpeed;
        if (isGrounded && Input.GetKey(sprintKey)) speedCap *= sprintMultiplier;
        float targetX = horizontalInput * Mathf.Min(maxSpeed, speedCap);

        // If we just wall-jumped, briefly ignore steering so the launch is clean.
        if (wallJumpTimer > 0f)
        {
            targetX = rb.velocity.x; // freeze horizontal target during lock
        }

        // --- Horizontal movement ---
        if (isGrounded)
        {
            float newX = Mathf.MoveTowards(rb.velocity.x, targetX, groundAcceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newX, rb.velocity.y);
        }
        else
        {
            bool sameDir = Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetX);
            if (!isSlowed && sameDir && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetX))
                targetX = rb.velocity.x;

            float accel = (wallJumpTimer > 0f) ? 0f : airAcceleration;
            float newX = Mathf.MoveTowards(rb.velocity.x, targetX, accel * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newX, rb.velocity.y);
        }

        // --- Wall slide vertical clamp ---
        if (isWallSliding)
        {
            // Limit downward speed while sliding on a wall
            if (rb.velocity.y < -wallSlideMaxFallSpeed)
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideMaxFallSpeed);
        }

        // --- Face movement direction (don’t override immediate flip after wall jump) ---
        if (wallJumpTimer <= 0f)
        {
            if (horizontalInput > 0f) transform.localScale = new Vector3(1f, 1f, 1f);
            else if (horizontalInput < 0f) transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        Animator1.SetBool("IsGrounded", isGrounded);
        Animator1.SetBool("IsRunning", horizontalInput != 0f);
    }

    void OnDrawGizmosSelected()
    {
        // visualize ground and wall checks in-editor
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistance);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistance);
        }
    }
}
