using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static LevelManager LM;

    private static readonly object padlock = new object();

    public int MonstersKilled { get; protected set; }

    public int MonsterCount { get; protected set; }

    public int GoldPerMonster = 10;

    public int TreasureFound = 0;

    public int HeroesRemanin = 0;

    public int HeroesRemainReward = 20;

    public void ChangeMonsterCount(int amount)
    {
        lock (padlock)
        {
            MonsterCount += amount;
        }
    }

    public void ChangeMonstersKilled(int amount)
    {
        lock (padlock)
        {
            MonstersKilled += amount;
        }
    }



    

    void Awake()
    {
        if (LM == null)
        {
            var gameplayManagerObj = GameObject.FindGameObjectWithTag("LevelController");
            LM = gameplayManagerObj.GetComponent<LevelManager>();
            DontDestroyOnLoad(gameplayManagerObj);
        }
    }
    void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    // Update is called once per frame
    void Update()
    {
        int heroCount = GetHeroesRemain();
        if (heroCount  == 0)
        {
            ShowLosePanel();
        }
    }

    public int GetHeroesRemain()
    {
        int heroCount = 0;
        if (GameInfoManager.knight.CurrentHealth > 0)
        {
            heroCount++;
        }
        if (GameInfoManager.archer.CurrentHealth > 0)
        {
            heroCount++;
        }
        if (GameInfoManager.archer.CurrentHealth > 0)
        {
            heroCount++;
        }

        HeroesRemanin = heroCount;

        return HeroesRemanin;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        MonsterCount = 0;

    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        MonstersKilled = 0;

        int heroCount = 0;
        if (GameInfoManager.knight.CurrentHealth > 0)
        {
            heroCount++;
        }
        if (GameInfoManager.archer.CurrentHealth > 0)
        {
            heroCount++;
        }
        if (GameInfoManager.archer.CurrentHealth > 0)
        {
            heroCount++;
        }

        HeroesRemanin = heroCount;

        TreasureFound = 0;

    }

    public int CalculateIncome()
    {
        int slay = MonstersKilled * GoldPerMonster;

        int heroCount = 0;
        if (GameInfoManager.knight.CurrentHealth > 0)
        {
            heroCount++;
        }
        if (GameInfoManager.archer.CurrentHealth > 0)
        {
            heroCount++;
        }
        if (GameInfoManager.archer.CurrentHealth > 0)
        {
            heroCount++;
        }

        HeroesRemanin = heroCount;
        int keepheroAlive = HeroesRemanin * HeroesRemainReward;

        int sum = slay + keepheroAlive + TreasureFound;

        

        return sum;
    }

    public void ShowVictoryPanel()
    {
        VictoryPanel.instance.Panel.SetActive(true);
        LosePanel.instance.Panel.SetActive(false);

        //Time.timeScale = 0;
    }

    public bool CheckWinCondition()
    {
        if (MonstersKilled == MonsterCount)
        {
            return true;
        }
        return false;
    }

    public void ShowLosePanel()
    {
        VictoryPanel.instance.Panel.SetActive(false);
        LosePanel.instance.Panel.SetActive(true);
        //Time.timeScale = 0;
    }
}
