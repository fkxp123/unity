using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject mainCameraObject;
    MonoBehaviour[] cameraComponents;
    MonoBehaviour[] playerComponents;

    #region Menu
    public GameObject mainMenuObject;
    public GameObject selectedSlot;
    public Text descriptionText;

    [HideInInspector]
    public int selectedCount = 0;
    [HideInInspector]
    public GameObject[] selectedSlots;
    [HideInInspector]
    public Image[] img;
    #endregion

    #region SelectedMenu
    public GameObject selectedMenuObject;
    public GameObject inventoryMenuObject;
    public GameObject keyItemMenuObject;
    public GameObject mappingMenuObject;
    public GameObject settingMenuObject;
    public GameObject logOutMenuObject;
    #endregion
    [HideInInspector]
    public List<GameObject> selectedMenuObjectList = new List<GameObject>();

    [HideInInspector]
    public IMenu mainMenu;
    IMenu inventoryMenu;
    IMenu keyItemMenu;
    IMenu mappingMenu;
    IMenu settingMenu;
    IMenu logOutMenu;

    [HideInInspector]
    public List<IMenu> selectedMenuList = new List<IMenu>();

    public bool isGamePaused;

    [HideInInspector]
    public MenuHandler menuHandler;

    #region Singleton
    public static MenuManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion Sigleton

    void Start()
    {
        playerComponents = playerObject.GetComponents<MonoBehaviour>();
        cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();

        selectedSlots = new GameObject[selectedSlot.transform.childCount];
        img = new Image[selectedSlot.transform.childCount];
        descriptionText.text = "";

        selectedMenuObjectList.Add(inventoryMenuObject);
        selectedMenuObjectList.Add(keyItemMenuObject);
        selectedMenuObjectList.Add(mappingMenuObject);
        selectedMenuObjectList.Add(settingMenuObject);
        selectedMenuObjectList.Add(logOutMenuObject);

        mainMenu = new MainMenu();
        inventoryMenu = new InventoryMenu();
        keyItemMenu = new KeyItemMenu();
        mappingMenu = new MappingMenu();
        settingMenu = new SettingMenu();
        logOutMenu = new LogOutMenu();

        selectedMenuList.Add(inventoryMenu);
        selectedMenuList.Add(keyItemMenu);
        selectedMenuList.Add(mappingMenu);
        selectedMenuList.Add(settingMenu);
        selectedMenuList.Add(logOutMenu);

        menuHandler = new MenuHandler(mainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGamePaused)
        {
            Debug.Log("menu type : " + menuHandler.CurrentMenu);
            menuHandler.CurrentMenu.ShowMenu();
        }
    }

    public void Resume()
    {
        SetActiveFalseAllSelectedMenu();
        selectedCount = 0;
        mainMenuObject.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
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
        mainMenuObject.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        foreach (MonoBehaviour component in playerComponents)
        {
            component.enabled = false;
        }
        foreach (MonoBehaviour component in cameraComponents)
        {
            component.enabled = false;
        }
    }

    #region SelectedMenu Function
    public void SetActiveFalseAllSelectedMenu()
    {
        selectedMenuObject.SetActive(false);
        inventoryMenuObject.SetActive(false);
        keyItemMenuObject.SetActive(false);
        mappingMenuObject.SetActive(false);
        settingMenuObject.SetActive(false);
        logOutMenuObject.SetActive(false);
    }
    public void SetActiveTrueSelectedMenu(int selectedCount)
    {
        switch (selectedCount)
        {
            case 0:
                inventoryMenuObject.SetActive(true);
                break;
            case 1:
                keyItemMenuObject.SetActive(true);
                break;
            case 2:
                mappingMenuObject.SetActive(true);
                break;
            case 3:
                settingMenuObject.SetActive(true);
                break;
            case 4:
                logOutMenuObject.SetActive(true);//ui가 아니라 타이틀씬으로 이동?
                break;
            default:
                break;
        }
    }
    #endregion
}
