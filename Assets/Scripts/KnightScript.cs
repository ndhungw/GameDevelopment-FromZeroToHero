using UnityEngine;

public class KnightScript : MonoBehaviour
{
    public float speed = 3.0f;
    public float jumpSpeed = 10.0f;
    public LayerMask ground;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Vector2 lookDirection;
    Collider2D collider;

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
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (horizontal < 0)
        {
            animator.SetFloat("MoveX", 0.0f);
            rigidbody2d.velocity = new Vector2(-speed, rigidbody2d.velocity.y);
            Debug.Log(horizontal);
            //transform.localScale = new Vector2(1, 1);

        }
        else if (horizontal > 0)
        {
            animator.SetFloat("MoveX", 1.0f);
            rigidbody2d.velocity = new Vector2(speed, rigidbody2d.velocity.y);
            Debug.Log(horizontal);
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

    //private void setState(State state)
    //{
    //    animator.SetInteger("state", (int) state);
    //}
}
