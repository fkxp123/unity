﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace MomodoraCopy
{
    public class ArrowSpawner : BaseSpawner
    {
        [HideInInspector]
        public Quaternion arrowRotation;
        public const float ACTIVATE_TIME = 3.0f;
        [HideInInspector]
        public PoolingObjectInfo info;

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            int childs = transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            info = SetPoolingObjectInfo(prefab, gameObject, gameObject.transform.position, transform.rotation);
            CreatePoolingObjectQueue(info, 10);
            Debug.Log("ppap:" + ObjectPooler.instance.poolDictionary[info.prefab].Count);
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}