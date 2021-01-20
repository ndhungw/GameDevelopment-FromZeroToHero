using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoManager
{
    public enum GAME_CHARACTER
    {
        KNIGHT,
        WIZARD,
        ARCHER,
    }

    public static Knight knight = new Knight();

    public static Archer archer = new Archer();

    public static Wizard wizard = new Wizard();

    public static Dictionary<GAME_CHARACTER, GameCharacter> characterDictionary { get; protected set; }

    public static int Food = 0;

    public static int Money = 1000;

    //private static GameInfoManager instance = null;

    //private int money = 1000;
    //private int food = 0;

    static GameInfoManager(){
        characterDictionary = new Dictionary<GAME_CHARACTER, GameCharacter>
        {
            { GAME_CHARACTER.KNIGHT, knight },
            { GAME_CHARACTER.WIZARD, wizard },
            { GAME_CHARACTER.ARCHER, archer }
        };
    }



    //public int Money { get { return money; } }
    //public int Food { get { return food; } }

    //public List<GameCharacter> Characters = new List<GameCharacter>();

    //void Awake()
    //{
    //    Characters.Add(new Knight());
    //    Characters.Add(new Archer());
    //    Characters.Add(new Wizard());
    //}

    public static bool ChangeMoney(int amount)
    {
        if (Money + amount < 0)
        {
            return false;
        }

        Money += amount;
        return true;
    }

    public static int ChangeFood(int amount)
    {
        Food = Food + amount;

        if (Food < 0)
        {
            int owed = Food;
            Food = 0;
            return owed;
        }
        else
        {
            return 0;
        }
    }

    //private GameInfoManager() { }

    //private void OnEnable()
    //{
    //    Debug.Log(GameInfoManager.GetInstance());
    //}

    //public static GameInfoManager GetInstance()
    //{
    //    if (instance == null)
    //    {
    //        instance = new GameInfoManager();

    //        var singletonObject = new GameObject();
    //        instance = singletonObject.AddComponent<GameInfoManager>();
    //        singletonObject.name = typeof(GameInfoManager).ToString() + " (Singleton)";

    //        // Make instance persistent.
    //        DontDestroyOnLoad(singletonObject);
    //    }
    //    return instance;
    //}


}