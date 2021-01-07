using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : MonoBehaviour
{
    public LayerMask playerLayer;

    public float DetectRadius;

    Animator animator;
    Collider2D collider;
    Rigidbody2D rigidbody2d;
    

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
        
    }

    void FixedUpdate()
    {
        Collider2D collider = Physics2D.OverlapCircle(rigidbody2d.position, DetectRadius, playerLayer);

        if (collider != null)
        {
            animator.SetTrigger("hit");

            float lookDirection = collider.transform.position.x - gameObject.transform.position.x;

            //player is on the right side
            if (lookDirection > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
            else if (lookDirection < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
        }
    }
    private void Attack()
    {
        
    }
}
