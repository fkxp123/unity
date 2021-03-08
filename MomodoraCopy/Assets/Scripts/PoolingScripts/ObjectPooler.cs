using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class ObjectPooler : Singleton<ObjectPooler>
    {
        [SerializeField]
        public Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        public void SetPoolingObject(PoolingObjectInfo info)
        {
            GameObject obj;
            Queue<GameObject> poolingObjectQueue;

            if (poolDictionary.ContainsKey(info.prefab))
            {
                poolingObjectQueue = poolDictionary[info.prefab];
                poolDictionary.Remove(info.prefab);
            }
            else
            {
                poolingObjectQueue = new Queue<GameObject>();
            }

            obj = Instantiate(info.prefab);
            obj.SetActive(false);
            obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
            obj.transform.SetParent(info.spawner.transform);
            poolingObjectQueue.Enqueue(obj);

            poolDictionary.Add(info.prefab, poolingObjectQueue);
        }
        public void SetActiveTruePoolingObjects(PoolingObjectInfo info)
        {
            if (!poolDictionary.ContainsKey(info.prefab))
            {
                return;
            }
            foreach (GameObject obj in poolDictionary[info.prefab])
            {
                obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
                obj.transform.rotation = info.objectRotation;
                obj.SetActive(true);
            }
        }
        public void SetActiveFalsePoolingObjects(PoolingObjectInfo info)
        {
            if (!poolDictionary.ContainsKey(info.prefab))
            {
                return;
            }
            foreach (GameObject obj in poolDictionary[info.prefab])
            {
                obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
                obj.transform.rotation = info.objectRotation;
                obj.SetActive(false);
            }
        }
        #region Overload SetActiveTrue&FalsePoolingObjects Function
        public void SetActiveTruePoolingObjects(GameObject prefab)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                Debug.Log("hello");
                return;
            }
            foreach (GameObject obj in poolDictionary[prefab])
            {
                obj.SetActive(true);
            }
        }
        public void SetActiveFalsePoolingObjects(GameObject prefab)
        {
            if (!poolDictionary.ContainsKey(prefab))
            {
                return;
            }
            Debug.Log(poolDictionary[prefab].Count);
            foreach (GameObject obj in poolDictionary[prefab])
            {
                obj.SetActive(false);
            }
        }
        #endregion

        public void CreatePoolingObjectQueue(PoolingObjectInfo info, int size)
        {
            GameObject obj;
            Queue<GameObject> poolingObjectQueue;

            if (poolDictionary.ContainsKey(info.prefab))
            {
                poolingObjectQueue = poolDictionary[info.prefab];
                poolDictionary.Remove(info.prefab);
            }
            else
            {
                poolingObjectQueue = new Queue<GameObject>();
            }

            for (int i = 0; i < size; i++)
            {
                obj = Instantiate(info.prefab);
                obj.SetActive(false);
                obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
                obj.transform.SetParent(info.spawner.transform);
                poolingObjectQueue.Enqueue(obj);
            }
            poolDictionary.Add(info.prefab, poolingObjectQueue);
        }
        public void ClearPoolingObjectQueue(PoolingObjectInfo info)
        {
            if (poolDictionary.ContainsKey(info.prefab))
            {
                Queue<GameObject> poolingObjectQueue;
                poolingObjectQueue = poolDictionary[info.prefab];
                poolingObjectQueue.Clear();
            }
        }

        private void CreatePoolingObject(PoolingObjectInfo info)
        {
            GameObject obj;
            Queue<GameObject> poolingObjectQueue;
            if (poolDictionary.ContainsKey(info.prefab))
            {
                poolingObjectQueue = poolDictionary[info.prefab];
                poolDictionary.Remove(info.prefab);
            }
            else
            {
                poolingObjectQueue = new Queue<GameObject>();
            }

            obj = Instantiate(info.prefab);
            obj.gameObject.SetActive(false);
            obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
            obj.transform.SetParent(info.spawner.transform);

            poolingObjectQueue.Enqueue(obj);
            poolDictionary.Add(info.prefab, poolingObjectQueue);
        }
        public GameObject GetPoolingObject(PoolingObjectInfo info, bool ignoreQueue = true)
        {
            GameObject obj;
            if (!poolDictionary.ContainsKey(info.prefab) || poolDictionary[info.prefab].Count == 0)
            {
                if (!ignoreQueue)
                {
                    return null;
                }
                CreatePoolingObject(info);
            }
            obj = poolDictionary[info.prefab].Dequeue();
            obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
            obj.gameObject.SetActive(true);
            obj.transform.rotation = info.objectRotation;
            return obj;
        }

        public void RecyclePoolingObject(PoolingObjectInfo info, GameObject clone)
        {
            if(clone == null)
            {
                return;
            }
            clone.transform.SetParent(info.spawner.transform);
            clone.SetActive(false);
            if (!poolDictionary.ContainsKey(info.prefab))
            {
                Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
                poolingObjectQueue.Enqueue(clone);
                poolDictionary.Add(info.prefab, poolingObjectQueue);
                return;
            }
            poolDictionary[info.prefab].Enqueue(clone);
        }
    }
}