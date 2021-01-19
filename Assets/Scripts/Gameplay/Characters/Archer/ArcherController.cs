using UnityEngine;
using Assets.Scripts.Game_System;

public class ArcherController : CharacterScript
{
    private float delayBetweenAttacks;
    private float shootArrowForce;
    private float delayTimer = 0.0f;
    private bool canAttack = true;

    private new void Start()
    {
        base.Start();
        Archer characterStats = (Archer) GameplayManager.GM.numbersForCharacters[GameplayManager.CHARACTERS.ARCHER];
        speed = characterStats.Speed;
        jumpSpeed = characterStats.JumpSpeed;
        MaxHealth = characterStats.MaxHealth;
        delayBetweenAttacks = characterStats.timeBetweenShots;
        shootArrowForce = characterStats.launchArrowForce;

        currentHealth = MaxHealth;
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
