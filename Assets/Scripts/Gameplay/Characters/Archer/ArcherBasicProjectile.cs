using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBasicProjectile : MonoBehaviour
{
    public LayerMask ground;

    Rigidbody2D rigidBody2d;
    Vector3 firedPosition;

    private int baseDamage;
    private float critRate;
    private float critDamage;
    private float arrowRange;

    bool hasHittedEnemy = false;

    void Awake()
    {
        rigidBody2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Archer characterStats = (Archer)GameManager.GM.numbersForCharacters[GameManager.CHARACTERS.ARCHER];
        baseDamage = characterStats.BaseDamage;
        critRate = characterStats.critRate;
        critDamage = characterStats.critDamage;
        arrowRange = characterStats.arrowRange;
    }

    private void OnEnable()
    {
        hasHittedEnemy = false;
    }

    public void Launch(Vector2 position, Vector2 direction, float force)
    {
        firedPosition = position;
        rigidBody2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // collide with enemy
        var e = collision.collider.GetComponent<EnemyScript>();
        if (e != null && !hasHittedEnemy)
        {
            // Logic for damage here
            int output = baseDamage;
            int chance = Random.Range(0, 101);
            if(chance <= critRate)
            {
                output = Mathf.RoundToInt(output * critDamage);
            }
            e.ChangeHealth(-output);

            hasHittedEnemy = true;

            gameObject.SetActive(false);
            return;
        }
        if ((1<<collision.collider.gameObject.layer) == ground.value)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        var distance = Vector3.Distance(firedPosition, gameObject.transform.position);
        if (distance > arrowRange*8)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
