using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class MenuManager : Singleton<MenuManager>
    {
        public GameObject mainMenuObject;
        MainMenu mainMenu;

        //public GameObject playerObject;
        //public GameObject mainCameraObject;
        //MonoBehaviour[] cameraComponents;
        //MonoBehaviour[] playerComponents;

        //public Canvas mainMenuCanvasStatic;
        //public Canvas mainMenuCanvasDynamic;
        //public Canvas mainMenuCanvasDynamic2;
        //public Canvas uiCanvas;

        //[Header("Others")]
        //public GameObject menuBackground;
        //public GameObject keyDescriptionBar;

        //#region MainMenu
        //[Header("MainMenu")]
        //public GameObject mainMenuObjectStatic;
        //public GameObject mainMenuObjectDynamic;
        //public GameObject selectedMainMenuSlot;
        //public Text mainMenuDescription;

        //[HideInInspector]
        //public int mainMenuSlotCount = 0;
        //[HideInInspector]
        //public GameObject[] mainMenuSlots;
        //[HideInInspector]
        //public Image[] mainMenuSlotImage;
        //#endregion

        //#region SelectedMenu
        //[Header("SelectedMenu")]
        //public GameObject selectedMenuObject;
        //public Text selectedMenuInfo;
        //public GameObject inventoryMenuObject;
        //public GameObject keyItemMenuObject;
        //public GameObject mappingMenuObject;
        //public GameObject settingMenuObject;
        //public GameObject logOutMenuObject;
        //#endregion

        //#region InventoryMenu

        //#endregion

        //#region KeyItemMenu

        //#endregion

        //#region MappingMenu

        //#endregion

        //#region SettingMenu
        //[Header("SettingMenu")]
        //public GameObject SoundEffectArea;
        //public GameObject AudioArea;
        //public GameObject ResolutionArea;
        //public GameObject QualityArea;
        //public GameObject ScreenVibrationArea;
        //public GameObject VibrationArea;
        //public GameObject KeyChangeArea;
        //public GameObject NormalSettingArea;
        //public GameObject ApplyChangesArea;
        //public GameObject ReturnArea;
        //[HideInInspector]
        //public List<GameObject> SettingMenuList = new List<GameObject>();
        //#endregion

        //#region LogOutMenu

        //#endregion

        //[HideInInspector]
        //public List<GameObject> selectedMenuObjectList = new List<GameObject>();

        //[HideInInspector]
        //public IMenu mainMenu;
        //IMenu inventoryMenu;
        //IMenu keyItemMenu;
        //IMenu mappingMenu;
        //IMenu settingMenu;
        //IMenu logOutMenu;

        //[HideInInspector]
        //public List<IMenu> selectedMenuList = new List<IMenu>();

        public bool _isGamePaused;
        public bool IsGamePaused
        {
            get { return _isGamePaused; }
            set
            {
                _isGamePaused = value;
                if (value)
                {
                    GameManager.instance.Pause();
                    mainMenu.enabled = true;
                    return;
                }
                GameManager.instance.Resume();
            }
        }

        void Start()
        {
            mainMenu = mainMenuObject.GetComponent<MainMenu>();
        }

        //[HideInInspector]
        //public MenuHandler menuHandler;

        //void Start()
        //{
            //playerComponents = playerObject.GetComponents<MonoBehaviour>();
            //cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();

            //mainMenuSlots = new GameObject[selectedMainMenuSlot.transform.childCount];
            //mainMenuSlotImage = new Image[selectedMainMenuSlot.transform.childCount];
            //mainMenuDescription.text = "";

            //selectedMenuInfo.text = "";
            //selectedMenuObjectList.Add(inventoryMenuObject);
            //selectedMenuObjectList.Add(keyItemMenuObject);
            //selectedMenuObjectList.Add(mappingMenuObject);
            //selectedMenuObjectList.Add(settingMenuObject);
            //selectedMenuObjectList.Add(logOutMenuObject);

            //mainMenu = new MainMenu();
            //inventoryMenu = new InventoryMenu();
            //keyItemMenu = new KeyItemMenu();
            //mappingMenu = new MappingMenu();
            //settingMenu = new SettingMenu();
            //logOutMenu = new LogOutMenu();

            //selectedMenuList.Add(inventoryMenu);
            //selectedMenuList.Add(keyItemMenu);
            //selectedMenuList.Add(mappingMenu);
            //selectedMenuList.Add(settingMenu);
            //selectedMenuList.Add(logOutMenu);

            //menuHandler = new MenuHandler();
            //SetActiveFalseAllMenu();
            //Pause();
            //Resume();
        //}

        // Update is called once per frame
        //void Update()
        //{
        //    if (IsGamePaused)
        //    {
        //        menuHandler.OperateUpdateMenu();
        //    }
        //}

        //void SetActiveFalseAllMenu()
        //{
        //    mainMenuObjectStatic.SetActive(false);
        //    mainMenuObjectDynamic.SetActive(false);
        //    menuBackground.SetActive(false);
        //    keyDescriptionBar.SetActive(false);
        //    SetActiveFalseAllSelectedMenu();
        //}

        //public void Resume()
        //{
        //    SetActiveFalseAllMenu();
        //    mainMenuSlotCount = 0;
        //    IsGamePaused = false;
        //    Time.timeScale = 1f;
        //    foreach (MonoBehaviour component in playerComponents)
        //    {
        //        component.enabled = true;
        //    }
        //    foreach (MonoBehaviour component in cameraComponents)
        //    {
        //        component.enabled = true;
        //    }
        //}
        //public void Pause()
        //{
        //    menuHandler.SetMenu(mainMenu);
        //    IsGamePaused = true;
        //    Time.timeScale = 0f;
        //    foreach (MonoBehaviour component in playerComponents)
        //    {
        //        component.enabled = false;
        //    }
        //    foreach (MonoBehaviour component in cameraComponents)
        //    {
        //        component.enabled = false;
        //    }
        //}

        //#region SelectedMenu Function
        //public void SetActiveFalseAllSelectedMenu()
        //{
        //    selectedMenuObject.SetActive(false);
        //    foreach (GameObject obj in selectedMenuObjectList)
        //    {
        //        obj.SetActive(false);
        //    }
        //}
        //public void SetActiveTrueSelectedMenu(int selectedCount)
        //{
        //    selectedMenuObjectList[selectedCount].SetActive(true);
        //}
        //#endregion
    }

}