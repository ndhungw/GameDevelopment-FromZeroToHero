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
    bool isInvincible = true;
    float invincibleTimer;

    private float staggerTime;
    bool isStaggered = false;
    private float staggerTimer;

    private bool isAPressedThisUpdate = false;
    private bool isDPressedThisUpdate = false;
    private bool isSpacePressedThisUpdate = false;

    bool canJump = true;

    enum State
    {
        IDLE = 0,
        RUNNING = 1,
        JUMPING = 2,
        FALLING = 3,
    }

    State state = State.IDLE;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        state = State.IDLE;
        currentHealth = MaxHealth;
        staggerTime = Mathf.Max(InvincibleTime / 4, 1f);
        staggerTimer = staggerTime;       
    }

    // Update is called once per frame
    void Update()
    {
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
            isStaggered = true;
            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
    }


    /// <summary>
    /// Physics related code down here, logic should be above this part
    /// </summary>
    /// 

    void FixedUpdate()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.fixedDeltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (isStaggered)
        {
            staggerTimer -= Time.fixedDeltaTime;
            if(staggerTimer < 0)
            {
                isStaggered = false;
                staggerTimer = staggerTime;
            }
        } 

        // we skip input checking + input validation if character is staggering
        if (!isStaggered)
        {
            isAPressedThisUpdate = Input.GetKey(KeyCode.A);
            isDPressedThisUpdate = Input.GetKey(KeyCode.D);
            isSpacePressedThisUpdate = Input.GetKey(KeyCode.Space);

            if (!canJump)
            {
                if (!isSpacePressedThisUpdate)
                {
                    canJump = true;
                }
            }

            bool isGrounded = CheckIsGrounded();

            if (!Mathf.Approximately(rigidbody2d.velocity.x, 0.0f) && isGrounded && !isAPressedThisUpdate && !isDPressedThisUpdate
            && (state == State.IDLE || state == State.RUNNING))
            {
                rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
                state = State.IDLE;
            }  

            //Check grounded but we want to get hit vector of the surface the ray is casted into, so we don't use the defined function
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, collider.bounds.extents.y, ground);

            if (isAPressedThisUpdate)
            {
                transform.localScale = new Vector2(-1, 1);
                if (hit)
                {
                    Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                    rigidbody2d.velocity = (-speed) * moveVect;
                } else
                {
                    rigidbody2d.velocity = new Vector2(-speed, rigidbody2d.velocity.y);
                }
            }
            else if (isDPressedThisUpdate)
            {
                transform.localScale = new Vector2(1, 1);
                if (hit)
                {
                    Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                    rigidbody2d.velocity = speed * moveVect;
                } else
                {
                    rigidbody2d.velocity = new Vector2(speed, rigidbody2d.velocity.y);
                }     
            }

            if (isSpacePressedThisUpdate && isGrounded && state != State.FALLING && state != State.JUMPING && canJump)
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpSpeed);
                state = State.JUMPING;
                canJump = false;
            }
            CheckSlope(isAPressedThisUpdate, isDPressedThisUpdate);

            checkVelocityState();
            animator.SetInteger("state", (int)state);
        }

        //Set all the press variable to false to prepare for next fixedupdate
        isAPressedThisUpdate = false;
        isDPressedThisUpdate = false;
        isSpacePressedThisUpdate = false;
    }

    public void CheckSlope(bool isAPressed, bool isDPressed)
    {
        // Character is grounded, and no axis pressed: 
        if (CheckIsGrounded() && !isAPressed && !isDPressed && state != State.JUMPING && state != State.FALLING)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position, Vector2.down, collider.bounds.extents.y, ground);

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
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position, Vector2.down, collider.bounds.extents.y, ground);

        return hit;
    }
}
