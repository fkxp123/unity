using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class MapManager : Singleton<MapManager>
    {
        public Dictionary<Tilemap, EnemySpawner> enemySpawnerDict = new Dictionary<Tilemap, EnemySpawner>();
        public Dictionary<Tilemap, GameObject[]> mapLinkDict = new Dictionary<Tilemap, GameObject[]>();

        [SerializeField]
        Tilemap currentTilemap;
        Tilemap CurrentTilemap
        {
            get
            {
                return currentTilemap;
            }
            set
            {
                if(currentTilemap == null)
                {
                    if (mapLinkDict.ContainsKey(value))
                    {
                        for (int i = 0; i < mapLinkDict[value].Length; i++)
                        {
                            mapLinkDict[value][i].SetActive(true);
                        }
                    }
                }
                else if (mapLinkDict.ContainsKey(currentTilemap))
                {
                    for (int i = 0; i < mapLinkDict[value].Length; i++)
                    {
                        bool noMatch = false;
                        for (int j = 0; j < mapLinkDict[currentTilemap].Length; j++)
                        {
                            if (mapLinkDict[value][i] == mapLinkDict[currentTilemap][j])
                            {
                                noMatch = true;
                            }
                        }
                        if (!noMatch)
                        {
                            mapLinkDict[value][i].SetActive(true);
                        }
                    }
                    for (int i = 0; i < mapLinkDict[currentTilemap].Length; i++)
                    {
                        bool noMatch = false;
                        for (int j = 0; j < mapLinkDict[value].Length; j++)
                        {
                            if (mapLinkDict[currentTilemap][i].name == mapLinkDict[value][j].name)
                            {
                                noMatch = true;
                            }
                        }
                        if (!noMatch)
                        {
                            mapLinkDict[currentTilemap][i].SetActive(false);
                        }
                    }
                }
                if (currentTilemap != null)
                {
                    if (enemySpawnerDict.ContainsKey(currentTilemap))
                    {
                        enemySpawnerDict[currentTilemap].RecycleEnemy();
                    }
                }
                CameraManager.instance.SetCameraBounds(value);
                if(currentTilemap != null)
                {
                    GameManager.instance.OperateSimpleFadeIn();
                }
                if (enemySpawnerDict.ContainsKey(value))
                {
                    enemySpawnerDict[value].SpawnEnemy();
                }

                currentTilemap = value;
            }
        }

        public void SetCurrentTilemap(Tilemap mainTilemap)
        {
            CurrentTilemap = mainTilemap;
        }

        public void AddMapLinkData(Tilemap tilemap, GameObject[] objs)
        {
            foreach (Tilemap key in mapLinkDict.Keys.ToList())
            {
                if (key.Equals(null))
                {
                    mapLinkDict.Remove(key);
                }
            }
            mapLinkDict.Add(tilemap, objs);
        }
        public void AddEnemySpawnerData(Tilemap tilemap, EnemySpawner spawner)
        {
            foreach (Tilemap key in enemySpawnerDict.Keys.ToList())
            {
                if (key.Equals(null))
                {
                    enemySpawnerDict.Remove(key);
                }
            }
            enemySpawnerDict.Add(tilemap, spawner);
        }
    }

}