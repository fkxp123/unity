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
            GameManager.instance.Resume();
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
                    mainMenuDescription.text = "장비";
                    break;
                case 1:
                    mainMenuDescription.text = "주요 아이템";
                    break;
                case 2:
                    mainMenuDescription.text = "지도";
                    break;
                case 3:
                    mainMenuDescription.text = "설정";
                    break;
                case 4:
                    mainMenuDescription.text = "시작 화면으로 돌아가기";
                    break;
                default:
                    break;
            }
        }
    }

}

