using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class SettingMenu : AbstractMenu
    {
        #region SettingMenu
        public GameObject mainMenuObject;
        public GameObject settingMenuSlot;
        public GameObject selectedSettingMenuSlot;
        public Text selectedMenuInfo;

        [Header("SettingMenu")]
        public GameObject keyChangeMenuObject;

        MainMenu mainMenu;
        KeyChangeMenu keyChangeMenu;

        GameObject[] settingMenuSlots;
        Image[] slotImage;
        int slotCount;

        [HideInInspector]
        public List<GameObject> SettingMenuList = new List<GameObject>();
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            settingMenuSlot.SetActive(true);
            selectedSettingMenuSlot.SetActive(true);
            selectedMenuInfo.gameObject.SetActive(true);
            selectedMenuInfo.text = "설정";
            backgroundMenu.SetBackgroundAlpha(1f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarLowPos);
        }

        void Start()
        {
            mainMenu = mainMenuObject.GetComponent<MainMenu>();
            keyChangeMenu = keyChangeMenuObject.GetComponent<KeyChangeMenu>();

            settingMenuSlots = new GameObject[selectedSettingMenuSlot.transform.childCount];
            slotImage = new Image[selectedSettingMenuSlot.transform.childCount];
            for (int i = 0; i < selectedSettingMenuSlot.transform.childCount; i++)
            {
                settingMenuSlots[i] = selectedSettingMenuSlot.transform.GetChild(i).gameObject;
                slotImage[i] = settingMenuSlots[i].GetComponent<Image>();
            }

            ChangeSelectedSlotMenu(slotCount);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            settingMenuSlot.SetActive(false);
            selectedSettingMenuSlot.SetActive(false);
            selectedMenuInfo.gameObject.SetActive(false);
        }

        public override void CheckArrowKey()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                slotCount += 1;
                if (slotCount >= selectedSettingMenuSlot.transform.childCount)
                {
                    slotCount = 0;
                }
                ChangeSelectedSlotMenu(slotCount);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                slotCount -= 1;
                if (slotCount < 0)
                {
                    slotCount = selectedSettingMenuSlot.transform.childCount - 1;
                }
                ChangeSelectedSlotMenu(slotCount);
            }
        }
        public override void CheckConfirmKey()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if(slotCount == 6)
                {

                }
            }
        }
        public override void CheckCancleKey()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                enabled = false;
                mainMenu.enabled = true;
            }
        }

        void ChangeSelectedSlotMenu(int selectedCount)
        {
            SetInvisibleAllSelectedSlot();
            SetVisibleSelectedSlot(selectedCount);
        }
        void SetInvisibleAllSelectedSlot()
        {
            for (int i = 0; i < selectedSettingMenuSlot.transform.childCount; i++)
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

    }
}