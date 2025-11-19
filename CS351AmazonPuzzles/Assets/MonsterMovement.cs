using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float speed = 2f;
    public int direction = -1;
    public LayerMask groundLayer;
    public float checkDown = 0.6f;
    public float frontOffset = 0.05f;

    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        rb.freezeRotation = true;
        if (sr) sr.flipX = (direction < 0);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        if (!GroundAhead()) Flip();//if there is no ground ahead flip direction
    }

    bool GroundAhead()//Checks if there is ground infront of the enemy to walk on
    {
        var b = col.bounds;
        Vector2 frontFoot = new Vector2(
            direction < 0 ? b.min.x - frontOffset : b.max.x + frontOffset,
            b.min.y + 0.02f
        );

        return Physics2D.Raycast(frontFoot, Vector2.down, checkDown, groundLayer);
    }

    void Flip()//Flips the direction of the enemy
    {
        direction = -direction;
        if (sr) sr.flipX = (direction < 0);
    }
}

