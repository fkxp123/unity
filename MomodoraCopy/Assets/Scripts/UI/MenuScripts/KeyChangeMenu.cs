﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class KeyChangeMenu : AbstractMenu
    {
        public GameObject contentSlot;
        public GameObject selectedContentSlot;
        public GameObject menuInfo;

        SettingMenu settingMenu;

        GameObject[] contentSlots;
        Text[] selectedKeyCodeText;
        Image[] slotImage;
        int slotCount;

        int maxVisialbeSlotCount = 12;

        public Scrollbar scrollBar;
        float scrollBarMoveAmount;

        KeyCode[] keyCodes;
        bool isWatingChangeKey;
        string selectedKeyName;

        public List<string> originKeyName = new List<string>();
        public List<string> changeKeyName = new List<string>();

        protected override void Awake()
        {
            base.Awake();
            contentSlot.SetActive(false);
            
            contentSlots = new GameObject[selectedContentSlot.transform.childCount];
            selectedKeyCodeText = new Text[selectedContentSlot.transform.childCount];
            slotImage = new Image[selectedContentSlot.transform.childCount];
            for (int i = 0; i < selectedContentSlot.transform.childCount; i++)
            {
                contentSlots[i] = selectedContentSlot.transform.GetChild(i).gameObject;
                slotImage[i] = contentSlots[i].GetComponent<Image>();
            }

            scrollBarMoveAmount = 1.0f / (selectedContentSlot.transform.childCount - maxVisialbeSlotCount);

            keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
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
                selectedKeyCodeText[i] = contentSlots[i].transform.GetChild(0).GetComponent<Text>();
                selectedKeyCodeText[i].text = KeyboardManager.instance.keyCodeList[i].ToString();
            }
            contentSlot.SetActive(true);
            selectedContentSlot.SetActive(true);
            menuInfo.gameObject.SetActive(true);
            backgroundMenu.SetBackgroundAlpha(1f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarLowPos);

            ChangeSelectedSlotMenu(slotImage, slotCount);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            contentSlot.SetActive(false);
            selectedContentSlot.SetActive(false);
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
                selectedKeyCodeText[slotCount].text = "아무 버튼이나 누르세요";
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
                if (slotCount >= selectedContentSlot.transform.childCount)
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
                    slotCount = selectedContentSlot.transform.childCount - 1;
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