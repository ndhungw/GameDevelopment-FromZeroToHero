﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : MonoBehaviour
{
    public LayerMask playerLayer;

    public float DetectRadius;
    public float AttackRange;
    public float AttackRate;

    public Transform AttackPoint;

    float nextAttackTime = 0f;
    bool canAttack = true;

    Animator animator;
    new Collider2D collider;
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
        if (Time.time >= nextAttackTime)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }

    void FixedUpdate()
    {
        Collider2D collider = Physics2D.OverlapCircle(rigidbody2d.position, DetectRadius, playerLayer);

        if (collider != null)
        {
            float distance = collider.transform.position.x - gameObject.transform.position.x;

            //player is on the right side
            if (distance > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
            else if (distance < 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }

            if (Mathf.Abs(distance) <= AttackRange)
            {
                if (canAttack)
                {
                    Attack();
                    
                }
                
            }
        }
    }
    private void Attack()
    {

        Collider2D attacked = Physics2D.OverlapCircle(AttackPoint.position, AttackRange, playerLayer);

        if (attacked != null)
        {
            animator.SetTrigger("hit");
            CharacterScript script = attacked.GetComponent<CharacterScript>();

            if (script != null)
            {
                script.ChangeHealth(-10);
                nextAttackTime = Time.time + 1f / AttackRate;
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
        }
    }
}
