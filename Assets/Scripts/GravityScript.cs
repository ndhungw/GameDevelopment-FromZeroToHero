using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityScript : MonoBehaviour
{
    public LayerMask ground;
    public Transform feet;
    public float RaycastDistanceFromFeet = 2f;

    Rigidbody2D rigidbody2d;
    new Collider2D collider;
    private bool isMoving = false;
    private Action callBackOnMoving;

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

        // Prevent sliding
        if (!Mathf.Approximately(rigidbody2d.velocity.x, 0.0f) && isGrounded  && !isMoving)
        {
            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
            callBackOnMoving?.Invoke();
        }
        CheckSlope();
    }

    public void SetEntityMovingState(bool isMoving, Action callback)
    {
        this.isMoving = isMoving;
        callBackOnMoving = callback;
    }

    public void CheckSlope()
    {
        // Object is grounded
        if (CheckIsGrounded() && !isMoving)
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

    bool CheckIsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, RaycastDistanceFromFeet, ground);
        return hit && collider.IsTouchingLayers(ground);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(feet.position, new Vector3(feet.position.x, feet.position.y - RaycastDistanceFromFeet, 0));
    }
}
