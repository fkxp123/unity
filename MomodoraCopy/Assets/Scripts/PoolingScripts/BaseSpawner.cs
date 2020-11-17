using System.Collections;
using UnityEngine;

namespace MomodoraCopy
{
    public class BaseSpawner : MonoBehaviour
    {
        protected PoolingObjectInfo SetPoolingObjectInfo(GameObject prefab, GameObject spawner, float objectRotateZ)
        {
            PoolingObjectInfo info = new PoolingObjectInfo
            {
                prefab = prefab,
                spawner = spawner,
                objectRotateZ = objectRotateZ
            };
            return info;
        }

        public void OperateSpawn(PoolingObjectInfo info, float objectRotateZ, float objectActivateTime)
        {
            info.objectRotateZ = objectRotateZ;
            GameObject poolingObject = ObjectPooler.instance.GetPoolingObject(info);
            ActivatePoolingObject(objectActivateTime, info.prefab, poolingObject);
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
        protected void RecyclePoolingObject(GameObject prefab, GameObject poolingObject)
        {
            ObjectPooler.instance.RecyclePoolingObject(prefab, poolingObject);
        }

        protected void ActivatePoolingObject(float objectActivateTime, GameObject prefab, GameObject poolingObject)
        {
            StartCoroutine(CheckActivateTimeCoroutine(objectActivateTime, prefab, poolingObject));
        }
        IEnumerator CheckActivateTimeCoroutine(float objectActivateTime, GameObject prefab, GameObject poolingObject)
        {
            yield return new WaitForSeconds(objectActivateTime);
            RecyclePoolingObject(prefab, poolingObject);
        }

        protected void SetAutoSpawn(PoolingObjectInfo info, float objectActivateTime)
        {
            StartCoroutine(CheckAutoSpawnCoroutine(info, objectActivateTime));
        }
        IEnumerator CheckAutoSpawnCoroutine(PoolingObjectInfo info, float objectActivateTime)
        {
            GameObject poolingObject = ObjectPooler.instance.GetPoolingObject(info);
            yield return new WaitForSeconds(objectActivateTime);
            RecyclePoolingObject(info.prefab, poolingObject);
            SetAutoSpawn(info, objectActivateTime);
        }

        protected void SetAutoSpawn(PoolingObjectInfo info, float objectRotateZ, float objectActivateTime)
        {
            StartCoroutine(CheckAutoSpawnCoroutine(info, objectRotateZ, objectActivateTime));
        }
        IEnumerator CheckAutoSpawnCoroutine(PoolingObjectInfo info, float objectRotateZ, float objectActivateTime)
        {
            info.objectRotateZ = objectRotateZ;
            GameObject poolingObject = ObjectPooler.instance.GetPoolingObject(info);
            yield return new WaitForSeconds(objectActivateTime);
            RecyclePoolingObject(info.prefab, poolingObject);
            SetAutoSpawn(info, objectRotateZ, objectActivateTime);
        }
    }

}