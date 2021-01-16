using System;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public float speed = 3.0f;
    public float jumpSpeed = 10.0f;
    public int MaxHealth = 100;
    public LayerMask ground;
    public Transform feet;
    //How many screen unit to cast the ray
    public float RaycastDistanceFromFeet = 2f;

    private int currentHealth = 100;

    Rigidbody2D rigidbody2d;
    Animator animator;
    new Collider2D collider;

    public float InvincibleTime = 2.0f;
    bool isInvincible = true;
    float invincibleTimer;

    private float staggerTime;
    bool isStaggered = false;
    private float staggerTimer;

    bool canJump = true;

    public enum State
    {
        IDLE = 0,
        RUNNING = 1,
        JUMPING = 2,
        FALLING = 3,
    }

    public State state = State.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        if (!feet)
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #endif
                        throw new Exception("Feet need to be assigned to use this script");
        }

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        state = State.IDLE;
        currentHealth = MaxHealth;
        staggerTime = Mathf.Max(InvincibleTime / 4, 0.5f);
    }

    private void checkVelocityState()
    {
        bool isGrounded = CheckIsGrounded();

        if (state == State.JUMPING)
        {
            if (!isGrounded && rigidbody2d.velocity.y < -0.1f)
            {
                state = State.FALLING;
            }
        }
        else if (state == State.FALLING)
        {
            if (isGrounded)
            {
                state = State.IDLE;
            }
        }
        else if (!isGrounded && rigidbody2d.velocity.y < -speed)
        {
            state = State.FALLING;
        }
        else if (Mathf.Abs(rigidbody2d.velocity.x) > 1f)
        {
            state = State.RUNNING;
        }
        else
        {
            state = State.IDLE;
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            animator.SetTrigger("hit");

            isInvincible = true;
            invincibleTimer = InvincibleTime;
            isStaggered = true;
            staggerTimer = staggerTime;
            
            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
    }

    // One unit = 8 pixels
    public void Knockback(float knockbackInUnit)
    {
        rigidbody2d.velocity = new Vector2(knockbackInUnit, Mathf.Abs(knockbackInUnit) * (jumpSpeed /speed) / Mathf.Tan(30) );
    }

    /// <summary>
    /// Physics related code down here, logic should be above this part
    /// </summary>
    /// 

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (isStaggered)
        {
            staggerTimer -= Time.deltaTime;
            if(staggerTimer < 0)
            {
                isStaggered = false;
            }
        } 

        // we skip input checking + input validation if character is staggering
        if (!isStaggered)
        {
            if (!canJump)
            {
                if (!Input.GetKey(KeyCode.Space))
                {
                    canJump = true;
                }
            }

            bool isGrounded = CheckIsGrounded();

            //Check grounded but we want to get normal vector of the surface the ray is casted into, so we don't use the defined function
            RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, RaycastDistanceFromFeet, ground);

            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector2(-1, 1);
                // Onground - our rule
                if (hit && state != State.JUMPING && state != State.FALLING)
                {
                    Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                    rigidbody2d.velocity = (-speed) * moveVect;
                } 
                // Mid-air - engine's rule
                else
                {
                    rigidbody2d.velocity = new Vector2(-speed, rigidbody2d.velocity.y);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector2(1, 1);
                // Onground - our rule
                if (hit && state != State.JUMPING && state != State.FALLING)
                {
                   Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                   rigidbody2d.velocity = speed * moveVect;
                }
                // Mid-air - engine's rule
                else
                {
                    rigidbody2d.velocity = new Vector2(speed, rigidbody2d.velocity.y);
                }     
            } 

            // Stop sliding on X after apply velocity from button and dash
            if (!Mathf.Approximately(rigidbody2d.velocity.x, 0.0f) && isGrounded && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)
                && (state == State.IDLE || state == State.RUNNING))
            {
                rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionX;
                rigidbody2d.freezeRotation = true;
                rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
                rigidbody2d.constraints = RigidbodyConstraints2D.None;
                rigidbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
                state = State.IDLE;
            }

            // Jump input
            if (Input.GetKey(KeyCode.Space) && isGrounded && canJump)
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpSpeed);
                state = State.JUMPING;
                canJump = false;
            }

            CheckSlope(Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D));

            checkVelocityState();
            animator.SetInteger("state", (int)state);
        }
    }

    public void CheckSlope(bool isAPressed, bool isDPressed)
    {
        // Character is grounded, and no axis pressed: 
        if (CheckIsGrounded() && !isAPressed && !isDPressed && state != State.JUMPING && state != State.FALLING)
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
