using UnityEngine;

public class KnightScript : MonoBehaviour
{
    public float speed = 3.0f;
    public float jumpSpeed = 10.0f;
    public LayerMask ground;
    public int MaxHealth = 100;

    private int currentHealth = 100;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Collider2D collider;

    public float InvincibleTime = 2.0f;
    bool isInvincible;
    float invincibleTimer;

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

        horizontal = Input.GetAxis("Horizontal");

        if (horizontal < 0)
        {
            transform.localScale = new Vector2(-1, 1);
            rigidbody2d.velocity = new Vector2(-speed, rigidbody2d.velocity.y);
            //transform.localScale = new Vector2(1, 1);

        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector2(1, 1);
            rigidbody2d.velocity = new Vector2(speed, rigidbody2d.velocity.y);
            //transform.localScale = new Vector2(-1, 1);

        }

        if (Input.GetKeyDown(KeyCode.Space) && collider.IsTouchingLayers(ground))
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpSpeed);
            state = State.JUMPING;
            //animator.SetBool("IsJumping", true);
            //animator.SetBool("IsFalling", false);
            //setState(State.JUMPING);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("attack");

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeHealth(-1);

        }


        checkVelocityState();
        animator.SetInteger("state", (int)state);





    }

    void FixedUpdate()
    {

        //animator.SetFloat("MoveY", rigidbody2d.velocity.y);

        //if (Mathf.Approximately(rigidbody2d.velocity.y, 0.0f))
        //{
        //    animator.SetBool("IsJumping", false);
        //}
    }

    private void checkVelocityState()
    {
        if (state == State.JUMPING)
        {
            if (rigidbody2d.velocity.y < 0.2f)
            {
                state = State.FALLING;
            }
        }
        else if (state == State.FALLING)
        {
            if (collider.IsTouchingLayers(ground))
            {
                state = State.IDLE;
            }
        }
        else if (Mathf.Abs(rigidbody2d.velocity.x) > 0.01f)
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

    //private void setState(State state)
    //{
    //    animator.SetInteger("state", (int) state);
    //}
}
