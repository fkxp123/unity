﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;


namespace MomodoraCopy
{
    [System.Serializable]
    public class PlayerData
    {
        public Vector3 playerPosition;
        public string sceneName;
        public bool isDead;
    }

    public class GameManager : Singleton<GameManager>
    {
        public PlayerData playerData;
        public GameObject playerObject;
        public GameObject mainCameraObject;
        GameObject[] checkPoints;
        Scene scene;

        MonoBehaviour[] cameraComponents;
        MonoBehaviour[] playerComponents;

        public Dictionary<int, List<GameObject>> checkPointsDict = new Dictionary<int, List<GameObject>>();
        public List<GameObject> checkPointsList = new List<GameObject>();

        public override void Awake()
        {
            base.Awake();
            scene = SceneManager.GetActiveScene();
        }

        public void Start()
        {

            if (playerObject == null || playerObject.tag != "Player")
            {
                playerObject = GameObject.FindGameObjectWithTag("Player");
            }
            if(checkPoints == null)
            {
                checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
            }
            playerComponents = playerObject.GetComponents<MonoBehaviour>();
            cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();
        }

        //public void AddCheckPoint(string sceneName, GameObject checkPoint)
        //{
        //    int hash = sceneName.GetHashCode();
        //    checkPointsList.Add(checkPoint);
        //    if (checkPointsDict.ContainsKey(hash))
        //    {
        //        checkPointsDict.Remove(hash);
        //    }
        //    checkPointsDict.Add(hash, checkPointsList);
        //}


        public void Resume()
        {
            Time.timeScale = 1f;
            foreach (MonoBehaviour component in playerComponents)
            {
                component.enabled = true;
            }
            foreach (MonoBehaviour component in cameraComponents)
            {
                component.enabled = true;
            }
        }
        public void Pause()
        {
            Time.timeScale = 0f;
            foreach (MonoBehaviour component in playerComponents)
            {
                component.enabled = false;
            }
            foreach (MonoBehaviour component in cameraComponents)
            {
                component.enabled = false;
            }
        }
        public void Save(GameObject checkPoint)
        {
            playerData.playerPosition = checkPoint.transform.position;
            playerData.sceneName = checkPoint.GetComponent<CheckPoint>().sceneName;
            string jsonData = JsonUtility.ToJson(playerData, true);
            string path = Path.Combine(Application.dataPath, "playerData.json");
            File.WriteAllText(path, jsonData);
        }
        public void Load()
        {
            Debug.Log("load!!!!!");
            if (File.Exists(Application.dataPath + "/playerData.json"))
            {
                Debug.Log("Load by playerData");
                string path = Path.Combine(Application.dataPath, "playerData.json");
                string jsonData = File.ReadAllText(path);
                playerData = JsonUtility.FromJson<PlayerData>(jsonData);
                SceneManager.LoadScene(playerData.sceneName);
                playerObject.transform.position = playerData.playerPosition;
            }
            else
            {
                SceneManager.LoadScene("test");
                playerObject.transform.position = CheckPointsData.checkPointsDict[scene.name.GetHashCode()][0].transform.position;
            }
        }
    }

}
