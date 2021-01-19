using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamageZone : MonoBehaviour
{
    public LayerMask playerLayer;

    Collider2D collider2d;

    private void Start()
    {
        collider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Constant touch damage
        var boxSize = collider2d.bounds.size;
        var collidedWithTouchDamageZone = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, playerLayer);
        foreach (var instance in collidedWithTouchDamageZone)
        {
            CharacterScript script = instance.GetComponent<CharacterScript>();

            if (script != null)
            {
                script.ChangeHealth(-5);
                script.Knockback((float)(transform.localScale.x * 3.75));
            }
        }
    }
}
