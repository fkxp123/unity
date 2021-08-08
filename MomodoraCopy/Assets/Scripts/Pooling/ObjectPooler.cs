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

        public Queue<GameObject> GetPoolingObjectQueue(PoolingObjectInfo info, int size)
        {
            GameObject obj;
            Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                obj = Instantiate(info.prefab);
                obj.SetActive(false);
                obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
                obj.transform.SetParent(info.spawner.transform);
                poolingObjectQueue.Enqueue(obj);
            }
            return poolingObjectQueue;
        }
        public GameObject GetPoolingObjectFromQueue(PoolingObjectInfo info, Queue<GameObject> queue)
        {
            GameObject obj = null;
            if (queue.Count != 0)
            {
                obj = queue.Dequeue();
                queue.Enqueue(obj);
            }
            return obj;
        }
        public GameObject GetActivatePoolingObjectFromQueue(PoolingObjectInfo info, Queue<GameObject> queue)
        {
            GameObject obj = null;
            if(queue.Count != 0)
            {
                obj = queue.Dequeue();
                obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
                obj.transform.rotation = info.objectRotation;
                obj.SetActive(true);
                //queue.Enqueue(obj);
            }
            return obj;
        }
        public void ActivatePoolingObjectFromQueue(PoolingObjectInfo info, Queue<GameObject> queue)
        {
            GameObject obj;
            if (queue.Count != 0)
            {
                obj = queue.Dequeue();
                obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
                obj.transform.rotation = info.objectRotation;
                obj.SetActive(true);
            }
        }
        public void RecyclePoolingObjectToQueue(GameObject obj, Queue<GameObject> queue)
        {
            if (obj == null)
            {
                return;
            }
            if (obj.transform.GetChild(0) != null)
            {
                obj.transform.GetChild(0).gameObject.SetActive(true);
            }
            obj.SetActive(false);
            queue.Enqueue(obj);
        }

        public void CreatePoolingObjects(PoolingObjectInfo info, int size)
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
        public void ClearPoolingObjects(PoolingObjectInfo info)
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
        public GameObject GetStaticPoolingObject(PoolingObjectInfo info)
        {
            GameObject obj;
            if (!poolDictionary.ContainsKey(info.prefab) || poolDictionary[info.prefab].Count == 0)
            {
                return null;
            }
            obj = poolDictionary[info.prefab].Dequeue();
            obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
            obj.transform.rotation = info.objectRotation;
            obj.gameObject.SetActive(true);
            return obj;
        }
        public GameObject GetDynamicPoolingObject(PoolingObjectInfo info)
        {
            GameObject obj;
            if (!poolDictionary.ContainsKey(info.prefab) || poolDictionary[info.prefab].Count == 0)
            {
                CreatePoolingObject(info);
            }
            obj = poolDictionary[info.prefab].Dequeue();
            obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
            obj.transform.rotation = info.objectRotation;
            obj.gameObject.SetActive(true);
            return obj;
        }
        public GameObject GetStaticPoolingObject(GameObject prefab)
        {
            GameObject obj;
            if (!poolDictionary.ContainsKey(prefab) || poolDictionary[prefab].Count == 0)
            {
                return null;
            }
            obj = poolDictionary[prefab].Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }


        public void ActivateDynamicPoolingObject(PoolingObjectInfo info)
        {
            GameObject obj;
            if (!poolDictionary.ContainsKey(info.prefab) || poolDictionary[info.prefab].Count == 0)
            {
                CreatePoolingObject(info);
            }
            obj = poolDictionary[info.prefab].Dequeue();
            obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
            obj.transform.rotation = info.objectRotation;
            obj.gameObject.SetActive(true);
        }
        public void ActivateStaticPoolingObject(PoolingObjectInfo info)
        {
            GameObject obj;
            if (!poolDictionary.ContainsKey(info.prefab) || poolDictionary[info.prefab].Count == 0)
            {
                return;
            }
            obj = poolDictionary[info.prefab].Dequeue();
            obj.transform.position = new Vector3(info.position.x, info.position.y, Random.Range(0.0f, 1.0f));
            obj.transform.rotation = info.objectRotation;
            obj.gameObject.SetActive(true);
        }

        public void RecyclePoolingObject(PoolingObjectInfo info, GameObject clone)
        {
            if(clone == null)
            {
                return;
            }
            if(clone.transform.GetChild(0) != null)
            {
                clone.transform.GetChild(0).gameObject.SetActive(true);
            }
            clone.SetActive(false);
            clone.transform.SetParent(info.spawner.transform);
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