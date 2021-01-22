using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelResultOverlay : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnLoaded;
    }

    private void SceneManager_sceneUnLoaded(Scene arg0)
    {
        
        VictoryPanel.instance.Panel.SetActive(false);
        LosePanel.instance.Panel.SetActive(false);
        gameObject.SetActive(false);
    }



    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var spawnPointObj = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPointObj)
        {
            gameObject.SetActive(true);
            VictoryPanel.instance.Panel.SetActive(false);
            LosePanel.instance.Panel.SetActive(false);
        }
    }
}
