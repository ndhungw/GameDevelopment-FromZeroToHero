using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : CharacterScript
{
    public LayerMask enemyLayer;
    public float DamageRange;
    public Transform AttackPoint;
    private int BaseDamage = 50;

    private float delayBetweenAttacks = 1.0f;
    private float defenseAgainstAttacks;
    private float delayTimer = 0.0f;
    private bool canAttack = true;

    //Attack related
    bool isAttacking = false;

    private new void OnEnable()
    {
        base.OnEnable();
        Knight characterStats = GameInfoManager.knight;
        speed = characterStats.Speed;
        jumpSpeed = characterStats.JumpSpeed;
        MaxHealth = characterStats.MaxHealth;
        BaseDamage = characterStats.BaseDamage;
        delayBetweenAttacks = characterStats.timeBetweenSwings;
        defenseAgainstAttacks = characterStats.defenseAgainstAttack;
        currentHealth = characterStats.CurrentHealth;
        HealthBar.instance.SetValue(currentHealth, MaxHealth);
        HealthBar.instance.SetAvatar(avatarSprite);
    }

    protected override void SetCurrentHealthToGameInfoManager()
    {
        GameInfoManager.knight.SetCurrentHealth(currentHealth);
    }

    private void Update()
    {
        if (isHit)
        {
            if (!canAttack)
            {
                canAttack = true;
            }
            if (isAttacking)
            {
                isAttacking = false;
            }
        }
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack && CheckIsGrounded())
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

                    // change health
                    if (script != null)
                    {
                        script.ChangeHealth(-BaseDamage);
                    }
                }
            }
        }
    }

    protected override int calculateDamage(int amount)
    {
        var random = Random.Range(0, 2);
        if (random == 0)
        {
            amount = Mathf.RoundToInt(amount * (1.0f - defenseAgainstAttacks));
        }
        return amount;
    }

    private void Attack()
    {
        isAttacking = true;
        canAttack = false;
        animator.SetTrigger("attack");
        delayTimer = delayBetweenAttacks;
    }
    
    private void OnAttackAnimationEnd()
    {
        isAttacking = false;
        canAttack = true;
        isIFraming = false;
    }

    private new void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (AttackPoint != null)
        {
            Gizmos.DrawWireSphere(AttackPoint.position, DamageRange);
        }
    }
}
