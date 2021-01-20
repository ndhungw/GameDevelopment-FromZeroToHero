using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampSystem : MonoBehaviour
{
    public GameObject CampUI;
    public GameObject CampObj;

    public void Awake()
    {
        if (CampUI)
        {
            CampUI.SetActive(false);
        }
    }

    public void displayCampUI()
    {
        if (!CampUI.activeInHierarchy)
        {
            CampUI.SetActive(true);
        }
        //fetch data from game info manager here
    }
}
