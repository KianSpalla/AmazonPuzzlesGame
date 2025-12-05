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

    [Header("Wall Slide / Jump")]
    public float wallCheckDistance = 0.5f;
    public float wallSlideMaxFallSpeed = 3.5f;
    public Vector2 wallJumpForce = new Vector2(12f, 14f);
    public float wallJumpControlLock = 0.18f;

    [Header("Better Jump / Gravity")]
    public float normalGravityScale = 3f;
    public float lowJumpGravityScale = 5f;
    public float fallGravityScale = 6f;
    public float apexThreshold = 0.1f;

    [Header("Variable Jump (Velocity Cut)")]
    [Range(0f, 1f)]
    public float jumpCutMultiplier = 0.5f;

    [Header("Double Jump")]
    public int maxJumps = 1;
    private int jumpsRemaining;
    public float doubleJumpForce = 9f;



    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioSource playerAudio;

    // Internals
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool onWall;
    private bool wallLeft;
    private bool wallRight;
    private bool isWallSliding;
    private float horizontalInput;
    private bool isSlowed = false;
    private float wallJumpTimer;
    private bool jumpHeld;

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
        if (!groundCheck) Debug.LogError("groundCheck not assigned on " + name);
        if (!wallCheck) Debug.LogError("wallCheck not assigned on " + name);
        if (!rb) Debug.LogError("Rigidbody2D missing on " + name);
        jumpsRemaining = maxJumps;

    }

    private IEnumerator FlipAfterWallJump(float away)
    {
        yield return null;
        transform.localScale = new Vector3(away, 1f, 1f);
    }

    void PerformJump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
        if (playerAudio && jumpSound) playerAudio.PlayOneShot(jumpSound, 0.5f);
        Animator1.SetTrigger("Jump");
    }

    void Update()
    {
        // --- Ground & Wall checks ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        wallLeft = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, WallLayer);
        wallRight = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, WallLayer);
        onWall = (wallLeft || wallRight) && !isGrounded;

        Animator1.SetBool("IsGrounded", isGrounded);
        Animator1.SetFloat("YVelocity", rb.velocity.y);

        // --- Input ---
        int dir = 0;
        if (Input.GetKey(leftKey)) dir -= 1;
        if (Input.GetKey(rightKey)) dir += 1;
        horizontalInput = dir;
        jumpHeld = Input.GetKey(jumpKey);

        // --- Jump Press ---
        // --- Jump Press ---
        if (Input.GetKeyDown(jumpKey))
        {
            if (onWall)
            {
                DoWallJump();
                // Optional: after a wall jump, allow one extra jump in air
                jumpsRemaining = Mathf.Max(jumpsRemaining, maxJumps - 1);
            }
            else
            {
                // Ground / coyote jump
                if (coyoteTimeCounter > 0f)
                {
                    PerformJump(jumpForce);
                    coyoteTimeCounter = 0f;
                    jumpsRemaining = maxJumps - 1; // you already used the first jump
                }
                // Air jump (double jump)
                else if (jumpsRemaining > 0)
                {
                    PerformJump(doubleJumpForce);
                    jumpsRemaining--;
                }
            }
        }


        // --- Jump Release ---
        if (Input.GetKeyUp(jumpKey) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
        }

        // --- Coyote time ---
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpsRemaining = maxJumps;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }


        // --- Wall slide state ---
        isWallSliding = onWall && rb.velocity.y < 0f;
        Animator1.SetBool("IsWallSliding", isWallSliding);

        // --- Face direction ---
        if (wallJumpTimer <= 0f)
        {
            if (horizontalInput > 0f) transform.localScale = new Vector3(1f, 1f, 1f);
            else if (horizontalInput < 0f) transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        Animator1.SetBool("IsRunning", horizontalInput != 0f);
    }

    void FixedUpdate()
    {
        rb.drag = isGrounded ? groundLinearDrag : airLinearDrag;

        float speedCap = moveSpeed;
        if (isGrounded && Input.GetKey(sprintKey)) speedCap *= sprintMultiplier;
        float targetX = horizontalInput * Mathf.Min(maxSpeed, speedCap);

        if (wallJumpTimer > 0f) targetX = rb.velocity.x;

        if (isGrounded)
        {
            float newX = Mathf.MoveTowards(rb.velocity.x, targetX, groundAcceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newX, rb.velocity.y);
        }
        else
        {
            float accel = (wallJumpTimer > 0f) ? 0f : airAcceleration;
            float newX = Mathf.MoveTowards(rb.velocity.x, targetX, accel * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newX, rb.velocity.y);
        }

        // --- Wall slide clamp ---
        if (isWallSliding && rb.velocity.y < -wallSlideMaxFallSpeed)
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideMaxFallSpeed);

        // --- Gravity logic ---
        if (!isWallSliding && !isGrounded)
        {
            float vy = rb.velocity.y;
            if (vy <= apexThreshold)
                rb.gravityScale = fallGravityScale;
            else
                rb.gravityScale = jumpHeld ? normalGravityScale : lowJumpGravityScale;
        }
        else
        {
            rb.gravityScale = normalGravityScale;
        }

        if (wallJumpTimer > 0f) wallJumpTimer -= Time.deltaTime;
    }

    void DoWallJump()
    {
        wallJumpTimer = wallJumpControlLock;
        float facing = transform.localScale.x;
        float away = -facing;
        rb.velocity = new Vector2(away * wallJumpForce.x, wallJumpForce.y);
        if (playerAudio && jumpSound) playerAudio.PlayOneShot(jumpSound, 0.5f);
        Animator1.SetTrigger("Jump");
        StartCoroutine(FlipAfterWallJump(away));
    }

    void OnDrawGizmosSelected()
    {
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
