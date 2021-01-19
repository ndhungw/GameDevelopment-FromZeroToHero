using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuildItem : MonoBehaviour
{
    public GameCharacter gameCharacter;

    public Text HitPointText;
    public Text DamageText;

    // Start is called before the first frame update
    void Start()
    {
        if (gameCharacter != null)
        {
            HitPointText.text = gameCharacter.CurrentHealth.ToString() + "/"  + gameCharacter.MaxHealth.ToString();
            DamageText.text = gameCharacter.BaseDamage.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameCharacter != null)
        {
            HitPointText.text = gameCharacter.CurrentHealth.ToString() + "/" + gameCharacter.MaxHealth.ToString();
            DamageText.text = gameCharacter.BaseDamage.ToString();
        }
    }

    public void SetGameCharacter(GameCharacter gameCharacter)
    {
        this.gameCharacter = gameCharacter;
    } 
}
