using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public List<Pool> pools;    //list of pools

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // we have list of pools, adding them to the dictionary
        createPool();
    }

    private void createPool()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            GameObject poolHolder = new GameObject(pool.name + "pool");
            poolHolder.transform.parent = transform;

            int poolSize = pool.amount;
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                obj.transform.parent = poolHolder.transform;
            }
            poolDictionary.Add(pool.name, objectPool);
        }
    }

    public GameObject spawnFromPool(string name, Vector3 position, Quaternion rotation) {
        // if object doesnt exist you can instantiate on the else part and return the object which is instantiated
        // currently method is void and dont instantiate an object. You can change it if you want. (maybe I will too)
        // dont remember to return spawnObject and method signiture

        if (poolDictionary.ContainsKey(name))
        {
            GameObject spawnObject = poolDictionary[name].Dequeue();
            spawnObject.transform.position = position;
            spawnObject.transform.rotation = rotation;
            spawnObject.SetActive(true);

            poolDictionary[name].Enqueue(spawnObject);

            resetObjectProperties(spawnObject);

            return spawnObject;
        }
        else Debug.Log("pool name doesnt exist" + name); return null;       // you can instantiate GameObject name, pos,rot and return it
    }

    void resetObjectProperties(GameObject spawnObject) {
        IPooledObject pooledObject = spawnObject.GetComponent<IPooledObject>();
        if (pooledObject != null)
        {
            pooledObject.onObjectSpawn();
        }
    }

    [System.Serializable]
    public class Pool {

        public string name;
        public GameObject prefab;
        public int amount;
    }
}
