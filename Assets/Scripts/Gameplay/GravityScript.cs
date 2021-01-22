using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only for entities that is not controlled by hands, use character script for more customizations
public class GravityScript : MonoBehaviour
{
    public LayerMask ground;
    public Transform feet;
    public float RaycastDistanceFromFeet = 2f;

    Rigidbody2D rigidbody2d;
    new Collider2D collider;
    private bool isMoving = false;
    private bool isJumping = false;
    private Action callBackOnMoving;
    private Action callBackOnJumping;

    private void Start()
    {
        if (!feet)
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #endif
                        throw new Exception("Feet need to be assigned to use this script");
        }

        rigidbody2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        bool isGrounded = CheckIsGrounded();

        //Check grounded but we want to get normal vector of the surface the ray is casted into, so we don't use the defined function
        RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, RaycastDistanceFromFeet, ground);

        if (rigidbody2d.velocity.x < 0 && isMoving)
        {
            transform.localScale = new Vector2(-1, 1);
            //Movement on ground
            if (hit && !isJumping)
            {
                Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                rigidbody2d.velocity = (-Mathf.Abs(rigidbody2d.velocity.x)) * moveVect;
            }
        }
        else if (rigidbody2d.velocity.x > 0 && isMoving)
        {
            transform.localScale = new Vector2(1, 1);
            //Movement on ground
            if (hit && !isJumping)
            {
                Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                rigidbody2d.velocity = Mathf.Abs(rigidbody2d.velocity.x) * moveVect;
            }
        }

        // Prevent sliding
        if (!Mathf.Approximately(rigidbody2d.velocity.x, 0.0f) && isGrounded && !isMoving)
        {
            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
        }

        // Callbacks from user
        if (isMoving)
        {
            callBackOnMoving?.Invoke();
        }

        if (isJumping)
        {
            callBackOnJumping?.Invoke();
        }

        CheckSlope();
    }

    public void SetEntityMovingState(bool isMoving, Action callback)
    {
        this.isMoving = isMoving;
        callBackOnMoving = callback;
    }

    public void SetEntityJumpingState(bool isJumping, Action callback)
    {
        this.isJumping = isJumping;
        callBackOnJumping = callback;
    }

    public void CheckSlope()
    {
        // Object is grounded
        if (CheckIsGrounded() && !isMoving && !isJumping)
        {
            RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, RaycastDistanceFromFeet, ground);

            // Check if there is slope
            if (hit && Mathf.Abs(hit.normal.x) > 0.1f)
            {
                // We freeze position X of the rigidbody constraints and put x velocity to 0
                // to stop the normal physics on slope, making object only movable from us setting velocity
                rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionX;
                rigidbody2d.freezeRotation = true;
                rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
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
        RaycastHit2D hit = Physics2D.CircleCast(feet.position, RaycastDistanceFromFeet / 2, Vector2.down, 0.0f, ground);
        return hit && collider.IsTouchingLayers(ground);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(feet.position, RaycastDistanceFromFeet / 2);
    }
}
