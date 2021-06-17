using Assets.Scripts.Game_System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampItem : MonoBehaviour
{
    [SerializeField]
    private Image characterAvatar;
    [SerializeField]
    private Image Mask;
    [SerializeField]
    private TextMeshProUGUI healthText;

    private GameInfoManager.GAME_CHARACTER character;

    public void SetGameCharacter(GameInfoManager.GAME_CHARACTER gameCharacter)
    {
        character = gameCharacter;
    }

    private void OnEnable()
    {
        
    }
}
