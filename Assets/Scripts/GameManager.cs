using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    private int money = 1000;
    private int food = 0;

    public int Money { get { return money; } }
    public int Food { get { return food; } }

    public void ChangeMoney(int amount)
    {
        if (amount > money)
        {
            return;
        }
        money += amount;
    }

    public int ChangeFood(int amount) 
    {
        food = food + amount;

        if (food < 0)
        {
            int owed = food;
            food = 0;
            return owed;
        }
        else
        {
            return 0;
        }
    }

    private GameManager() { }

    private void OnEnable()
    {
        Debug.Log(GameManager.GetInstance());
    }

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameManager();

            var singletonObject = new GameObject();
            instance = singletonObject.AddComponent<GameManager>();
            singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";

            // Make instance persistent.
            DontDestroyOnLoad(singletonObject);
        }
        return instance;
    }


}
