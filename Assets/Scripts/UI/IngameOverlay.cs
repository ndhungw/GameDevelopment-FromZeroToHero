using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameOverlay : MonoBehaviour
{
    bool isHidden = false;
    bool sceneHasSpawnpoint = false;
    
    private void Awake()
    {
        gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var spawnPointObj = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPointObj)
        {
            gameObject.SetActive(true);
        }
    }
}
