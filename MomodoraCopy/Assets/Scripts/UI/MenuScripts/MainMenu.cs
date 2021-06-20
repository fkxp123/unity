using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class MainMenu : AbstractMenu
    {
        #region MainMenu
        [Header("MainMenu")]
        public GameObject mainMenuObjectStatic;
        public GameObject mainMenuObjectDynamic;
        public GameObject selectedMainMenuSlot;
        public Text mainMenuDescription;

        int slotCount = 0;
        GameObject[] mainMenuSlots;
        Image[] slotImage;
        #endregion

        #region SelectedMenu
        [Header("SelectedMenu")]
        public GameObject selectedMenuObject;
        public GameObject inventoryMenuObject;
        public GameObject keyItemMenuObject;
        public GameObject mappingMenuObject;
        public GameObject settingMenuObject;
        public GameObject logOutMenuObject;

        InventoryMenu inventoryMenu;
        KeyItemMenu keyItemMenu;
        MappingMenu mappingMenu;
        SettingMenu settingMenu;
        LogOutMenu logOutMenu;
        #endregion
        
        List<AbstractMenu> selectedMenuList = new List<AbstractMenu>();

        string inventoryText;
        string keyItemText;
        string mapText;
        string settingsText;
        string returnToTitleScreenText;

        LocalizeManager localizeManager;

        protected override void Awake()
        {
            base.Awake();

            mainMenuSlots = new GameObject[selectedMainMenuSlot.transform.childCount];
            slotImage = new Image[selectedMainMenuSlot.transform.childCount];
            for (int i = 0; i < selectedMainMenuSlot.transform.childCount; i++)
            {
                mainMenuSlots[i] = selectedMainMenuSlot.transform.GetChild(i).gameObject;
                slotImage[i] = mainMenuSlots[i].GetComponent<Image>();
            }

            inventoryMenu = inventoryMenuObject.GetComponent<InventoryMenu>();
            keyItemMenu = keyItemMenuObject.GetComponent<KeyItemMenu>();
            mappingMenu = mappingMenuObject.GetComponent<MappingMenu>();
            settingMenu = settingMenuObject.GetComponent<SettingMenu>();
            logOutMenu = logOutMenuObject.GetComponent<LogOutMenu>();

            selectedMenuList.Add(inventoryMenu);
            selectedMenuList.Add(keyItemMenu);
            selectedMenuList.Add(mappingMenu);
            selectedMenuList.Add(settingMenu);
            selectedMenuList.Add(logOutMenu);

            localizeManager = LocalizeManager.instance;

            inventoryText = localizeManager.descriptionsDict
                ["InventoryDesc".GetHashCode()][localizeManager.CurrentLanguage];
            keyItemText = localizeManager.descriptionsDict
                ["KeyItemDesc".GetHashCode()][localizeManager.CurrentLanguage];
            mapText = localizeManager.descriptionsDict
                ["MapDesc".GetHashCode()][localizeManager.CurrentLanguage];
            settingsText = localizeManager.descriptionsDict
                ["SettingsDesc".GetHashCode()][localizeManager.CurrentLanguage];
            returnToTitleScreenText = localizeManager.descriptionsDict
                ["ReturnToTitleScreenDesc".GetHashCode()][localizeManager.CurrentLanguage];

            EventManager.instance.AddListener(EventType.LanguageChanged, OnLanguageChanged);
        }

        void OnLanguageChanged()
        {
            inventoryText = localizeManager.descriptionsDict
                ["InventoryDesc".GetHashCode()][localizeManager.CurrentLanguage];
            keyItemText = localizeManager.descriptionsDict
                ["KeyItemDesc".GetHashCode()][localizeManager.CurrentLanguage];
            mapText = localizeManager.descriptionsDict
                ["MapDesc".GetHashCode()][localizeManager.CurrentLanguage];
            settingsText = localizeManager.descriptionsDict
                ["SettingsDesc".GetHashCode()][localizeManager.CurrentLanguage];
            returnToTitleScreenText = localizeManager.descriptionsDict
                ["ReturnToTitleScreenDesc".GetHashCode()][localizeManager.CurrentLanguage];
            ChangeMenuDescription(slotCount);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (MenuManager.instance.isDisableByEscapeKey)
            {
                slotCount = 0;
            }
            mainMenuObjectStatic.SetActive(true);
            mainMenuObjectDynamic.SetActive(true);
            backgroundMenu.SetBackgroundAlpha(0.5f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarHighPos);

            ChangeSelectedSlotMenu(slotImage, slotCount);
            ChangeMenuDescription(slotCount);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            mainMenuObjectStatic.SetActive(false);
            mainMenuObjectDynamic.SetActive(false);
        }

        protected override void OperateMenuConfirm()
        {
            base.OperateMenuConfirm();
            selectedMenuList[slotCount].enabled = true;
        }
        protected override void OperateMenuCancle()
        {
            enabled = false;
            MenuManager.instance.isDisableByEscapeKey = true;
            EventManager.instance.PostNotification(EventType.GameResume);
        }

        public override void CheckArrowKey()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                slotCount += 1;
                if (slotCount >= selectedMainMenuSlot.transform.childCount)
                {
                    slotCount = 0;
                }
                ChangeSelectedSlotMenu(slotImage, slotCount);
                ChangeMenuDescription(slotCount);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                slotCount -= 1;
                if (slotCount < 0)
                {
                    slotCount = selectedMainMenuSlot.transform.childCount - 1;
                }
                ChangeSelectedSlotMenu(slotImage, slotCount);
                ChangeMenuDescription(slotCount);
            }
        }
        void ChangeMenuDescription(int selectedCount)
        {
            switch (selectedCount)
            {
                case 0:
                    mainMenuDescription.text = inventoryText;
                    break;
                case 1:
                    mainMenuDescription.text = keyItemText;
                    break;
                case 2:
                    mainMenuDescription.text = mapText;
                    break;
                case 3:
                    mainMenuDescription.text = settingsText;
                    break;
                case 4:
                    mainMenuDescription.text = returnToTitleScreenText;
                    break;
                default:
                    break;
            }
        }
    }

}

