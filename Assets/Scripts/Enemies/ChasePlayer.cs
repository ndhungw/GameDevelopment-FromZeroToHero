using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
    EnemyScript enemyScript;

    GravityScript gravityScript;

    public float ChasingSpeed;

    Rigidbody2D rigidbody2D;

    float detectRadius;
    LayerMask playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
        gravityScript = GetComponent<GravityScript>();
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
    }

    private void moveTorward(Collider2D collider)
    {
        float distance = collider.transform.position.x - gameObject.transform.position.x;

        //enemy on the left
        if (distance < 0)
        {
            rigidbody2D.velocity = new Vector2(-ChasingSpeed, 0);
        }
        else if (distance > 0)
        {
            rigidbody2D.velocity = new Vector2(ChasingSpeed, 0);
        }

        gravityScript.SetEntityMovingState(true, null);

    }
}
