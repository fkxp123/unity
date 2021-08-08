using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class BaseSpawner : MonoBehaviour
    {
        [SerializeField]
        protected GameObject prefab;
        [SerializeField]
        protected int size;

        protected PoolingObjectInfo SetPoolingObjectInfo(GameObject prefab, GameObject spawner, Vector3 position, Quaternion objectRotation)
        {
            PoolingObjectInfo info = new PoolingObjectInfo
            {
                prefab = prefab,
                spawner = spawner,
                position = position,
                objectRotation = objectRotation
            };
            return info;
        }
        #region Overload SetPoolingObjectInfo
        protected PoolingObjectInfo SetPoolingObjectInfo(GameObject prefab, GameObject spawner, Vector3 position)
        {
            PoolingObjectInfo info = new PoolingObjectInfo
            {
                prefab = prefab,
                spawner = spawner,
                position = position,
                objectRotation = spawner.transform.rotation
            };
            return info;
        }
        protected PoolingObjectInfo SetPoolingObjectInfo(GameObject prefab, GameObject spawner, Quaternion objectRotation)
        {
            PoolingObjectInfo info = new PoolingObjectInfo
            {
                prefab = prefab,
                spawner = spawner,
                position = spawner.transform.position,
                objectRotation = objectRotation
            };
            return info;
        }
        protected PoolingObjectInfo SetPoolingObjectInfo(GameObject prefab, GameObject spawner)
        {
            PoolingObjectInfo info = new PoolingObjectInfo
            {
                prefab = prefab,
                spawner = spawner,
                position = spawner.transform.position,
                objectRotation = spawner.transform.rotation
            };
            return info;
        }
        #endregion

        public void OperateSpawn(PoolingObjectInfo info, Queue<GameObject> queue, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetActivatePoolingObjectFromQueue(info, queue);
            ActivatePoolingObject(objectActivateTime, queue, poolingObject);
        }
        protected void ActivatePoolingObject(float objectActivateTime, Queue<GameObject> queue, GameObject poolingObject)
        {
            StartCoroutine(CheckActivateTimeCoroutine(objectActivateTime, queue, poolingObject));
        }
        IEnumerator CheckActivateTimeCoroutine(float objectActivateTime, Queue<GameObject> queue, GameObject poolingObject)
        {
            yield return new WaitForSeconds(objectActivateTime);
            RecyclePoolingObject(queue, poolingObject);
        }
        protected void RecyclePoolingObject(Queue<GameObject> queue, GameObject poolingObject)
        {
            ObjectPooler.instance.RecyclePoolingObjectToQueue(poolingObject, queue);
        }

        #region ActivatePoolingObject
        public void OperateDynamicSpawn(PoolingObjectInfo info)
        {
            ObjectPooler.instance.ActivateDynamicPoolingObject(info);
        }
        public void OperateStaticSpawn(PoolingObjectInfo info)
        {
            ObjectPooler.instance.ActivateStaticPoolingObject(info);
        }

        public void OperateDynamicSpawn(PoolingObjectInfo info, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetDynamicPoolingObject(info);
            ActivatePoolingObject(objectActivateTime, info, poolingObject);
        }
        public void OperateStaticSpawn(PoolingObjectInfo info, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetStaticPoolingObject(info);
            ActivatePoolingObject(objectActivateTime, info, poolingObject);
        }
        #endregion

        #region GetPoolingObject
        public GameObject GetStaticPoolingObject(PoolingObjectInfo info)
        {
            GameObject poolingObject = ObjectPooler.instance.GetStaticPoolingObject(info);
            return poolingObject;
        }
        public GameObject GetStaticPoolingObject(PoolingObjectInfo info, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetStaticPoolingObject(info);
            ActivatePoolingObject(objectActivateTime, info, poolingObject);
            return poolingObject;
        }
        public GameObject GetDynamicPoolingObject(PoolingObjectInfo info)
        {
            GameObject poolingObject = ObjectPooler.instance.GetDynamicPoolingObject(info);
            return poolingObject;
        }
        public GameObject GetDynamicPoolingObject(PoolingObjectInfo info, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetDynamicPoolingObject(info);
            ActivatePoolingObject(objectActivateTime, info, poolingObject);
            return poolingObject;
        }
        #endregion

        protected void CreatePoolingObjectQueue(PoolingObjectInfo info, int size)
        {
            ObjectPooler.instance.CreatePoolingObjects(info, size);
        }
        protected void RecyclePoolingObject(PoolingObjectInfo info, GameObject poolingObject)
        {
            ObjectPooler.instance.RecyclePoolingObject(info, poolingObject);
        }

        protected void ActivatePoolingObject(float objectActivateTime, PoolingObjectInfo info, GameObject poolingObject)
        {
            StartCoroutine(CheckActivateTimeCoroutine(objectActivateTime, info, poolingObject));
        }
        IEnumerator CheckActivateTimeCoroutine(float objectActivateTime, PoolingObjectInfo info, GameObject poolingObject)
        {
            yield return new WaitForSeconds(objectActivateTime);
            RecyclePoolingObject(info, poolingObject);
        }

        protected void SetAutoSpawn(PoolingObjectInfo info, float objectActivateTime)
        {
            StartCoroutine(CheckAutoSpawnCoroutine(info, objectActivateTime));
        }
        IEnumerator CheckAutoSpawnCoroutine(PoolingObjectInfo info, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetStaticPoolingObject(info);
            yield return new WaitForSeconds(objectActivateTime);
            RecyclePoolingObject(info, poolingObject);
            SetAutoSpawn(info, objectActivateTime);
        }
    }

}