using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudbornScript : EnemyScript
{
    
    public override void ExecuteAttack()
    {
        var circleCastResults = Physics2D.CircleCastAll(AttackPoint.position, DamageRange, Vector2.up, 0.0f, PlayerLayer);

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
}
