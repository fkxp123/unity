using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        public Tilemap targetTilemap;

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
                    recycleObjects[i] = ObjectPooler.instance.GetStaticPoolingObject(infos[i]);
                }
            }

            //MapManager.instance.enemySpawnerDict.Add(targetTilemap, this);
            MapManager.instance.AddEnemySpawnerData(targetTilemap, this);
        }

        public void SpawnEnemy()
        {
            for (int i = 0; i < spawnPositionCount; i++)
            {
                recycleObjects[i] = ObjectPooler.instance.GetStaticPoolingObject(infos[i]);
            }
        }

        public void RecycleEnemy()
        {
            for (int i = 0; i < spawnPositionCount; i++)
            {
                RecyclePoolingObject(infos[i], recycleObjects[i]);
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
                        recycleObjects[i] = ObjectPooler.instance.GetDynamicPoolingObject(infos[i]);
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
                recycleObjects[spawnIndex] = ObjectPooler.instance.GetDynamicPoolingObject(infos[spawnIndex]);
                spawnIndex++;
            }
        }

        void Update()
        {
            if(targetTilemap != null)
            {
                return;
            }
            Check();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, findPlayerBoxSize);
        }
    }
}
