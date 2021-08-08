using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using InventorySystem;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    [System.Serializable]
    public class PlayerData
    {
        public Vector3 playerPosition;
        public string sceneName;

        public List<int> activeItemIDList;
        public List<int> passiveItemIDList;
        public List<int> keyItemIDList;

        public bool isItemActivated;

        public int savedScore;

        public int checkPointNameHash;
    }

    public class GameManager : Singleton<GameManager>
    {
        public PlayerData playerData;
        public GameObject playerPhysics;
        public GameObject playerSprite;
        public GameObject mainCameraObject;
        GameObject[] CheckPoints;
        Scene scene;
        public string currentScene;
        int totalScore;
        public Text score;

        MonoBehaviour[] cameraComponents;
        MonoBehaviour[] playerPhysicsComponents;
        MonoBehaviour[] playerSpriteComponents;

        public Dictionary<int, List<GameObject>> CheckPointsDict = new Dictionary<int, List<GameObject>>();
        public List<GameObject> CheckPointsList = new List<GameObject>();

        public GameObject globalLightPrefab;
        GameObject globalLightObject;
        Light2D globalLight;

        public GameObject invertFadeEffect;
        public Canvas fadeCanvas;
        public Image fadeMask;
        RectTransform fadeRectTransform;
        float fadeSpeed = 100;
        float speedAccel = 5000f;

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

        public GameObject simpleFadeEffect;
        public Image simpleFadeMask;
        RectTransform simpleFadeRectTransform;
        Animator simpleFadeAnimator;


        [Header("GuideBox")]
        public GameObject guideBox;
        public Text guideText;
        public GameObject saveGuideBox;
        public RectTransform mapleRect;
        float hideSaveGuideBoxTime = 5f;
        WaitForSeconds hideSaveGuideDelay;
        Coroutine mapleRotateRoutine;
        Coroutine hideSaveGuideRoutine;

        [Header("InspectBox")]
        public GameObject inspectInteraction;
        public GameObject inspectBox;
        Animator inspectBoxAnimator;
        public Text inspectText;

        [Header("GotItemMessageBox")]
        public GameObject GotItemMessageBox;
        public RectTransform GotItemRect;
        public Image GotItemImage;
        RectTransform GotItemImageRect;
        public Text GotItemText;
        public RectTransform GotItemButtonRect;
        float hideGotItemMessageTime = 1.5f;
        WaitForSeconds hideGotItemMessageDelay;

        [Header("PrayBox")]
        public GameObject prayInteraction;
        public GameObject prayBox;
        public GameObject praySelectBox;
        Animator prayBoxAnimator;
        public RectTransform choiceButtonRect;
        RectTransform choiceButtonParentRect;
        Coroutine choiceButtonRoutine;
        float rotateSpeed = 3f;

        [Header("UI")]
        public GameObject ui;

        HealthBar hpBar;
        int currentItemSlotCount;
        public int CurrentItemSlotCount
        {
            get { return currentItemSlotCount; }
            set
            {
                if(value < 0)
                {
                    value = maxItemSlotCount;
                }
                else if(value > maxItemSlotCount)
                {
                    value = 0;
                }
                for(int i = 0; i < slotItemImages.Length; i++)
                {
                    slotItemImages[i].gameObject.SetActive(false);
                    slotItemTexts[i].gameObject.SetActive(false);
                }
                slotItemImages[value].gameObject.SetActive(true);
                slotItemTexts[value].gameObject.SetActive(true);
                currentItemSlotCount = value;
            }
        }

        public GameObject itemSlots;
        public Item[] slotItems = new Item[3];
        Image[] slotItemImages = new Image[3];
        Text[] slotItemTexts = new Text[3];
        public Image itemSlotImage;
        public Text itemCountText;
        
        int maxItemSlotCount;

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (playerPhysics == null || !playerPhysics.CompareTag("Player"))
            {
                playerPhysics = GameObject.FindGameObjectWithTag("Player");
                playerSprite = playerPhysics.transform.GetChild(0).gameObject;
            }
            if (mainCameraObject == null || !mainCameraObject.CompareTag("MainCamera"))
            {
                mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            }
            if (CheckPoints == null)
            {
                CheckPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
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
                playerSprite = playerPhysics.transform.GetChild(0).gameObject;
            }
            if (mainCameraObject == null || !mainCameraObject.CompareTag("MainCamera"))
            {
                mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            }
            if (CheckPoints == null)
            {
                CheckPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
            }
            globalLightObject = Instantiate(globalLightPrefab, Vector3.zero, Quaternion.identity);
            globalLight = globalLightObject.GetComponent<Light2D>();
            //ui.SetActive(false);
            fadeRectTransform = fadeMask.GetComponent<RectTransform>();
            simpleFadeRectTransform = simpleFadeMask.GetComponent<RectTransform>();
            simpleFadeAnimator = simpleFadeMask.GetComponent<Animator>();

            playerPhysicsComponents = playerPhysics.GetComponents<MonoBehaviour>();
            playerSpriteComponents = playerSprite.GetComponents<MonoBehaviour>();
            cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();

            playerFsm = playerSprite.GetComponent<Player>();
            playerMovement = playerPhysics.GetComponent<PlayerMovement>();
            playerInput = playerPhysics.GetComponent<PlayerInput>();
            playerStatus = playerSprite.GetComponent<PlayerStatus>();

            hpBar = ui.GetComponent<HealthBar>();

            inspectBoxAnimator = inspectBox.transform.GetChild(0).GetComponent<Animator>();
            GotItemImageRect = GotItemImage.GetComponent<RectTransform>();

            prayBoxAnimator = prayBox.transform.GetChild(0).GetComponent<Animator>();
            choiceButtonParentRect = choiceButtonRect.transform.parent.GetComponent<RectTransform>();

            maxItemSlotCount = itemSlots.transform.childCount - 1;
            for (int i = 0; i < itemSlots.transform.childCount; i++)
            {
                slotItemImages[i] = itemSlots.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>(); 
                slotItemTexts[i] = itemSlots.transform.GetChild(i).transform.GetChild(1).GetComponent<Text>(); 
            }
        }

        public void Start()
        {
            EventManager.instance.AddListener(EventType.GamePause, OnGamePause);
            EventManager.instance.AddListener(EventType.GameResume, OnGameResume);
            EventManager.instance.AddListener(EventType.LanguageChange, OnLanguageChange);

            hideSaveGuideDelay = new WaitForSeconds(hideSaveGuideBoxTime);
            hideGotItemMessageDelay = new WaitForSeconds(hideGotItemMessageTime);

            CurrentItemSlotCount = 0;

            if (File.Exists(Application.dataPath + "/playerData.json"))
            {
                Load();
                return;
            }

            //OperateSimpleFadeIn();
        }

        void OnLanguageChange()
        {
            inspectText.text = LocalizeManager.instance.descriptionsDict
                ["InspectDesc".GetHashCode()]
                [LocalizeManager.instance.CurrentLanguage];
        }

        void OnGamePause()
        {
            HideGuideText();
            Pause();
        }
        void OnGameResume()
        {
            Resume();
        }

        //void SetSceneHashList()
        //{
        //    int sceneCount = SceneManager.sceneCountInBuildSettings;
        //    for (int i = 0; i < sceneCount; i++)
        //    {
        //        sceneHashList.Add(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)).GetHashCode());
        //    }
        //}

        //public void AddCheckPoint(string sceneName, GameObject CheckPoint)
        //{
        //    int hash = sceneName.GetHashCode();
        //    CheckPointsList.Add(CheckPoint);
        //    if (CheckPointsDict.ContainsKey(hash))
        //    {
        //        CheckPointsDict.Remove(hash);
        //    }
        //    CheckPointsDict.Add(hash, CheckPointsList);
        //}


        public void Resume()
        {
            Time.timeScale = 1f;
            foreach (MonoBehaviour component in playerPhysicsComponents)
            {
                component.enabled = true;
            }
            //foreach (MonoBehaviour component in playerSpriteComponents)
            //{
            //    component.enabled = true;
            //}
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
            //foreach (MonoBehaviour component in playerSpriteComponents)
            //{
            //    component.enabled = false;
            //}
            foreach (MonoBehaviour component in cameraComponents)
            {
                component.enabled = false;
            }
        }

        IEnumerator ShowImage(Image image)
        {
            Color color = image.color;
            float alphaAmount = 0;
            while(color.a < 1)
            {
                alphaAmount += 3;
                color.a = alphaAmount / 255f;
                image.color = color;
                yield return null;
            }
        }
        IEnumerator HideImage(Image image)
        {
            Color color = image.color;
            float alphaAmount = 0;
            while (color.a > 0)
            {
                alphaAmount -= 3;
                color.a = color.a - (alphaAmount / 255f);
                image.color = color;
                yield return null;
            }
        }

        void ShowSavedMessage()
        {
            saveGuideBox.SetActive(true);
            this.RestartCoroutine(HideSaveGuide(), ref hideSaveGuideRoutine);
            this.RestartCoroutine(RotateRect(mapleRect), ref mapleRotateRoutine);
        }
        IEnumerator HideSaveGuide()
        {
            yield return hideSaveGuideDelay;
            if (saveGuideBox.activeSelf)
            {
                saveGuideBox.SetActive(false);
            }
        }
        public void Save(CheckPoint checkPoint)
        {
            playerData.playerPosition = checkPoint.transform.position;
            playerData.sceneName = checkPoint.sceneName;
            playerData.savedScore = totalScore;
            playerData.checkPointNameHash = checkPoint.gameObject.name.GetHashCode();

            string jsonData = JsonUtility.ToJson(playerData, true);
            string path = Path.Combine(Application.dataPath, "playerData.json");
            File.WriteAllText(path, jsonData);

            ShowSavedMessage();
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
            //else
            //{
            //    //OperateFadeIn();

            //    scene = SceneManager.GetActiveScene();
            //    currentScene = scene.name;
            //    invertMask.SetActive(true);
            //    StartCoroutine(LoadScene(currentScene, true));
            //}
        }

        IEnumerator LoadScene(string sceneName, bool isGeneratedMap = false)
        {
            DisablePlayer();
            yield return null;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            //yield return asyncOperation;
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
            hpBar.HideUI();
            yield return null;
            float fadeAmount = 0;
            invertFadeEffect.transform.position = playerPhysics.transform.position;
            fadeRectTransform.sizeDelta = new Vector2(fadeAmount, fadeAmount);
            while (fadeAmount < 2400)
            {
                invertMask.SetActive(false);
                invertMask.SetActive(true);
                fadeAmount += Time.deltaTime * fadeSpeed;
                fadeSpeed += Time.deltaTime * speedAccel;
                if (fadeAmount >= 1200)
                {
                    hpBar.ShowUI();
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
            invertFadeEffect.transform.position = playerPhysics.transform.position;
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

        public void OperateSimpleFadeIn()
        {
            simpleFadeMask.gameObject.SetActive(true);
            simpleFadeEffect.transform.position = new Vector3(mainCameraObject.transform.position.x,
                mainCameraObject.transform.position.y, 0);
            simpleFadeAnimator.Play(Animator.StringToHash("SimpleFadeIn"), -1, 0);
        }

        public void OperateSimpleFadeOut()
        {
            simpleFadeEffect.transform.position = new Vector3(mainCameraObject.transform.position.x,
                mainCameraObject.transform.position.y, 0);
            simpleFadeAnimator.Play(Animator.StringToHash("SimpleFadeOut"), -1, 0);
            simpleFadeMask.gameObject.SetActive(false);
        }

        public void DisablePlayer()
        {
            playerInput.enabled = false;
            playerMovement.enabled = false;
            //playerFsm.enabled = false;
            playerFsm.isPause = true;
            playerStatus.enabled = false;
        }
        public void EnablePlayer()
        {
            playerInput.enabled = true;
            playerMovement.enabled = true;
            //playerFsm.enabled = true;
            playerFsm.isPause = false;
            playerStatus.enabled = true;
        }

        public void ShowGuideText(string context)
        {
            guideBox.SetActive(true);
            guideText.text = context;
        }
        public void HideGuideText()
        {
            guideBox.SetActive(false);
        }

        public void OpenInspectBox(Vector3 showPosition)
        {
            inspectBox.SetActive(false);
            inspectBox.SetActive(true);
            inspectInteraction.transform.position = showPosition;
        }
        public void CloseInspectBox()
        {
            inspectBoxAnimator.Play(Animator.StringToHash("Close"));
        }

        public void OpenPrayBox(Vector3 showPosition)
        {
            prayBox.SetActive(false);
            prayBox.SetActive(true);
            prayInteraction.transform.position = showPosition;
        }
        public void ClosePrayBox()
        {
            prayBoxAnimator.Play(Animator.StringToHash("Close"));
        }
        public void OpenPraySelectBox()
        {
            praySelectBox.SetActive(true);
        }
        public void ClosePraySelectBox()
        {
            praySelectBox.SetActive(false);
        }
        public void MoveChoiceButton(int selectedCount, int previousCount)
        {
            choiceButtonParentRect.anchoredPosition = new Vector2(choiceButtonParentRect.anchoredPosition.x,
                choiceButtonParentRect.anchoredPosition.y - 18 * (selectedCount - previousCount));
        }
        public void StartRotateChoiceButton()
        {
            choiceButtonRoutine = StartCoroutine(RotateRect(choiceButtonRect));
        }
        public void StopRotateChoiceButton()
        {
            choiceButtonRect.rotation = Quaternion.Euler(Vector3.zero);
            StopCoroutine(choiceButtonRoutine);
        }

        IEnumerator RotateRect(RectTransform rect)
        {
            float rotateAmount = rect.localRotation.y;
            while (true)
            {
                rotateAmount += 3;
                rect.localRotation = Quaternion.Euler(rect.localRotation.x,
                        rotateAmount, rect.localRotation.z);
                if (rect.rotation.y >= 360)
                {
                    rect.localRotation = Quaternion.Euler(rect.localRotation.x,
                        0, rect.localRotation.z);
                    rotateAmount = 0;
                }
                yield return null;
            }
        }

        public void ShowGotItemMessageBox(Item item)
        {
            GotItemMessageBox.SetActive(true);
            GotItemImage.sprite = ItemDatabase.instance.itemIDDict
                [item.itemIDName.GetHashCode()].itemIcon;
            string content = ItemDatabase.instance.itemContentDict
                [("Got" + item.itemIDName).GetHashCode()]
                [LocalizeManager.instance.CurrentLanguage];
            GotItemText.text = content;

            GotItemRect.sizeDelta = new Vector2(content.Length * 
                DialogueManager.instance.letterSizeDictionary
                [LocalizeManager.instance.CurrentLanguage] + 32, 32);
            GotItemButtonRect.anchoredPosition = new Vector3((GotItemRect.sizeDelta.x * 0.5f) - 4,
                (-1 * GotItemRect.sizeDelta.y * 0.5f) + 2 + 4, 0);
            GotItemImageRect.anchoredPosition = new Vector3((GotItemRect.sizeDelta.x * 0.5f) - 16, 0, 0);

            StartCoroutine(HideGotItemMessage());
        }
        IEnumerator HideGotItemMessage()
        {
            yield return hideGotItemMessageDelay;
            HideGotItemMessageBox();
        }
        public void HideGotItemMessageBox()
        {
            GotItemMessageBox.SetActive(false);
        }

        public void GetItem(Item item)
        {
            if(item.itemType == ItemType.ActiveItem)
            {
                playerData.activeItemIDList.Add(item.itemIDName.GetHashCode());
            }
            else if(item.itemType == ItemType.PassiveItem)
            {
                playerData.passiveItemIDList.Add(item.itemIDName.GetHashCode());
            }
            else if (item.itemType == ItemType.KeyItem)
            {
                playerData.keyItemIDList.Add(item.itemIDName.GetHashCode());
            }
        }

        public void SetSlotItem(int index, Item item)
        {
            if(item == null)
            {
                return;
            }
            slotItems[index] = item;
            slotItemImages[index].sprite = item.itemIcon;
            slotItemTexts[index].text = item.itemCount.ToString();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CurrentItemSlotCount -= 1;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {

                CurrentItemSlotCount += 1;
            }

            //for test------------------
            if (Input.GetKeyDown(KeyCode.F1))
            {
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
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
