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

    public float StaggerTime;
    public int MaxHealth = 100;
    //In screen units
    public float ATKKnockBack = 3.75f;


    //Attack related
    protected float nextAttackTime = 0f;
    protected bool canAttack = true;
    protected bool isAttacking = false;

    //Invincible related
    public float InvincibleTime = 0.5f;
    bool isInvincible = true;
    float invincibleTimer;

    //getting hit related
    protected int currentHealth;
    
    bool isStaggered = false;
    private float staggerTimer;

    //Components
    protected Animator animator;
    protected GravityScript gravityScript;
    protected Rigidbody2D rigidbody2d;

    enum State
    {
        IDLE = 0,
        RUNNING = 1,
        JUMPING = 2,
        FALLING = 3,
    }

    //Movement
    State state = State.IDLE;
    int movingDirection;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gravityScript = GetComponent<GravityScript>();
        movingDirection = UnityEngine.Random.Range(0, 1);
        rigidbody2d = GetComponent<Rigidbody2D>();

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
            ExecuteAttack();
        }

        if (isStaggered)
        {
            staggerTimer -= Time.fixedDeltaTime;
            if (staggerTimer < 0)
            {
                isStaggered = false;
            }
            return;
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

        checkVelocityState();
        animator.SetInteger("state", (int)state);

    }

    public virtual void Attack()
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

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }

            animator.SetTrigger("hit");
            GameManager.GM?.CreatePlayerDamageText(Mathf.Abs(amount), gameObject);

            isInvincible = true;
            invincibleTimer = InvincibleTime;
            isStaggered = true;
            staggerTimer = StaggerTime;

            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);

        if (currentHealth <= 0)
        {
            animator.SetTrigger("dead");
            //Destroy(gameObject);
        }
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

    private void checkVelocityState()
    {
        if (Mathf.Abs(rigidbody2d.velocity.x) > 1f)
        {
            state = State.RUNNING;
        }
        else
        {
            state = State.IDLE;
        }
    }

    public virtual void ExecuteAttack()
    {

    }
}
