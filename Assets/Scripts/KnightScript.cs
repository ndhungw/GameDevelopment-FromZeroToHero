using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //horizontal = Input.GetAxis("Horizontal");

        // Running state
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody2d.velocity = new Vector2(-speed, rigidbody2d.velocity.y);
            animator.SetFloat("MoveX", 0.0f);
            animator.SetFloat("Speed", speed);
            //animator.SetBool("IsJumping", false);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rigidbody2d.velocity = new Vector2(speed, rigidbody2d.velocity.y);
            animator.SetFloat("MoveX", 1.0f);
            animator.SetFloat("Speed", speed);
            //animator.SetBool("IsJumping", false);
        }

        // Idle state
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            animator.SetFloat("Speed", 0.0f);
            animator.SetBool("IsJumping", false);
        }

        //rigidbody2d.velocity = new Vector2(horizontal * speed, rigidbody2d.velocity.y);

        //animator.SetFloat("Speed", Mathf.Abs(rigidbody2d.velocity.x));

        //float direction = (horizontal + 1) / 2;
        //animator.SetFloat("MoveX", direction);

        //Debug.Log(horizontal);



        if (Input.GetKeyDown(KeyCode.Space)) {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpSpeed);
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }

        //animator.SetFloat("MoveY", rigidbody2d.velocity.y);

        //if (Mathf.Approximately(rigidbody2d.velocity.y, 0.0f))
        //{
        //    animator.SetBool("IsJumping", false);
        //}

        // Falling
        if (animator.GetBool("IsJumping")  && rigidbody2d.velocity.y < 0.1f)
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false);
            Debug.Log("Fall");
        }

        if (animator.GetBool("IsFalling") && collider.IsTouchingLayers(ground))
        {
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false);
        }

        

    }

    void FixedUpdate() {
        //animator.SetFloat("MoveY", rigidbody2d.velocity.y);

        //if (Mathf.Approximately(rigidbody2d.velocity.y, 0.0f))
        //{
        //    animator.SetBool("IsJumping", false);
        //}
    }

    enum State
    {
        JUMPING,
        FALLING,
    }
}
