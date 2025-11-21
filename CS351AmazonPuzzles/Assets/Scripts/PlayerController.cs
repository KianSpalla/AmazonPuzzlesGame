using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Player Control Settings
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
    public Vector2 wallJumpForce = new Vector2(5f, 6f);
    public float wallJumpControlLock = 0.18f;
    private float wallJumpTimer;

    // Better jump / gravity tuning
    [Header("Better Jump / Gravity")]
    public float normalGravityScale = 3f;      // gravity while rising & holding jump
    public float lowJumpGravityScale = 5f;     // gravity while rising but jump released
    public float fallGravityScale = 6f;        // gravity while falling
    public float apexThreshold = 0.1f;

    // NEW (velocity cut): shorten jump immediately on release
    [Header("Variable Jump (Velocity Cut)")]
    [Range(0f, 1f)]
    public float jumpCutMultiplier = 0.5f;     // 0.5 = cut upward speed in half when jump is released

    //Audio
    public AudioClip jumpSound;
    private AudioSource playerAudio;

    //Internals
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool onWall;
    private bool wallLeft;
    private bool wallRight;
    private bool isWallSliding;
    private float horizontalInput;
    private bool isSlowed = false;

    // track jump held state for gravity logic
    private bool jumpHeld;

    //Animations
    [Header("Animations")]
    private Animator PlayerAnimator;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    void Start()
    {
        PlayerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (!playerAudio) playerAudio = GetComponent<AudioSource>();
        if (!groundCheck) Debug.LogError("groundCheck not assigned to the player controller on " + name);
        if (!wallCheck) Debug.LogError("wallCheck not assigned to the player controller on " + name);
        if (!rb) Debug.LogError("Rigidbody2D missing on " + name);

        // initialize Rigidbody2D gravityScale from our normal setting
        if (rb != null)
        {
            rb.gravityScale = normalGravityScale;
        }
    }

    private void ProcessWallSlide()
    {
        bool pressingTowardsWall =
            (wallLeft && horizontalInput < 0) ||
            (wallRight && horizontalInput > 0);

        if (!isGrounded && onWall && pressingTowardsWall)
        {
            isWallSliding = true;

            // Clamp downward velocity
            if (rb.velocity.y < -wallSlideMaxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideMaxFallSpeed);
            }
        }
        else
        {
            isWallSliding = false;
        }

        // Update Animator
        PlayerAnimator.SetBool("IsWallSliding", isWallSliding);
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        //Use short left/right raycasts to know which side the wall is on.
        wallLeft = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, WallLayer);
        wallRight = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, WallLayer);
        onWall = (wallLeft || wallRight) && !isGrounded;

        PlayerAnimator.SetBool("IsGrounded", isGrounded);
        PlayerAnimator.SetFloat("YVelocity", rb.velocity.y);

        int dir = 0;
        if (Input.GetKey(leftKey)) dir -= 1;
        if (Input.GetKey(rightKey)) dir += 1;
        horizontalInput = dir;

        // track whether jump is currently held (for gravity logic)
        jumpHeld = Input.GetKey(jumpKey);

        // JUMP PRESS
        if (Input.GetKeyDown(jumpKey))
        {
            if (onWall)
            {
                // Wall Jump
                wallJumpTimer = wallJumpControlLock;
                float away = wallLeft ? 1f : -1f;
                rb.velocity = new Vector2(away * wallJumpForce.x, wallJumpForce.y);

//<<<<<<< Updated upstream

                
                PlayerAnimator.SetTrigger("WallJump");

//=======
                if (playerAudio && jumpSound) playerAudio.PlayOneShot(jumpSound, 0.5f);
                PlayerAnimator.SetTrigger("Jump");

                //Small quality of life: flip to face away from wall immediately
//>>>>>>> Stashed changes
                transform.localScale = new Vector3(away, 1f, 1f);
            }
            else if (coyoteTimeCounter > 0f)
            {
                
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                if (playerAudio && jumpSound) playerAudio.PlayOneShot(jumpSound, 0.5f);
//<<<<<<< Updated upstream

             

//=======
                PlayerAnimator.SetTrigger("Jump");
            }
        }

        // NEW (velocity cut): JUMP RELEASE
        if (Input.GetKeyUp(jumpKey))
        {
            // Only cut while still moving up; don't touch downward motion
            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(
                    rb.velocity.x,
                    rb.velocity.y * jumpCutMultiplier
                );
//>>>>>>> Stashed changes
            }
        }

            //Coyote time handling (ground only)
            if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Wall slide state (handled here for responsive anims; clamped in FixedUpdate)

        ProcessWallSlide();

        Debug.Log("WallLeft: " + wallLeft + " WallRight: " + wallRight + " Input: " + horizontalInput);

        //Decrement control lock timer
        if (wallJumpTimer > 0f) wallJumpTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        //Drag swap
        rb.drag = isGrounded ? groundLinearDrag : airLinearDrag;

        //Target speed (sprint only on ground)
        float speedCap = moveSpeed;
        if (isGrounded && Input.GetKey(sprintKey)) speedCap *= sprintMultiplier;
        float targetX = horizontalInput * Mathf.Min(maxSpeed, speedCap);

        //If we just wall-jumped, briefly ignore steering so the launch is clean, otherwise you can just hug the wall and go up
        if (wallJumpTimer > 0f)
        {
            targetX = rb.velocity.x; //freeze horizontal target during lock
        }

        //Horizontal movement
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

        Debug.Log("Velocity X: " + rb.velocity.x);
        if (isWallSliding)
        {
            //Limit downward speed while sliding on a wall
            if (rb.velocity.y < -wallSlideMaxFallSpeed)
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideMaxFallSpeed);
        }

        // Better jump gravity logic (skip when wall-sliding so your clamp stays in control)
        if (!isWallSliding && !isGrounded)
        {
            float vy = rb.velocity.y;

            // Treat near-zero vertical speed as "top of jump" and already falling
            if (vy <= apexThreshold)
            {
                // Falling or at apex: use strong fall gravity
                rb.gravityScale = fallGravityScale;
            }
            else
            {
                // Clearly going up
                if (jumpHeld)
                {
                    // Holding jump: higher, floatier full jump
                    rb.gravityScale = normalGravityScale;
                }
                else
                {
                    // Released jump while rising: cut jump short
                    rb.gravityScale = lowJumpGravityScale;
                }
            }
        }
        else
        {
            // On ground or wall-sliding: keep gravity stable
            rb.gravityScale = normalGravityScale;
        }


        //Face movement direction (don’t override immediate flip after wall jump)
        if (wallJumpTimer <= 0f)
        {
            if (horizontalInput > 0f) transform.localScale = new Vector3(1f, 1f, 1f);
            else if (horizontalInput < 0f) transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        PlayerAnimator.SetBool("IsGrounded", isGrounded);
        PlayerAnimator.SetBool("IsRunning", horizontalInput != 0f);
        PlayerAnimator.SetFloat("YVelocity", rb.velocity.y);
    }
}
