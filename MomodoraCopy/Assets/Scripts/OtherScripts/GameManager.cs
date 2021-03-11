using System.Collections;
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
        public GameObject playerPhysics;
        public GameObject playerSprite;
        public GameObject mainCameraObject;
        GameObject[] checkPoints;
        Scene scene;
        public string currentScene;

        MonoBehaviour[] cameraComponents;
        MonoBehaviour[] playerPhysicsComponents;
        MonoBehaviour[] playerSpriteComponents;

        public Dictionary<int, List<GameObject>> checkPointsDict = new Dictionary<int, List<GameObject>>();
        public List<GameObject> checkPointsList = new List<GameObject>();

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (playerPhysics == null || !playerPhysics.CompareTag("Player"))
            {
                playerPhysics = GameObject.FindGameObjectWithTag("Player");
                playerSprite = playerPhysics.transform.GetChild(1).gameObject;
            }
            if (mainCameraObject == null || !mainCameraObject.CompareTag("MainCamera"))
            {
                mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            }
            if (checkPoints == null)
            {
                checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
            }
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public override void Awake()
        {
            base.Awake();
            scene = SceneManager.GetActiveScene();
            currentScene = scene.name;
            if (playerPhysics == null || !playerPhysics.CompareTag("Player"))
            {
                playerPhysics = GameObject.FindGameObjectWithTag("Player");
                playerSprite = playerPhysics.transform.GetChild(1).gameObject;
            }
            if (mainCameraObject == null || !mainCameraObject.CompareTag("MainCamera"))
            {
                mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            }
            if (checkPoints == null)
            {
                checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
            }
        }
        public void Start()
        {
            playerPhysicsComponents = playerPhysics.GetComponents<MonoBehaviour>();
            playerSpriteComponents = playerSprite.GetComponents<MonoBehaviour>();
            cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();
        }

        //void SetSceneHashList()
        //{
        //    int sceneCount = SceneManager.sceneCountInBuildSettings;
        //    for (int i = 0; i < sceneCount; i++)
        //    {
        //        sceneHashList.Add(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).GetHashCode());
        //    }
        //}

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
            foreach (MonoBehaviour component in playerPhysicsComponents)
            {
                component.enabled = true;
            }
            foreach (MonoBehaviour component in playerSpriteComponents)
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
            foreach (MonoBehaviour component in playerPhysicsComponents)
            {
                component.enabled = false;
            }
            foreach (MonoBehaviour component in playerSpriteComponents)
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
            if (File.Exists(Application.dataPath + "/playerData.json"))
            {
                string path = Path.Combine(Application.dataPath, "playerData.json");
                string jsonData = File.ReadAllText(path);
                playerData = JsonUtility.FromJson<PlayerData>(jsonData);
                //SceneManager.LoadScene(playerData.sceneName);
                StartCoroutine(LoadScene(playerData.sceneName));
            }
        }
        IEnumerator LoadScene(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = true;
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            Vector3 relaxedLoadAmount = Vector2.up * 0.015f;
            playerPhysics.transform.position = playerData.playerPosition + relaxedLoadAmount;
            currentScene = playerData.sceneName;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
