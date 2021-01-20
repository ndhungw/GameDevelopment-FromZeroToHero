using UnityEngine;
using Assets.Scripts.Game_System;

public class ArcherController : CharacterScript
{
    private float delayBetweenAttacks;
    private float shootArrowForce;
    private float delayTimer = 0.0f;

    private new void OnEnable()
    {
        base.OnEnable();
        Archer characterStats = GameInfoManager.archer;
        speed = characterStats.Speed;
        jumpSpeed = characterStats.JumpSpeed;
        MaxHealth = characterStats.MaxHealth;
        delayBetweenAttacks = characterStats.timeBetweenShots;
        shootArrowForce = characterStats.launchArrowForce;
        currentHealth = characterStats.CurrentHealth;
        HealthBar.instance.SetValue(currentHealth, MaxHealth);
        HealthBar.instance.SetAvatar(avatarSprite);

        float currentTime = Time.time;
        if (previousTime.HasValue)
        {
            float elapsedTimeSinceSwitch = Mathf.Abs(currentTime - previousTime.Value);
            // remove the elapsedTime from cooldown
            delayTimer = Mathf.Max(0, delayTimer - elapsedTimeSinceSwitch);
        }
    }

    protected new void OnDisable()
    {
        base.OnDisable();
        previousTime = Time.time;
        if (!canAttack)
        {
            canAttack = true;
        }
        isIFraming = false;
    }

    protected override void SetCurrentHealthToGameInfoManager()
    {
        GameInfoManager.archer.SetCurrentHealth(currentHealth);
    }

    private void Update()
    {
        if (isHit)
        {
            if (!canAttack)
            {
                canAttack = true;
            }
        }
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        canAttack = false;
        animator.SetTrigger("attack");
    }

    private void SpawnArrow()
    {
        delayTimer = delayBetweenAttacks;
        GameObject arrowFired = ObjectPooler.GameSceneGlobal_ObjectPoolingEntity.spawnFromPool("ArcherBasicArrowPool");
        if (arrowFired)
        {
            arrowFired.transform.position = rigidbody2d.position;
            var arrowScript = arrowFired.GetComponent<ArcherBasicProjectile>();
            if (arrowScript)
            {
                arrowFired.SetActive(true);
                arrowScript.Launch(rigidbody2d.position, new Vector2(transform.localScale.x, 0.0f), shootArrowForce * 8);
            }
        }
    }

    private void OnAttackAnimationEnd()
    {
        canAttack = true;
    }
}
