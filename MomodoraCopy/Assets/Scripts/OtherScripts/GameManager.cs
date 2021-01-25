using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MomodoraCopy
{
    [System.Serializable]
    public class PlayerData
    {
        public Vector3 playerPosition;
        public bool isDead;
    }

    public class GameManager : Singleton<GameManager>
    {
        public PlayerData playerData;
        public GameObject playerObject;
        public GameObject mainCameraObject;
        MonoBehaviour[] cameraComponents;
        MonoBehaviour[] playerComponents;
        public void Start()
        {
            if (playerObject == null || playerObject.tag != "Player")
            {
                playerObject = GameObject.FindGameObjectWithTag("Player");
            }
            playerComponents = playerObject.GetComponents<MonoBehaviour>();
            cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();
        }

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
        public void Save()
        {
            playerData.playerPosition = GameManager.instance.playerObject.transform.position;
            string jsonData = JsonUtility.ToJson(playerData);
            string path = Path.Combine(Application.dataPath, "playerData.json");
            File.WriteAllText(path, jsonData);
        }
        public void Load()
        {
            if (File.Exists(Application.dataPath + "/playerData.json"))
            {
                string path = Path.Combine(Application.dataPath, "playerData.json");
                string jsonData = File.ReadAllText(path);
                playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            }
            playerObject.transform.position = playerData.playerPosition;
        }
    }

}
