using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScript : MonoBehaviour
{
    public LayerMask ground;
    public Transform feet;
    //How many screen unit to cast the ray
    public float RaycastDistanceFromFeet = 2f;
    public Sprite avatarSprite;

    protected int currentHealth = 100;
    protected float speed = 3.0f;
    protected float jumpSpeed = 10.0f;
    protected int MaxHealth = 100;

    protected Rigidbody2D rigidbody2d;
    protected Animator animator;
    protected new Collider2D collider;

    public float InvincibleTime = 2.0f;
    protected bool isInvincible = true;
    float invincibleTimer;

    protected bool isIFraming = false;

    protected bool isHit = false;
    protected bool isActuallyDead = false;

    private float staggerTime;
    bool isStaggered = false;
    private float staggerTimer;

    protected float? previousTime = null;

    bool canJump = true;
    protected bool canAttack = true;

    public AudioClip AttackSound;

    public AudioClip HitSound;

    public AudioClip DieSound;

    protected AudioSource audioSource;

    public enum State
    {
        IDLE = 0,
        RUNNING = 1,
        JUMPING = 2,
        FALLING = 3,
    }

    public State state = State.IDLE;

    // Start is called before the first frame update
    protected void Start()
    {
        if (!feet)
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #endif
                        throw new Exception("Feet need to be assigned to use this script");
        }

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        state = State.IDLE;
        staggerTime = Mathf.Max(InvincibleTime / 4, 0.5f);
        HealthBar.instance.SetValue(currentHealth, MaxHealth);
        HealthBar.instance.SetAvatar(avatarSprite);

        audioSource = GetComponent<AudioSource>();
        
    }

    // Called before start and repeated on every reenabling attempt
    protected void OnEnable()
    {
        isInvincible = true;
        invincibleTimer = 0.5f;

    }

    protected void OnDisable()
    {
        previousTime = Time.time;
    }

    private void checkVelocityState()
    {
        bool isGrounded = CheckIsGrounded();

        if (state == State.JUMPING)
        {
            if (!isGrounded && rigidbody2d.velocity.y < -0.1f)
            {
                state = State.FALLING;
            }
        }
        else if (state == State.FALLING)
        {
            if (isGrounded)
            {
                state = State.IDLE;
            }
        }
        else if (!isGrounded && rigidbody2d.velocity.y < -speed)
        {
            state = State.FALLING;
        }
        else if (Mathf.Abs(rigidbody2d.velocity.x) > 1f)
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
        int changeAmount = 0;
        if (amount < 0)
        {
            if (isInvincible || isIFraming)
            {
                return;
            }
            isHit = true;
            animator.SetTrigger("hit");
            changeAmount = calculateDamage(amount);
            GameplayManager.GM?.CreateEnemyDamageText(Mathf.Abs(changeAmount), gameObject);

            isInvincible = true;
            invincibleTimer = InvincibleTime;
            isStaggered = true;
            staggerTimer = staggerTime;
            
            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
        }

        currentHealth = Mathf.Clamp(currentHealth + changeAmount, 0, MaxHealth);

        HealthBar.instance.SetValue(currentHealth, MaxHealth);

        SetCurrentHealthToGameInfoManager();

        if (currentHealth <= 0)
        {
            isInvincible = true;
            animator.SetTrigger("dead");
        }
    }

    protected virtual void SetCurrentHealthToGameInfoManager()
    {

    }

    protected virtual int calculateDamage(int amount)
    {
        return amount;
    }

    private void hitAnimationEnd()
    {
        isHit = false;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    // One unit = 8 pixels
    public void Knockback(float knockbackInUnit)
    {
        rigidbody2d.velocity = new Vector2(knockbackInUnit, Mathf.Abs(knockbackInUnit) * (jumpSpeed /speed) / Mathf.Tan(30) );
    }

    protected void FixedUpdate()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (isStaggered)
        {
            staggerTimer -= Time.deltaTime;
            if(staggerTimer < 0)
            {
                isStaggered = false;
            }
        }

        bool isGrounded = CheckIsGrounded();


        //Fixed too much jump with this if condition
        if(isGrounded && !Input.GetKey(KeyCode.Space) && !canJump)
        {
            canJump = true;
        }

        // we skip input checking + input validation if character is staggering
        if (!isStaggered && currentHealth > 0)
        {
            //Check grounded but we want to get normal vector of the surface the ray is casted into, so we don't use the defined function
            RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, RaycastDistanceFromFeet, ground);

            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector2(-1, 1);
                // Onground - our rule
                if (hit && state != State.JUMPING && state != State.FALLING)
                {
                    Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                    rigidbody2d.velocity = (-speed) * moveVect;
                } 
                // Mid-air - engine's rule
                else
                {
                    rigidbody2d.velocity = new Vector2(-speed, rigidbody2d.velocity.y);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(1, 1);
                // Onground - our rule
                if (hit && state != State.JUMPING && state != State.FALLING)
                {
                   Vector3 moveVect = Vector3.Cross(hit.normal, Vector3.forward);
                   rigidbody2d.velocity = speed * moveVect;
                }
                // Mid-air - engine's rule
                else
                {
                    rigidbody2d.velocity = new Vector2(speed, rigidbody2d.velocity.y);
                }     
            } 

            // Jump input
            if (Input.GetKey(KeyCode.Space) && isGrounded && canJump)
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpSpeed);
                state = State.JUMPING;
                canJump = false;
            }
        }

        // Stop sliding on X after apply velocity from button
        if (!Mathf.Approximately(rigidbody2d.velocity.x, 0.0f) && isGrounded && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)
            && (state == State.IDLE || state == State.RUNNING))
        {
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigidbody2d.freezeRotation = true;
            rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
            rigidbody2d.constraints = RigidbodyConstraints2D.None;
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            state = State.IDLE;
        }

        CheckSlope(Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D));

        checkVelocityState();
        animator.SetInteger("state", (int)state);
    }

    public void CheckSlope(bool isAPressed, bool isDPressed)
    {
        // Character is grounded, and no axis pressed: 
        if (CheckIsGrounded() && !isAPressed && !isDPressed && state != State.JUMPING && state != State.FALLING)
        {
            RaycastHit2D hit = Physics2D.Raycast(feet.position, Vector2.down, RaycastDistanceFromFeet, ground);

            // Check if there is slope
            if (hit && Mathf.Abs(hit.normal.x) > 0.1f)
            {
                // We freeze position X of the rigidbody constraints and put x velocity to 0
                // to stop the normal physics on slope, making object only movable from us setting velocity
                rigidbody2d.constraints = RigidbodyConstraints2D.FreezePositionX;
                rigidbody2d.freezeRotation = true;
                rigidbody2d.velocity = new Vector2(0.0f, rigidbody2d.velocity.y);
            }
        }
        else
        {
            // if we are on air or moving - jumping, unfreeze all and freeze only rotation.
            rigidbody2d.constraints = RigidbodyConstraints2D.None;
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    protected bool CheckIsGrounded()
    {
        RaycastHit2D hit = Physics2D.CircleCast(feet.position, RaycastDistanceFromFeet/2, Vector2.down, 0.0f, ground); 
        return hit && collider.IsTouchingLayers(ground);
    }

    public void DeactivateOnDead()
    {
        gameObject.SetActive(false);
        isActuallyDead = true;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(feet.position, RaycastDistanceFromFeet / 2);
    }

    public void startIFraming()
    {
        isIFraming = true;
    }

    public void endIFraming()
    {
        isIFraming = false;
    }

    public bool isCharacterActuallyDead()
    {
        return isActuallyDead;
    }

    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public bool isAbleToClickAttack()
    {
        return canAttack;
    }

    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(AttackSound);
    }

    public void PlayDeadSound()
    {
        audioSource.PlayOneShot(DieSound);
    }

    public void PlayHitSound()
    {
        audioSource.PlayOneShot(HitSound);
    }
}
