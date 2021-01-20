using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayLevel(string mapName)
    {
        SceneManager.LoadScene(mapName);
    }
}
