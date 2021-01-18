using UnityEngine;
using Assets.Scripts.Game_System;

public class ArcherController : CharacterScript
{
    private int BaseDamage;

    private float delayBetweenAttacks;
    private float delayTimer = 0.0f;
    private bool canAttack = true;

    private new void Start()
    {
        base.Start();
        Archer characterStats = (Archer) GameManager.GM.numbersForCharacters[GameManager.CHARACTERS.ARCHER];
        speed = characterStats.Speed;
        jumpSpeed = characterStats.JumpSpeed;
        MaxHealth = characterStats.MaxHealth;
        BaseDamage = characterStats.BaseDamage;
        delayBetweenAttacks = characterStats.timeBetweenShots;

        currentHealth = MaxHealth;
    }


}
