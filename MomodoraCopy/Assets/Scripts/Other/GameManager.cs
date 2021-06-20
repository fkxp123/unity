using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;


namespace MomodoraCopy
{
    [System.Serializable]
    public class PlayerData
    {
        public Vector3 playerPosition;
        public string sceneName;

        public int savedScore;
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
        int totalScore;
        public Text score;

        MonoBehaviour[] cameraComponents;
        MonoBehaviour[] playerPhysicsComponents;
        MonoBehaviour[] playerSpriteComponents;

        public Dictionary<int, List<GameObject>> checkPointsDict = new Dictionary<int, List<GameObject>>();
        public List<GameObject> checkPointsList = new List<GameObject>();

        public GameObject globalLightPrefab;
        GameObject globalLightObject;
        Light2D globalLight;

        public Image fadeMask;
        RectTransform fadeRectTransform;
        float fadeSpeed = 100;
        float speedAccel = 5000f;

        public GameObject ui;
        public GameObject fadeEffect;
        public GameObject invertMask;

        public PlayerInput playerInput;
        public PlayerMovement playerMovement;
        public Player playerFsm;
        public PlayerStatus playerStatus;

        public bool stopPlayerInput;
        public bool stopPlayerMovement;
        public bool stopPlayerFsm;

        public bool isPaused;

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
            Destroy(globalLightObject);
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
            globalLightObject = Instantiate(globalLightPrefab, Vector3.zero, Quaternion.identity);
            globalLight = globalLightObject.GetComponent<Light2D>();
            //ui.SetActive(false);
            fadeRectTransform = fadeMask.GetComponent<RectTransform>();

            playerPhysicsComponents = playerPhysics.GetComponents<MonoBehaviour>();
            playerSpriteComponents = playerSprite.GetComponents<MonoBehaviour>();
            cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();

            playerFsm = playerSprite.GetComponent<Player>();
            playerMovement = playerPhysics.GetComponent<PlayerMovement>();
            playerInput = playerPhysics.GetComponent<PlayerInput>();
            playerStatus = playerSprite.GetComponent<PlayerStatus>();
        }
        public void Start()
        {
            EventManager.instance.AddListener(EventType.GamePause, OnGamePause);
            EventManager.instance.AddListener(EventType.GameResume, OnGameResume);
        }

        void OnGamePause()
        {
            Pause();
            score.text = string.Empty;
        }
        void OnGameResume()
        {
            Resume();
            UpdateScoreBoard();
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
            playerData.savedScore = totalScore;

            string jsonData = JsonUtility.ToJson(playerData, true);
            string path = Path.Combine(Application.dataPath, "playerData.json");
            File.WriteAllText(path, jsonData);
        }
        public void Load()
        {
            if (File.Exists(Application.dataPath + "/playerData.json"))
            {
                invertMask.SetActive(true);
                string path = Path.Combine(Application.dataPath, "playerData.json");
                string jsonData = File.ReadAllText(path);
                playerData = JsonUtility.FromJson<PlayerData>(jsonData);
                //Vector3 relaxedLoadAmount = Vector2.up * 0.015f;
                //playerPhysics.transform.position = playerData.playerPosition + relaxedLoadAmount;
                //SceneManager.LoadScene(playerData.sceneName);
                StartCoroutine(LoadScene(playerData.sceneName));
            }
            else
            {
                //OperateFadeIn();

                scene = SceneManager.GetActiveScene();
                currentScene = scene.name;
                invertMask.SetActive(true);
                StartCoroutine(LoadScene(currentScene, true));
            }
        }
        IEnumerator LoadScene(string sceneName, bool isGeneratedMap = false)
        {
            DisablePlayer();
            yield return null;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            //asyncOperation.allowSceneActivation = false;
            if (!isGeneratedMap)
            {
                currentScene = playerData.sceneName;
                Vector3 relaxedLoadAmount = Vector2.up * 0.015f;
                playerPhysics.transform.position = playerData.playerPosition + relaxedLoadAmount;
                transform.position = playerPhysics.transform.position;
                totalScore = playerData.savedScore;
                UpdateScoreBoard();
            }
            else
            {

            }

            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            OperateFadeIn();
            EnablePlayer();
            //ui.SetActive(true);
        }

        public int GetTotalScore()
        {
            return totalScore;
        }
        public void SetTotalScore(int gemValue)
        {
            totalScore += gemValue;
            UpdateScoreBoard();
        }
        void UpdateScoreBoard()
        {
            score.text = totalScore.ToString();
        }

        public void OperateFadeIn()
        {
            StartCoroutine(FadeIn());
        }
        public void OperateFadeOut()
        {
            StartCoroutine(FadeOut());
        }
        IEnumerator FadeIn()
        {
            yield return null;
            float fadeAmount = 0;
            transform.position = playerPhysics.transform.position;
            fadeRectTransform.sizeDelta = new Vector2(fadeAmount, fadeAmount);
            while (fadeAmount < 1200)
            {
                invertMask.SetActive(false);
                invertMask.SetActive(true);
                fadeAmount += Time.deltaTime * fadeSpeed;
                fadeSpeed += Time.deltaTime * speedAccel;
                if (fadeAmount >= 600)
                {
                    //ui.SetActive(true);
                }
                fadeRectTransform.sizeDelta = new Vector2(fadeAmount, fadeAmount);
                yield return null;
            }
        }
        IEnumerator FadeOut()
        {
            yield return null;
            //ui.SetActive(false);
            float fadeAmount = 1200;
            fadeRectTransform.sizeDelta = new Vector2(fadeAmount, fadeAmount);
            transform.position = playerPhysics.transform.position;
            while (fadeAmount > 0)
            {
                invertMask.SetActive(false);
                invertMask.SetActive(true);
                fadeAmount -= Time.deltaTime * fadeSpeed;
                fadeSpeed += Time.deltaTime * speedAccel;
                fadeRectTransform.sizeDelta = new Vector2(fadeAmount, fadeAmount);
                yield return null;
            }
            Load();
        }

        public void DisablePlayer()
        {
            playerInput.enabled = false;
            playerMovement.enabled = false;
            playerFsm.enabled = false;
            playerStatus.enabled = false;
        }
        public void EnablePlayer()
        {
            playerInput.enabled = true;
            playerMovement.enabled = true;
            playerFsm.enabled = true;
            playerStatus.enabled = true;
        }

        void Update()
        {
            //for test------------------
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StartCoroutine(LoadScene("test1-1"));
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                OperateFadeOut();
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                OperateFadeOut();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                LocalizeManager.instance.CurrentLanguage = Language.English;
            }
            //--------------------------
        }
    }
}
