using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameplayManager.GM.spawnPoint = gameObject.transform;
    }

    private void OnDestroy()
    {
        GameplayManager.GM.spawnPoint = null;
    }
}
