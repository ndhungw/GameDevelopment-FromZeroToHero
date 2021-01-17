using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float DamageRange;
    public Transform AttackPoint;

    public float delayBetweenAttacks = 1.0f;
    private float delayTimer = 0.0f;

    //Attack related
    bool isAttacking = false;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        delayTimer = delayBetweenAttacks;
    }

    private void Update()
    {
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();     
            }
        }

        // If knight is attacking, check for damage range around the attack point for possible collision with enemies
        if (isAttacking)
        {
            var circleCastResults = Physics2D.CircleCastAll(AttackPoint.position, DamageRange, Vector2.up, Mathf.Infinity, enemyLayer);

            if (circleCastResults != null)
            {
                foreach (var result in circleCastResults)
                {
                    Collider2D attacked = result.collider;
                    //to be filled in
                    EnemyScript script = attacked.GetComponent<EnemyScript>();

                    // change health and knocked back
                    if (script != null)
                    {
                        script.ChangeHealth(-50);
                    }
                }
            }
        }
    }

    private void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        delayTimer = delayBetweenAttacks;
    }
    
    private void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.DrawWireSphere(AttackPoint.position, DamageRange);
        }
    }
}
