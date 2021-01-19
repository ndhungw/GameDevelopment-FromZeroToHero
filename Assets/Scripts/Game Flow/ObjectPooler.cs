using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public string poolName;
        public GameObject prefab;
        public int poolSize;
    }

    [SerializeField]
    private List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;

    public static ObjectPooler GameSceneGlobal_ObjectPoolingEntity;

    private void Awake()
    {
        if (GameSceneGlobal_ObjectPoolingEntity == null)
        {
            GameSceneGlobal_ObjectPoolingEntity = GameObject.FindGameObjectWithTag("GlobalObjectPool").GetComponent<ObjectPooler>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(var pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for(int i = 0; i < pool.poolSize; i++)
            {
                GameObject tempObj = Instantiate(pool.prefab);
                tempObj.SetActive(false);
                objectQueue.Enqueue(tempObj);
            }

            poolDictionary.Add(pool.poolName, objectQueue);
        }
    }

    public GameObject spawnFromPool(string poolName)
    {
        if (poolDictionary.ContainsKey(poolName))
        {
            GameObject newObj = poolDictionary[poolName].Peek();
            if (newObj)
            {
                if (newObj.activeInHierarchy) return null;
                newObj = poolDictionary[poolName].Dequeue();
                poolDictionary[poolName].Enqueue(newObj);
            }
            return newObj;
        }

        return null;
    }
}
