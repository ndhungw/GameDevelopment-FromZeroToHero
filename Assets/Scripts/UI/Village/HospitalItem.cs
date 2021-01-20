using Assets.Scripts.Game_System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HospitalItem : MonoBehaviour
{
    public Image HealthBarMask;

    float originalSize;

    public Text HealthText;

    public Text healCost;

    public GameCharacter gameCharacter;

    private int cost;

    public void SetGameCharacter(GameCharacter gameCharacter)
    {
        this.gameCharacter = gameCharacter;
    }

    private int calculateHealCost()
    {
        return 500 * (gameCharacter.MaxHealth - gameCharacter.CurrentHealth) / gameCharacter.MaxHealth;
    }

    public void HealGameCharacter()
    {
        if (gameCharacter.CurrentHealth < gameCharacter.MaxHealth)
        {

            bool canPay = GameInfoManager.ChangeMoney(-calculateHealCost());

            if (canPay)
            {
                gameCharacter.SetCurrentHealth(gameCharacter.MaxHealth);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        originalSize = HealthBarMask.rectTransform.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameCharacter != null)
        {
            float value = (float)gameCharacter.CurrentHealth / (float) gameCharacter.MaxHealth;
            Debug.Log(value);
            HealthBarMask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);

            HealthText.text = gameCharacter.CurrentHealth.ToString() + "/" + gameCharacter.MaxHealth.ToString();
            healCost.text = calculateHealCost().ToString();
        }
        

    }
}
