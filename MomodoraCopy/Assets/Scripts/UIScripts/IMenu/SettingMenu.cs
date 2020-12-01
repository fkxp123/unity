using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class SettingMenu : AbstractMenu
    {
        #region SettingMenu
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
        #endregion

        protected override void Awake()
        {
            base.Awake();
            settingMenuSlot.SetActive(false);
            keyChangeMenu = keyChangeMenuObject.GetComponent<KeyChangeMenu>();

            settingMenuSlots = new GameObject[selectedSettingMenuSlot.transform.childCount];
            slotImage = new Image[selectedSettingMenuSlot.transform.childCount];
            for (int i = 0; i < selectedSettingMenuSlot.transform.childCount; i++)
            {
                settingMenuSlots[i] = selectedSettingMenuSlot.transform.GetChild(i).gameObject;
                slotImage[i] = settingMenuSlots[i].GetComponent<Image>();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (MenuManager.instance.isDisableByEscapeKey)
            {
                slotCount = 0;
            }
            settingMenuSlot.SetActive(true);
            selectedSettingMenuSlot.SetActive(true);
            selectedMenuInfo.gameObject.SetActive(true);
            selectedMenuInfo.text = "설정";
            backgroundMenu.SetBackgroundAlpha(1f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarLowPos);

            ChangeSelectedSlotMenu(slotImage, slotCount);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            settingMenuSlot.SetActive(false);
            selectedSettingMenuSlot.SetActive(false);
            selectedMenuInfo.gameObject.SetActive(false);
        }

        protected override void OperateMenuConfirm()
        {
            if (slotCount == 6)
            {
                enabled = false;
                MenuManager.instance.menuMemento.SetMenuMemento(gameObject);
                keyChangeMenu.enabled = true;
            }
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
                ChangeSelectedSlotMenu(slotImage, slotCount);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                slotCount -= 1;
                if (slotCount < 0)
                {
                    slotCount = selectedSettingMenuSlot.transform.childCount - 1;
                }
                ChangeSelectedSlotMenu(slotImage, slotCount);
            }
        }

    }
}