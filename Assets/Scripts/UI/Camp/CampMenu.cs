using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CampUI;

    private void SetHealthEffect(int perecentages)
    {
        GameInfoManager.knight.SetCurrentHealth(GameInfoManager.knight.CurrentHealth * perecentages / 100);
        GameInfoManager.archer.SetCurrentHealth(GameInfoManager.archer.CurrentHealth * perecentages / 100);
        GameInfoManager.wizard.SetCurrentHealth(GameInfoManager.wizard.CurrentHealth * perecentages / 100);
    }

    void onEnable()
    {
        Time.timeScale = 0;
    }

    public void UseFood(int amount)
    {
        if (amount < GameInfoManager.Food)
        {
            return;
        }

        switch(amount)
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
