using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : EnemyScript
{
    public GameObject ProjectilePrefab;

    public override void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        nextAttackTime = Time.time + 1f / AttackRate;
        

        var circleCastResults = Physics2D.CircleCastAll(AttackPoint.position, AttackRange, Vector2.up, Mathf.Infinity, PlayerLayer);

        //if player was attacked
        if (circleCastResults != null && circleCastResults.Length > 0)
        {
            GameObject attacked = circleCastResults[0].collider.gameObject;

            Vector2 direction = new Vector2(
                attacked.transform.position.x - transform.position.x,
                attacked.transform.position.y - transform.position.y);

            direction.Normalize();

            if (canAttack)
            {
                // change health and knocked back
                Launch(direction);
            }

        }

        canAttack = false;

    }

    void Launch(Vector2 lookDirection)
    {
        GameObject projectileObject = Instantiate(ProjectilePrefab, AttackPoint.position, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile)
        {
            projectile.Launch(lookDirection, 300);
        }
       
        //animator.SetTrigger("Launch");
        //PlaySound(lauchSound);
    }
}
