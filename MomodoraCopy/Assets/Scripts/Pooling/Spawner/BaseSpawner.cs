using System.Collections;
using UnityEngine;

namespace MomodoraCopy
{
    public class BaseSpawner : MonoBehaviour
    {
        [SerializeField]
        protected GameObject prefab;

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

        public void OperateSpawn(PoolingObjectInfo info)
        {
            GameObject poolingObject = ObjectPooler.instance.GetPoolingObject(info);
        }

        public void OperateSpawn(PoolingObjectInfo info, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetPoolingObject(info);
            ActivatePoolingObject(objectActivateTime, info, poolingObject);
        }

        protected GameObject GetPoolingObject(PoolingObjectInfo info)
        {
            GameObject poolingObject = ObjectPooler.instance.GetPoolingObject(info);
            return poolingObject;
        }
        protected void CreatePoolingObjectQueue(PoolingObjectInfo info, int size)
        {
            ObjectPooler.instance.CreatePoolingObjectQueue(info, size);
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
            GameObject poolingObject = ObjectPooler.instance.GetPoolingObject(info);
            yield return new WaitForSeconds(objectActivateTime);
            RecyclePoolingObject(info, poolingObject);
            SetAutoSpawn(info, objectActivateTime);
        }
    }

}