using Assets.Scripts.Game_System;
using UnityEngine;

public class WizardScript : CharacterScript
{
    public GameObject CastCircle01Skill;
    public Transform CastSkill01Point;

    private float delayBetweenAttacks;
    private float delayTimer = 0.0f;
    private bool canAttack = true;

    private new void OnEnable()
    {
        base.OnEnable();
        Wizard characterStats = GameInfoManager.wizard;
        speed = characterStats.Speed;
        jumpSpeed = characterStats.JumpSpeed;
        MaxHealth = characterStats.MaxHealth;
        delayBetweenAttacks = characterStats.Cast01SkillCooldown;
        currentHealth = characterStats.CurrentHealth;
        HealthBar.instance.SetValue(currentHealth, MaxHealth);
        HealthBar.instance.SetAvatar(avatarSprite);
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

    protected override void SetCurrentHealthToGameInfoManager()
    {
        GameInfoManager.wizard.SetCurrentHealth(currentHealth);
    }

    private void Attack()
    {
        canAttack = false;
        animator.SetTrigger("attackE");
    }

    private void SpawnCastCircle01()
    {
        delayTimer = delayBetweenAttacks;
        if (CastSkill01Point)
        {
            GameObject spellCasted = Instantiate(CastCircle01Skill, CastSkill01Point.position, new Quaternion());
        }
    }

    private void OnAttackAnimationEnd()
    {
        canAttack = true;
        isIFraming = false;
    }
}
