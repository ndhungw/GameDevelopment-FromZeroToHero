using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public LayerMask PlayerLayer;

    public float DetectRadius;
    public float AttackRange;
    public float DamageRange;
    public float AttackRate;
    public Transform AttackPoint;
    //In screen units
    public float ATKKnockBack = 3.75f;

    //Attack related
    float nextAttackTime = 0f;
    bool canAttack = true;
    bool isAttacking = false;

    //Components
    Animator animator;
    GravityScript gravityScript;

    //Movement
    State state = State.IDLE;
    int movingDirection;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gravityScript = GetComponent<GravityScript>();
        movingDirection = UnityEngine.Random.Range(0, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Time.time >= nextAttackTime)
        {
            canAttack = true;
        }

        // If enemy is attacking, check for damage range around the attack point for possible collision with player
        if (isAttacking)
        {
            var circleCastResults = Physics2D.CircleCastAll(AttackPoint.position, DamageRange, Vector2.up, Mathf.Infinity, PlayerLayer);

            //if player was attacked
            if (circleCastResults != null)
            {
                foreach (var result in circleCastResults)
                {
                    Collider2D attacked = result.collider;

                    CharacterScript script = attacked.GetComponent<CharacterScript>();

                    // change health and knocked back
                    if (script != null)
                    {
                        script.ChangeHealth(-10);
                        script.Knockback((float)(transform.localScale.x * ATKKnockBack));
                    }
                }
            }
        }

        // Detect player
        Collider2D collider = Physics2D.OverlapCircle(transform.position, DetectRadius, PlayerLayer);

        if (collider != null)
        {
            float distance = collider.transform.position.x - gameObject.transform.position.x;

            //player is on the right side
            if (distance > 0)
            {
                transform.localScale = new Vector2(1, 1);
            }
            //player is on left side
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
        isAttacking = true;
        animator.SetTrigger("attack");
        nextAttackTime = Time.time + 1f / AttackRate;
        canAttack = false;
    }

    private void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }

    enum State
    {
        IDLE = 0,
        RUNNING = 1,
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.DrawWireSphere(AttackPoint.position, DamageRange);
            Gizmos.DrawWireSphere(GetComponent<Rigidbody2D>().position, AttackRange);
            Gizmos.DrawWireSphere(transform.position, DetectRadius);
        }
    }
}
