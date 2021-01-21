using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmithMenu : MonoBehaviour
{
    public Text knightCost;
    public Text knightLevel;

    public Text archerCost;
    public Text archerLevel;

    public Text wizradCost;
    public Text wizardLevel;

    public AudioClip OpenSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }
    private int calculateUpgradeCost(GameCharacter character)
    {
        return 1000 + 1000 * (character.WeaponLevel - 1) / 50;
    }
    void onEnable()
    {
        knightCost.text = calculateUpgradeCost(GameInfoManager.knight).ToString();
        knightLevel.text = (GameInfoManager.knight.WeaponLevel).ToString();

        archerCost.text = calculateUpgradeCost(GameInfoManager.archer).ToString();
        archerLevel.text = (GameInfoManager.archer.WeaponLevel).ToString();

        wizradCost.text = calculateUpgradeCost(GameInfoManager.wizard).ToString();
        wizardLevel.text = (GameInfoManager.wizard.WeaponLevel).ToString();

        
    }

    public void PlayOpenSound() {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(OpenSound);
    }

    void Update()
    {
        knightCost.text = calculateUpgradeCost(GameInfoManager.knight).ToString();
        knightLevel.text = (GameInfoManager.knight.WeaponLevel).ToString();

        archerCost.text = calculateUpgradeCost(GameInfoManager.archer).ToString();
        archerLevel.text = (GameInfoManager.archer.WeaponLevel).ToString();

        wizradCost.text = calculateUpgradeCost(GameInfoManager.wizard).ToString();
        wizardLevel.text = (GameInfoManager.wizard.WeaponLevel).ToString();
    }

    public void UpgradeWeapon(int choice)
    {
        GameCharacter gameCharacter = null;

        switch(choice)
        {
            case 1:
                gameCharacter = GameInfoManager.knight;
                break;
            case 2:
                gameCharacter = GameInfoManager.archer;
                break;
            case 3:
                gameCharacter = GameInfoManager.wizard;
                break;
        }
        int cost = calculateUpgradeCost(gameCharacter);
        bool result = GameInfoManager.ChangeMoney(-cost);
        if (result)
        {
            gameCharacter.SetWeaponLevel(gameCharacter.WeaponLevel + 1);
        }
    }
}
