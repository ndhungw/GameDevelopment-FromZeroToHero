using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuildMenu : MonoBehaviour
{
    public Image knight;
    public Image archer;
    public Image wizard;

    private 
    // Start is called before the first frame update
    void Start()
    {
        knight.GetComponent<GuildItem>().SetGameCharacter(GameInfoManager.knight);
        archer.GetComponent<GuildItem>().SetGameCharacter(GameInfoManager.archer);
        wizard.GetComponent<GuildItem>().SetGameCharacter(GameInfoManager.wizard);
    }

    // Update is called once per frame
    void Update()
    {
        knight.GetComponent<GuildItem>().SetGameCharacter(GameInfoManager.knight);
        archer.GetComponent<GuildItem>().SetGameCharacter(GameInfoManager.archer);
        wizard.GetComponent<GuildItem>().SetGameCharacter(GameInfoManager.wizard);
    }
}
