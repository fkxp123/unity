using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class KeyChangeMenu : AbstractMenu
    {
        public GameObject staticContentSlot;
        public GameObject dynamicContentSlot;
        public GameObject menuInfo;
        Text menuInfoText;

        SettingMenu settingMenu;

        GameObject[] dynamicContentSlots;
        GameObject[] staticContentSlots;
        Text[] selectedKeyCodeText;
        Text[] descriptionText;
        Image[] slotImage;
        int slotCount;

        int maxVisialbeSlotCount = 12;

        public Scrollbar scrollBar;
        float scrollBarMoveAmount;

        KeyCode[] keyCodes;
        bool isWatingChangeKey;
        string selectedKeyName;
        string pressAnyKeyText;

        public List<string> originKeyName = new List<string>();
        public List<string> changeKeyName = new List<string>();

        LocalizeManager localizeManager;

        protected override void Awake()
        {
            base.Awake();
            staticContentSlot.SetActive(false);
            
            dynamicContentSlots = new GameObject[dynamicContentSlot.transform.childCount];
            staticContentSlots = new GameObject[staticContentSlot.transform.childCount];
            selectedKeyCodeText = new Text[dynamicContentSlot.transform.childCount];
            descriptionText = new Text[staticContentSlot.transform.childCount];
            slotImage = new Image[dynamicContentSlot.transform.childCount];

            for (int i = 0; i < dynamicContentSlot.transform.childCount; i++)
            {
                dynamicContentSlots[i] = dynamicContentSlot.transform.GetChild(i).gameObject;
                slotImage[i] = dynamicContentSlots[i].GetComponent<Image>();
            }

            scrollBarMoveAmount = 1.0f / (dynamicContentSlot.transform.childCount - maxVisialbeSlotCount);

            keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

            localizeManager = LocalizeManager.instance;

            for (int i = 0; i < staticContentSlot.transform.childCount; i++)
            {
                staticContentSlots[i] = staticContentSlot.transform.GetChild(i).gameObject;
                descriptionText[i] = staticContentSlots[i].transform.GetChild(0).GetComponent<Text>();
                descriptionText[i].text = localizeManager.descriptionsDict
                    [descriptionText[i].name.GetHashCode()][localizeManager.CurrentLanguage];
            }
            menuInfoText = menuInfo.GetComponent<Text>();
            menuInfoText.text = localizeManager.descriptionsDict
                ["CustomizeKeysDesc".GetHashCode()][localizeManager.CurrentLanguage];

            pressAnyKeyText = localizeManager.descriptionsDict
                ["PressAnyKeyDesc".GetHashCode()][localizeManager.CurrentLanguage];

            EventManager.instance.AddListener(EventType.LanguageChanged, OnLanguageChanged);
        }

        void OnLanguageChanged()
        {
            menuInfoText.text = localizeManager.descriptionsDict
                ["CustomizeKeysDesc".GetHashCode()][localizeManager.CurrentLanguage];
            pressAnyKeyText = localizeManager.descriptionsDict
                ["PressAnyKeyDesc".GetHashCode()][localizeManager.CurrentLanguage];

            for (int i = 0; i < descriptionText.Length; i++)
            {
                descriptionText[i].text = localizeManager.descriptionsDict
                    [descriptionText[i].name.GetHashCode()][localizeManager.CurrentLanguage];
            }
        }

        protected override void Update()
        {
            if (!isWatingChangeKey)
            {
                base.Update();
                return;
            }
            DetectedKeyCodeDown();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (MenuManager.instance.isDisableByEscapeKey)
            {
                slotCount = 0;
                scrollBar.value = 1f;
            }
            KeyboardManager.instance.GetKeyCodes();
            for (int i = 0; i < KeyboardManager.instance.keyCodeList.Count; i++)
            {
                selectedKeyCodeText[i] = dynamicContentSlots[i].transform.GetChild(0).GetComponent<Text>();
                selectedKeyCodeText[i].text = KeyboardManager.instance.keyCodeList[i].ToString();
            }
            staticContentSlot.SetActive(true);
            dynamicContentSlot.SetActive(true);
            menuInfo.gameObject.SetActive(true);
            backgroundMenu.SetBackgroundAlpha(1f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarLowPos);

            ChangeSelectedSlotMenu(slotImage, slotCount);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            staticContentSlot.SetActive(false);
            dynamicContentSlot.SetActive(false);
            menuInfo.gameObject.SetActive(false);
            originKeyName.Clear();
            changeKeyName.Clear();
        }

        protected override void OperateMenuConfirm()
        {
            if(slotCount < 12)
            {
                isWatingChangeKey = true;
                selectedKeyName = selectedKeyCodeText[slotCount].text;
                selectedKeyCodeText[slotCount].text = pressAnyKeyText;
            }
            else if(slotCount == 12)
            {
                SaveKeyChanges();
            }
            else if(slotCount == 13)
            {
                SetDefaultKeySetting();
            }
            else if(slotCount == 14)
            {
                OperateMenuCancle();
            }
            
        }
        
        public override void CheckArrowKey()
        {
            if (Input.GetKeyDown(KeyboardManager.instance.DownKey))
            {
                slotCount += 1;
                if (slotCount >= dynamicContentSlot.transform.childCount)
                {
                    slotCount = 0;
                    scrollBar.value = 1f;
                }
                if (slotCount >= maxVisialbeSlotCount)
                {
                    scrollBar.value -= scrollBarMoveAmount;
                }
                else
                {
                    scrollBar.value = 1f;
                }
                ChangeSelectedSlotMenu(slotImage, slotCount);
            }
            else if (Input.GetKeyDown(KeyboardManager.instance.UpKey))
            {
                slotCount -= 1;
                if (slotCount >= maxVisialbeSlotCount)
                {
                    scrollBar.value += scrollBarMoveAmount;
                }
                else
                {
                    scrollBar.value = 1f;
                }
                if (slotCount < 0)
                {
                    slotCount = dynamicContentSlot.transform.childCount - 1;
                    scrollBar.value = 0f;
                }
                ChangeSelectedSlotMenu(slotImage, slotCount);
            }
        }

        void DetectedKeyCodeDown()
        {
            foreach(KeyCode keyCode in keyCodes)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    CheckSameKeyCode(keyCode);
                    selectedKeyCodeText[slotCount].text = keyCode.ToString();
                    originKeyName.Add(KeyboardManager.instance.keyNameList[slotCount]);
                    changeKeyName.Add(keyCode.ToString());
                    isWatingChangeKey = false;
                    selectedKeyName = string.Empty;
                }
            }
        }
        void CheckSameKeyCode(KeyCode keyCode)
        {
            for (int i = 0; i < KeyboardManager.instance.keyCodeList.Count; i++)
            {
                if (selectedKeyCodeText[i].text.Equals(keyCode.ToString()))
                {
                    selectedKeyCodeText[i].text = selectedKeyName;
                    originKeyName.Add(KeyboardManager.instance.keyNameList[i]);
                    changeKeyName.Add(selectedKeyName);
                }
            }
        }
        void SetDefaultKeySetting()
        {
            originKeyName.Clear();
            changeKeyName.Clear();
            KeyboardManager.instance.SetDefaultKeyCodes();
            KeyboardManager.instance.GetKeyCodes();
            for (int i = 0; i < KeyboardManager.instance.keyCodeList.Count; i++)
            {
                selectedKeyCodeText[i].text = KeyboardManager.instance.keyCodeList[i].ToString();
            }
        }
        void SaveKeyChanges()
        {
            for(int i = 0; i < originKeyName.Count; i++)
            {
                PlayerPrefs.SetString(originKeyName[i], changeKeyName[i]);
            }
            KeyboardManager.instance.GetKeyCodes();
        }
    }
}
