using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingObjectInfo 
{
    public GameObject prefab;
    public GameObject spawner;
    public float objectRotateY;
}

public class ObjectPooler : MonoBehaviour
{
    #region Singleton
    public static ObjectPooler instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }
    #endregion

    public Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    public void CreatePoolingObjectQueue(PoolingObjectInfo info, int size) 
    {
        GameObject obj;
        Queue<GameObject> poolingObjectQueue;

        if (!poolDictionary.ContainsKey(info.prefab))
        {
            poolingObjectQueue = new Queue<GameObject>();
            for (int i = 0; i < size; i++)
            {
                obj = Instantiate(info.prefab);
                obj.SetActive(false);
                obj.transform.position = info.spawner.transform.position;
                poolingObjectQueue.Enqueue(obj);
            }
            poolDictionary.Add(info.prefab, poolingObjectQueue);
            return;
        }

        poolingObjectQueue = poolDictionary[info.prefab];
        for (int i = 0; i < size; i++)
        {
            obj = Instantiate(info.prefab);
            obj.SetActive(false);
            obj.transform.position = info.spawner.transform.position;
            poolingObjectQueue.Enqueue(obj);
        }
        poolDictionary[info.prefab] = poolingObjectQueue;
    }
    private void CreatePoolingObject(PoolingObjectInfo info) 
    {
        GameObject obj;
        Queue<GameObject> poolingObjectQueue;
        if (poolDictionary.ContainsKey(info.prefab))
        {
            poolingObjectQueue = poolDictionary[info.prefab];
            obj = Instantiate(info.prefab);
            obj.gameObject.SetActive(false);
            obj.transform.position = info.spawner.transform.position;
            poolingObjectQueue.Enqueue(obj);
            poolDictionary[info.prefab] = poolingObjectQueue;
            return;
        }
        poolingObjectQueue = new Queue<GameObject>();
        obj = Instantiate(info.prefab);
        obj.gameObject.SetActive(false);
        obj.transform.position = info.spawner.transform.position;
        poolingObjectQueue.Enqueue(obj);
        poolDictionary.Add(info.prefab, poolingObjectQueue);
    }
    public GameObject GetPoolingObject(PoolingObjectInfo info) 
    {
        GameObject obj;
        if (!poolDictionary.ContainsKey(info.prefab) || poolDictionary[info.prefab].Count == 0)
        {
            CreatePoolingObject(info);

            obj = poolDictionary[info.prefab].Dequeue();
            obj.gameObject.SetActive(true);
            obj.transform.position = info.spawner.transform.position;
            obj.transform.rotation = Quaternion.Euler(0, info.objectRotateY, 0);
            return obj;
        }
        obj = poolDictionary[info.prefab].Dequeue();
        obj.gameObject.SetActive(true);
        obj.transform.position = info.spawner.transform.position;
        obj.transform.rotation = Quaternion.Euler(0, info.objectRotateY, 0);
        return obj;
    }
    public void RecyclePoolingObject(GameObject prefab, GameObject clone)
    {
        clone.SetActive(false);
        if (!poolDictionary.ContainsKey(prefab))
        {
            Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
            poolingObjectQueue.Enqueue(clone);
            poolDictionary.Add(prefab, poolingObjectQueue);
            return;
        }
        poolDictionary[prefab].Enqueue(clone);
    }
}