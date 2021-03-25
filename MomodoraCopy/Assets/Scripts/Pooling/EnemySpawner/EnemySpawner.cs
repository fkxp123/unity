using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class EnemySpawner : BaseSpawner
    {
        protected PoolingObjectInfo[] infos;
        protected GameObject[] recycleObjects;
        protected int spawnPositionCount;

        [SerializeField]
        [Tooltip("If check this variable, spawn enemies when the player is within the set vector range.")]
        protected bool isTriggerSpawner;
        [SerializeField]
        protected Vector2 findPlayerBoxSize;
        [SerializeField]
        [Tooltip("If check this variable, spawn enemies periodically.")]
        protected bool isUpdateSpawner;
        [SerializeField]
        protected float spawnTime = 1.0f;
        protected float currentTime;
        protected int spawnIndex = 0;

        bool doneSpawn;

        void Start()
        {
            spawnPositionCount = transform.childCount;
            infos = new PoolingObjectInfo[spawnPositionCount];
            recycleObjects = new GameObject[spawnPositionCount];

            for(int i = 0; i < spawnPositionCount; i++)
            {
                infos[i] = new PoolingObjectInfo
                {
                    prefab = prefab,
                    spawner = gameObject,
                    position = transform.GetChild(i).transform.position,
                    objectRotation = transform.rotation
                };
                ObjectPooler.instance.SetPoolingObject(infos[i]);
                if(!isTriggerSpawner && !isUpdateSpawner)
                {
                    recycleObjects[i] = ObjectPooler.instance.GetPoolingObject(infos[i]);
                }
            }

            //StartCoroutine("CheckSomething");
        }
        IEnumerator CheckSomething()
        {
            while (enabled)
            {
                Check();
                yield return new WaitForSeconds(0.1f);
            }
        }
        void Test()
        {
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position, findPlayerBoxSize, 0);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider.tag == "Player")
                {
                    Debug.Log("find Player in coroutine : " + Time.time);
                }
            }
        }


        void Check()
        {
            if (spawnIndex >= spawnPositionCount)
            {
                return;
            }
            if (isTriggerSpawner)
            {
                bool flag = false;
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position, findPlayerBoxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Player")
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    return;
                }
                if (!isUpdateSpawner)
                {
                    if (doneSpawn)
                    {
                        return;
                    }
                    for (int i = 0; i < spawnPositionCount; i++)
                    {
                        recycleObjects[i] = ObjectPooler.instance.GetPoolingObject(infos[i], false);
                    }
                    doneSpawn = true;
                    return;
                }
            }
            if (isUpdateSpawner)
            {
                if (currentTime >= 0)
                {
                    currentTime -= Time.deltaTime;
                    return;
                }
                currentTime = spawnTime;
                recycleObjects[spawnIndex] = ObjectPooler.instance.GetPoolingObject(infos[spawnIndex], false);
                spawnIndex++;
            }
        }

        void Update()
        {
            Check();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, findPlayerBoxSize);
        }
    }
}
