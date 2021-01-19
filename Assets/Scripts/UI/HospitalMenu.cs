using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HospitalMenu : MonoBehaviour
{
    public Image knight;
    public Image archer;
    public Image wizard;

    private
    // Start is called before the first frame update
    void Start()
    {
        knight.GetComponent<HospitalItem>().SetGameCharacter(GameInfoManager.knight);
        archer.GetComponent<HospitalItem>().SetGameCharacter(GameInfoManager.archer);
        wizard.GetComponent<HospitalItem>().SetGameCharacter(GameInfoManager.wizard);
    }

    // Update is called once per frame
    void Update()
    {
        knight.GetComponent<HospitalItem>().SetGameCharacter(GameInfoManager.knight);
        archer.GetComponent<HospitalItem>().SetGameCharacter(GameInfoManager.archer);
        wizard.GetComponent<HospitalItem>().SetGameCharacter(GameInfoManager.wizard);
    }
}
