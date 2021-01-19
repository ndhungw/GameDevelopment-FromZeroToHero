using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnPoint : MonoBehaviour
{
    private bool isWaitingForGameplayManagerToLoad = false;

    // Start is called before the first frame update
    void Start()
<<<<<<< Updated upstream
=======
    {
        if (GameplayManager.GM)
        {
            GameplayManager.GM.spawnPoint = gameObject.transform;
        }
        else
        {
            isWaitingForGameplayManagerToLoad = true;
        }
    }

    private void Update()
>>>>>>> Stashed changes
    {
        if (isWaitingForGameplayManagerToLoad)
        {
            if (GameplayManager.GM)
            {
                GameplayManager.GM.spawnPoint = gameObject.transform;
                isWaitingForGameplayManagerToLoad = false;
            }
        }
    }

    private void OnDestroy()
    {
        GameplayManager.GM.spawnPoint = null;
    }
}
