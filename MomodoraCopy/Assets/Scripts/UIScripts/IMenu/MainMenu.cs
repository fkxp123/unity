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

        protected override void OnEnable()
        {
            base.OnEnable();
            slotCount = 0;
            mainMenuObjectStatic.SetActive(true);
            mainMenuObjectDynamic.SetActive(true);
            backgroundMenu.SetBackgroundAlpha(0.5f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarHighPos);
        }
        void Start()
        {
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

            ChangeSelectedSlotMenu(slotCount);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            mainMenuObjectStatic.SetActive(false);
            mainMenuObjectDynamic.SetActive(false);
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
                ChangeSelectedSlotMenu(slotCount);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                slotCount -= 1;
                if (slotCount < 0)
                {
                    slotCount = selectedMainMenuSlot.transform.childCount - 1;
                }
                ChangeSelectedSlotMenu(slotCount);
            }
        }
        public override void CheckConfirmKey()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                enabled = false;
                selectedMenuList[slotCount].enabled = true;
            }
        }
        public override void CheckCancleKey()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                enabled = false;
                GameManager.instance.Resume();
            }
        }

        void ChangeSelectedSlotMenu(int selectedCount)
        {
            ChangeMenuDescription(selectedCount);
            SetInvisibleAllSelectedSlot();
            SetVisibleSelectedSlot(selectedCount);
        }
        void SetInvisibleAllSelectedSlot()
        {
            for (int i = 0; i < selectedMainMenuSlot.transform.childCount; i++)
            {
                Color temp = slotImage[i].color;
                temp.a = 0.0f;
                slotImage[i].color = temp;
            }
        }
        void SetVisibleSelectedSlot(int selectedCount)
        {
            Color temp = slotImage[selectedCount].color;
            temp.a = 1.0f;
            slotImage[selectedCount].color = temp;
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

        #region SelectedMenu Function
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
        #endregion
    }

}

