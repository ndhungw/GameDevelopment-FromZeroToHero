using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float movementSpeed;
    public LayerMask ground;


    private new Collider2D collider;
    private float speed;
    Rigidbody2D rigidbody2d;

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        speed = movementSpeed;
    }

    public void CheckSlope()
    {
        // Object is grounded
        if (CheckIsGrounded())
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (float)(0.5 * rigidbody2d.gravityScale + speed), ground);

            // Check if we are on the slope
            if (hit && Mathf.Abs(hit.normal.x) > 0.1f && collider.IsTouchingLayers(ground))
            {
                // We freeze all the rigidbody constraints and put velocity to 0
                rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
                rigidbody2d.velocity = Vector2.zero;
            }
        }
        else
        {
            // if we are on air or moving - jumping, unfreeze all and freeze only rotation.
            rigidbody2d.constraints = RigidbodyConstraints2D.None;
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public bool CheckIsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, collider.bounds.extents.y, ground);

        return hit;
    }
}
