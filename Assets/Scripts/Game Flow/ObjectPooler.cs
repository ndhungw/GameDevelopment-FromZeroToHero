using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            var gameObj = GameObject.FindGameObjectWithTag("GlobalObjectPool");
            GameSceneGlobal_ObjectPoolingEntity = gameObj.GetComponent<ObjectPooler>();
            DontDestroyOnLoad(gameObj);
        }
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            poolDictionary.Add(pool.poolName, objectQueue);
        }
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        foreach (var pool in pools)
        {
            var objectQueue = poolDictionary[pool.poolName];

            for (int i = 0; i < pool.poolSize; i++)
            {
                objectQueue.Clear();
            }
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        foreach (var pool in pools)
        {
            var objectQueue = poolDictionary[pool.poolName];

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject tempObj = Instantiate(pool.prefab);
                tempObj.SetActive(false);
                objectQueue.Enqueue(tempObj);
            }
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
