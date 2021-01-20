using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampSystem : MonoBehaviour
{
    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void displayCampUI()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        //fetch data from game info manager here
    }
}
