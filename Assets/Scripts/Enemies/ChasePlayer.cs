using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
    EnemyScript enemyScript;

    GravityScript gravityScript;

    public float ChasingSpeed;

    new Rigidbody2D rigidbody2D;

    Animator animator;

    float detectRadius;
    LayerMask playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
        gravityScript = GetComponent<GravityScript>();
        animator = GetComponent<Animator>();
        detectRadius = enemyScript.DetectRadius;
        playerLayer = enemyScript.PlayerLayer;

        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, detectRadius, playerLayer);

        //if detect player
        if (collider != null)
        {
            moveTorward(collider);
        }
        else
        {
            stopMoving();
        }

    }

    private void stopMoving()
    {
        rigidbody2D.velocity = new Vector2(0.0f, rigidbody2D.velocity.y);

        gravityScript.SetEntityMovingState(false, null);
    }

    private void moveTorward(Collider2D collider)
    {
        float distance = collider.transform.position.x - gameObject.transform.position.x;

        if (Mathf.Abs(distance) >= enemyScript.AttackRange)
        {
            //enemy on the left
            if (distance < 0)
            {
                rigidbody2D.velocity = new Vector2(-ChasingSpeed, 0.0f);
            }
            else
            {
                rigidbody2D.velocity = new Vector2(ChasingSpeed, 0.0f);
            }
            gravityScript.SetEntityMovingState(true, null);
        }
        else
        {
            stopMoving();
        }
        

        

    }
}
