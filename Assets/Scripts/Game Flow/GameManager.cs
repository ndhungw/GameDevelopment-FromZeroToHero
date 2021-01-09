using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    private void Awake()
    {
        if (GM == null)
        {
            GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>(); 
        }
    }
}
