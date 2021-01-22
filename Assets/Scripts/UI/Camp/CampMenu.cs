using Assets.Scripts.Game_System;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CampUI;

    public Text FoodText;

    private void SetHealthEffect(int perecentages)
    {
        List<Tuple<GameObject, Type>> characters = GameplayManager.GM.GetCurrentCharacterScripts();

        foreach (var e in characters)
        {
            if (e.Item1 != null)
            {
                CharacterScript script = e.Item1.GetComponent<CharacterScript>();
                if (script)
                {
                    int currentHealth = 0;

                    if (e.Item2==typeof(Knight))
                    {
                        currentHealth = GameInfoManager.knight.CurrentHealth;
                    }
                    else if (e.Item2 == typeof(Archer))
                    {
                        currentHealth = GameInfoManager.archer.CurrentHealth;
                    }
                    else if (e.Item2 == typeof(Wizard))
                    {
                        currentHealth = GameInfoManager.wizard.CurrentHealth;
                    }

                    int value = currentHealth * (100 - perecentages) / 100;
                    Debug.Log("change health: " + value);
                    script.ChangeHealth(-value);

                }
            }
            else
            {
                if (e.Item2 == typeof(Knight))
                {
                    int newHealth = GameInfoManager.knight.CurrentHealth * perecentages / 100;

                    GameInfoManager.knight.SetCurrentHealth(newHealth);
                }
                else if (e.Item2 == typeof(Archer))
                {
                    int newHealth = GameInfoManager.archer.CurrentHealth * perecentages / 100;
                    GameInfoManager.archer.SetCurrentHealth(newHealth);
                }
                else if (e.Item2 == typeof(Wizard))
                {
                    int newHealth = GameInfoManager.wizard.CurrentHealth * perecentages / 100;
                    GameInfoManager.wizard.SetCurrentHealth(newHealth);
                }

            }
        }   
    }

    void onEnable()
    {
        Time.timeScale = 0;
        FoodText.text = GameInfoManager.Food.ToString();
    }

    void Update()
    {
        FoodText.text = GameInfoManager.Food.ToString();
    }

    public void UseFood(int amount)
    {
        if (amount < GameInfoManager.Food)
        {
            return;
        }

        switch (amount)
        {
            case 0:
                SetHealthEffect(60);
                break;
            case 3:
                SetHealthEffect(80);
                break;
            case 6:
                SetHealthEffect(100);
                break;
            case 12:
                SetHealthEffect(120);
                break;
        }
        GameInfoManager.ChangeFood(-amount);

        Time.timeScale = 1;
        Debug.Log("hello");
        CampUI.SetActive(false);
    }

}
