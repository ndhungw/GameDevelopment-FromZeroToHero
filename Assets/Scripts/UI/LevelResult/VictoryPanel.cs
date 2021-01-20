using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    public Text EnemiesKilledText;
    public Text TreasureText;
    public Text HeroesRemainText;
    public Text TotalIncomText;
    // Start is called before the first frame update
    void Start()
    {
        LevelManager manager = LevelManager.LM;
        EnemiesKilledText.text = manager.MonstersKilled.ToString() + " x " + manager.GoldPerMonster;
        TreasureText.text = manager.TreasureFound.ToString();
        HeroesRemainText.text = manager.GetHeroesRemain().ToString() + " x " + manager.HeroesRemainReward;
        TotalIncomText.text = manager.CalculateIncome().ToString();
    }

    public void ReturnToVillage()
    {
        GameInfoManager.ChangeMoney(LevelManager.LM.CalculateIncome());
        SceneManager.LoadScene("Village");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
