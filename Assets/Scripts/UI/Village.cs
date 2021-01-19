using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Village : MonoBehaviour
{
    public int FoodCost = 100;

    public int foodPerMan = 6;

    public Text FoodText;

    public Text MoneyText;

    void Awake()
    {
        //FoodText.text = gameInfoManager.Food.ToString();

        
    }

    void Update()
    {
        MoneyText.text = GameInfoManager.Money.ToString();

        FoodText.text = GameInfoManager.Food.ToString();
    }
    
    public void BuyFood()
    {
        bool canBuy = GameInfoManager.ChangeMoney(-FoodCost);

        if (canBuy)
        {
            GameInfoManager.ChangeFood(foodPerMan);
        }
    }
}
