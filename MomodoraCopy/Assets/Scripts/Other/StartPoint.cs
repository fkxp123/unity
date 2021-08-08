﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class StartPoint : MonoBehaviour
    {
        public string startSceneName;
        public Tilemap startTilemap;
        public bool isStartingLeft;

        void Start()
        {
            if (!File.Exists(Application.dataPath + "/playerData.json") ||
                startSceneName != GameManager.instance.currentScene)
            {
                MapManager.instance.SetCurrentTilemap(startTilemap);
                GameManager.instance.playerPhysics.transform.position = transform.position;
                if (isStartingLeft)
                {
                    GameManager.instance.playerPhysics.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                GameManager.instance.currentScene = startSceneName;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.75f);
        }
    }
}