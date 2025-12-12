using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainClimber : MonoBehaviour
{
    public LayerMask chainLayer;      // set this to your "Chain" layer
    public float climbSpeed = 4f;
    public float checkRadius = 0.2f;
    public Transform climbCheck;      // small point at player's center or feet

    Rigidbody2D rb;
    float defaultGravity;
    bool isClimbing;
    float verticalInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");

        bool touchingChain = Physics2D.OverlapCircle(climbCheck.position, checkRadius, chainLayer);

        if (touchingChain && Mathf.Abs(verticalInput) > 0.1f)
            isClimbing = true;
        else if (!touchingChain)
            isClimbing = false;
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (climbCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(climbCheck.position, checkRadius);
        }
    }
}
