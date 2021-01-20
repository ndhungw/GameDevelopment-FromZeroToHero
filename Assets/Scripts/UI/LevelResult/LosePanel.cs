using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LosePanel : MonoBehaviour
{
    public static LosePanel instance;

    public GameObject Panel;

    void Awake()
    {
        instance = this;
    }

    public void ReturnToVillage()
    {
        SceneManager.LoadScene("Village");
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
