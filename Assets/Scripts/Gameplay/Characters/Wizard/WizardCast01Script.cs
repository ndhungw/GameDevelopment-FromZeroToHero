using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardCast01Script : MonoBehaviour
{
    public LayerMask enemyLayer;
    public float DamageRange;
    public Transform AttackPoint;
    
    private float skillDuration;
    private int skillDamage;
    private float skillTimer;

    private void OnEnable()
    {
        Wizard characterStats = GameInfoManager.wizard;
        // 80% base damage
        skillDamage = Mathf.RoundToInt(characterStats.BaseDamage * characterStats.Cast01SkillMultiplier);
        skillDuration = characterStats.Cast01SkillDuration;
        skillTimer = skillDuration;
    }

    private void Update()
    {
        skillTimer -= Time.deltaTime;
        if(skillTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void PulsateDamageCircle()
    {
        var circleCastResults = Physics2D.CircleCastAll(AttackPoint.position, DamageRange, Vector2.up, 0.0f, enemyLayer);

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
                    script.ChangeHealth(-skillDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (AttackPoint != null)
        {
            Gizmos.DrawWireSphere(AttackPoint.position, DamageRange);
        }
    }
}
