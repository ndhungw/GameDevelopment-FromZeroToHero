using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public float speed = 3.0f;
    public float jumpSpeed = 10.0f;
    public int MaxHealth = 100;
    public LayerMask ground;

    private int currentHealth = 100;

    Rigidbody2D rigidbody2d;
    Animator animator;
    new Collider2D collider;

    public float InvincibleTime = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    bool canJump = true;

    enum State
    {
        IDLE = 0,
        RUNNING = 1,
        JUMPING = 2,
        FALLING = 3,
    }

    State state = State.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        state = State.IDLE;
        currentHealth = MaxHealth;
    }

    // Update is called once per frame
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

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeHealth(-1);
        }
    }

    private void checkVelocityState()
    {
        if (state == State.JUMPING)
        {
            if (rigidbody2d.velocity.y < -0.2f)
            {
                state = State.FALLING;
            }
        }
        else if (state == State.FALLING)
        {
            if (CheckIsGrounded())
            {
                state = State.IDLE;
            }
        }
        else if (rigidbody2d.velocity.y < -20f)
        {
            state = State.FALLING;
        }
        else if (Mathf.Abs(rigidbody2d.velocity.x) > 2f)
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
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);

        Debug.Log(currentHealth);
    }


    /// <summary>
    /// Physics related code down here, logic should be above this part
    /// </summary>
    /// 

    void FixedUpdate()
    {
        if (!canJump)
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                canJump = true;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.localScale = new Vector2(-1, 1);
            rigidbody2d.velocity = new Vector2(-speed, rigidbody2d.velocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.localScale = new Vector2(1, 1);
            rigidbody2d.velocity = new Vector2(speed, rigidbody2d.velocity.y);
        }

        if (!Mathf.Approximately(rigidbody2d.velocity.x, 0.0f) && CheckIsGrounded() && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)
            && (state == State.IDLE || state == State.RUNNING))
        {
            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
            state = State.IDLE;
        }

        if (Input.GetKey(KeyCode.Space) && CheckIsGrounded() && state != State.FALLING && state != State.JUMPING && canJump)
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpSpeed);
            state = State.JUMPING;
            canJump = false;
        }
        checkVelocityState();
        animator.SetInteger("state", (int)state);
        CheckSlope();
    }

    public void CheckSlope()
    {
        // Character is grounded, and no axis pressed: 
        if (CheckIsGrounded() && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && state != State.JUMPING && state != State.FALLING)
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

    bool CheckIsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, collider.bounds.extents.y, ground);

        return hit;
    }
}
