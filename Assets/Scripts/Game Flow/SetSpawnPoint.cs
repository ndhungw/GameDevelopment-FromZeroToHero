using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.GM.spawnPoint = gameObject.transform;
    }
}
