using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using InventorySystem;
using System;

namespace MomodoraCopy
{
    public class InventoryMenu : AbstractMenu
    {
        public GameObject inventoryMenuDynamic;
        public GameObject inventoryMenuStatic;

        public GameObject inventorySlot;

        public GameObject activeSlotDynamic;
        public GameObject passiveSlotDynamic;

        public GameObject activeContentSlot;
        public GameObject passiveContentSlot;

        GameObject[] activeSlots;
        Image[] activeSlotImages;
        Image[] activeSlotItemImages;
        GameObject[] passiveSlots; 
        Image[] passiveSlotImages;
        Image[] passiveSlotItemImages;

        LocalizeManager localizeManager;

        Coroutine routine;

        float blinkAmount = 2f;
        float maxSlotBlinkAlpha = 150f;
        int selectedSlotCount;
        int SelectedSlotCount
        {
            get { return selectedSlotCount; }
            set
            {
                if (routine != null)
                {
                    StopCoroutine(routine);
                }
                //ChangeAlpha(rowCount == 0 ? activeSlotImages[selectedSlotCount]
                //    : passiveSlotImages[selectedSlotCount], 0);
                if (RowCount == 0)
                {
                    for (int i = 0; i < activeSlotDynamic.transform.childCount; i++)
                    {
                        ChangeAlpha(activeSlotImages[i], 0);
                    }
                    if (activeSlotItemImages[value].sprite != null)
                    {
                        itemNameText.text = itemDatabase.itemContentDict
                            [(activeSlotItemImages[value].sprite.name + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemEffectText.text = itemDatabase.itemContentDict
                            [(activeSlotItemImages[value].sprite.name + "Effect").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemDescriptionText.text = itemDatabase.itemContentDict
                            [(activeSlotItemImages[value].sprite.name + "Description").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                    }
                    else
                    {
                        itemNameText.text = string.Empty;
                        itemEffectText.text = string.Empty;
                        itemDescriptionText.text = string.Empty;
                    }
                }
                else if (RowCount == 1)
                {
                    for (int i = 0; i < passiveSlotDynamic.transform.childCount; i++)
                    {
                        ChangeAlpha(passiveSlotImages[i], 0);
                    }
                    if (passiveSlotItemImages[value].sprite != null)
                    {
                        itemNameText.text = itemDatabase.itemContentDict
                            [(passiveSlotItemImages[value].sprite.name + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemEffectText.text = itemDatabase.itemContentDict
                            [(passiveSlotItemImages[value].sprite.name + "Effect").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemDescriptionText.text = itemDatabase.itemContentDict
                            [(passiveSlotItemImages[value].sprite.name + "Description").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                    }
                    else
                    {
                        itemNameText.text = string.Empty;
                        itemEffectText.text = string.Empty;
                        itemDescriptionText.text = string.Empty;
                    }
                }
                routine = StartCoroutine(BlinkImage(RowCount == 0 ?
                    activeSlotImages[value] : passiveSlotImages[value], maxSlotBlinkAlpha, 0));
                selectedSlotCount = value;
            }
        }
        int rowCount;
        int RowCount
        {
            get { return rowCount; }
            set
            {
                if (routine != null)
                {
                    StopCoroutine(routine);
                }
                if (RowCount == 0)
                {
                    for (int i = 0; i < activeSlotDynamic.transform.childCount; i++)
                    {
                        ChangeAlpha(activeSlotImages[i], 0);
                    }
                }
                else if (RowCount == 1)
                {
                    for (int i = 0; i < passiveSlotDynamic.transform.childCount; i++)
                    {
                        ChangeAlpha(passiveSlotImages[i], 0);
                    }
                }

                if(value == 0)
                {
                    activeScrollArea.SetActive(true);
                    passiveScrollArea.SetActive(false);
                }
                else if(value == 1)
                {
                    activeScrollArea.SetActive(false);
                    passiveScrollArea.SetActive(true);
                }

                rowCount = value;
                SelectedSlotCount = 0;
            }
        }
        int maxRowCount = 1;
        int maxActiveSlotCount;
        int maxPassiveSlotCount;

        bool isActiveInventory;

        int selectedInventorySlotCount;

        int SelectedInventorySlotCount
        {
            get { return selectedInventorySlotCount; }
            set
            {
                //ChangeAlpha(RowCount == 0 ? activeInventoryImageList[selectedInventorySlotCount] :
                //    passiveInventoryImageList[selectedInventorySlotCount], 0);
                if (RowCount == 0)
                {
                    //DownArrow
                    if (value > SelectedInventorySlotCount)
                    {

                        for (int i = 0; i < equippedActiveItemSelectNumber.Length; i++)
                        {
                            for (int j = 0; j < equippedActiveItemSelectNumber.Length; j++)
                            {
                                if (equippedActiveItemSelectNumber[j] != null)
                                {
                                    if (value == equippedActiveItemSelectNumber[j])
                                    {
                                        value = value + 1;
                                        break;
                                    }
                                }
                            }
                        }
                        if(value > activeInventorySlotList.Count - 1)
                        {
                            value = 0;
                            activeScrollBar.value = 1f; 
                        }
                        moveCount = (1 - activeScrollBar.value) *
                            (activeInventorySlotList.Count - 1 - activeEquippedCount - maxVisibleSlotCount);

                        int checkCount = 0;
                        for (int i = Mathf.CeilToInt(moveCount); 
                            i < maxVisibleSlotCount + Mathf.CeilToInt(moveCount); i++)
                        {
                            for (int j = 0; j < equippedActiveItemSelectNumber.Length; j++)
                            {
                                if (i == equippedActiveItemSelectNumber[j])
                                {
                                    checkCount += 1;
                                }
                            }
                        }
                        if (value >= maxVisibleSlotCount + Mathf.CeilToInt(moveCount) + checkCount)
                        {
                            activeScrollMoveAmount = 1.0f /
                                (activeInventorySlotList.Count - activeEquippedCount - maxVisibleSlotCount);
                            activeScrollBar.value -= activeScrollMoveAmount;
                            activeScrollBar.value = Mathf.Clamp01(activeScrollBar.value);
                        }
                    }
                    //UpArrow
                    else if (value < SelectedInventorySlotCount)
                    {
                        if(value < 0)
                        {
                            value = activeInventorySlotList.Count - 1;
                            activeScrollBar.value = 0f;
                        }

                        int fixedCount = 0;
                        for (int i = 0; i < equippedActiveItemSelectNumber.Length; i++)
                        {
                            for (int j = 0; j < equippedActiveItemSelectNumber.Length; j++)
                            {
                                if (equippedActiveItemSelectNumber[j] != null)
                                {
                                    if (value == equippedActiveItemSelectNumber[j])
                                    {
                                        value = value - 1;
                                        fixedCount += 1;
                                        break;
                                    }
                                }
                            }
                        }
                        //Debug.Log("fixedCount" + fixedCount);

                        moveCount = (1 - activeScrollBar.value) *
                            (activeInventorySlotList.Count - 1 - maxVisibleSlotCount);
                        int checkCount = 0;
                        for (int i = Mathf.FloorToInt(moveCount) + activeEquippedCount; 
                            i <= maxVisibleSlotCount + Mathf.FloorToInt(moveCount) + activeEquippedCount; i++)
                        {
                            for (int j = 0; j < equippedActiveItemSelectNumber.Length; j++)
                            {
                                if (i == equippedActiveItemSelectNumber[j])
                                {
                                    checkCount += 1;
                                }
                            }
                        }

                        moveCount = (1 - activeScrollBar.value) *
                            (activeInventorySlotList.Count - 1 - activeEquippedCount - maxVisibleSlotCount);
                        //Debug.Log("checkCount : " + checkCount);
                        //Debug.Log("value : " + value);
                        //Debug.Log(moveCount);
                        //Debug.Log(Mathf.FloorToInt(moveCount));
                        moveCount = Mathf.Clamp(moveCount, 0, moveCount);

                        if (value <= Mathf.FloorToInt(moveCount) + activeEquippedCount - checkCount + fixedCount)
                        {
                            activeScrollMoveAmount = 1.0f /
                                (activeInventorySlotList.Count - activeEquippedCount - maxVisibleSlotCount);
                            activeScrollBar.value += activeScrollMoveAmount;
                            activeScrollBar.value = Mathf.Clamp01(activeScrollBar.value);
                        }
                    }

                    if (activeInventoryItemList[value].itemIDName != null)
                    {
                        itemNameText.text = itemDatabase.itemContentDict
                            [(activeInventoryItemList[value].itemIDName + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemEffectText.text = itemDatabase.itemContentDict
                            [(activeInventoryItemList[value].itemIDName + "Effect").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemDescriptionText.text = itemDatabase.itemContentDict
                            [(activeInventoryItemList[value].itemIDName + "Description").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                    }
                    else
                    {
                        itemNameText.text = string.Empty;
                        itemEffectText.text = string.Empty;
                        itemDescriptionText.text = string.Empty;
                    }
                    for(int i = 0; i < activeInventoryImageList.Count; i++)
                    {
                        ChangeAlpha(activeInventoryImageList[i], 0);
                    }
                }
                else if (RowCount == 1)
                {
                    //DownArrow
                    if (value > SelectedInventorySlotCount)
                    {

                        for (int i = 0; i < equippedPassiveItemSelectNumber.Length; i++)
                        {
                            for (int j = 0; j < equippedPassiveItemSelectNumber.Length; j++)
                            {
                                if (equippedPassiveItemSelectNumber[j] != null)
                                {
                                    if (value == equippedPassiveItemSelectNumber[j])
                                    {
                                        value = value + 1;
                                        break;
                                    }
                                }
                            }
                        }
                        if (value > passiveInventorySlotList.Count - 1)
                        {
                            value = 0;
                            passiveScrollBar.value = 1f;
                        }
                        moveCount = (1 - passiveScrollBar.value) *
                            (passiveInventorySlotList.Count - 1 - passiveEquippedCount - maxVisibleSlotCount);

                        int checkCount = 0;
                        for (int i = Mathf.CeilToInt(moveCount);
                            i < maxVisibleSlotCount + Mathf.CeilToInt(moveCount); i++)
                        {
                            for (int j = 0; j < equippedPassiveItemSelectNumber.Length; j++)
                            {
                                if (i == equippedPassiveItemSelectNumber[j])
                                {
                                    checkCount += 1;
                                }
                            }
                        }
                        if (value >= maxVisibleSlotCount + Mathf.CeilToInt(moveCount) + checkCount)
                        {
                            passiveScrollMoveAmount = 1.0f /
                                (passiveInventorySlotList.Count - passiveEquippedCount - maxVisibleSlotCount);
                            passiveScrollBar.value -= passiveScrollMoveAmount;
                            passiveScrollBar.value = Mathf.Clamp01(passiveScrollBar.value);
                        }
                    }
                    //UpArrow
                    else if (value < SelectedInventorySlotCount)
                    {
                        if (value < 0)
                        {
                            value = passiveInventorySlotList.Count - 1;
                            passiveScrollBar.value = 0f;
                        }

                        int fixedCount = 0;
                        for (int i = 0; i < equippedPassiveItemSelectNumber.Length; i++)
                        {
                            for (int j = 0; j < equippedPassiveItemSelectNumber.Length; j++)
                            {
                                if (equippedPassiveItemSelectNumber[j] != null)
                                {
                                    if (value == equippedPassiveItemSelectNumber[j])
                                    {
                                        value = value - 1;
                                        fixedCount += 1;
                                        break;
                                    }
                                }
                            }
                        }
                        //Debug.Log("fixedCount" + fixedCount);

                        moveCount = (1 - passiveScrollBar.value) *
                            (passiveInventorySlotList.Count - 1 - maxVisibleSlotCount);
                        int checkCount = 0;
                        for (int i = Mathf.FloorToInt(moveCount) + passiveEquippedCount;
                            i <= maxVisibleSlotCount + Mathf.FloorToInt(moveCount) + passiveEquippedCount; i++)
                        {
                            for (int j = 0; j < equippedPassiveItemSelectNumber.Length; j++)
                            {
                                if (i == equippedPassiveItemSelectNumber[j])
                                {
                                    checkCount += 1;
                                }
                            }
                        }

                        moveCount = (1 - passiveScrollBar.value) *
                            (passiveInventorySlotList.Count - 1 - passiveEquippedCount - maxVisibleSlotCount);
                        //Debug.Log("checkCount : " + checkCount);
                        //Debug.Log("value : " + value);
                        //Debug.Log(moveCount);
                        //Debug.Log(Mathf.FloorToInt(moveCount));
                        moveCount = Mathf.Clamp(moveCount, 0, moveCount);

                        if (value <= Mathf.FloorToInt(moveCount) + passiveEquippedCount - checkCount + fixedCount)
                        {
                            passiveScrollMoveAmount = 1.0f /
                                (passiveInventorySlotList.Count - passiveEquippedCount - maxVisibleSlotCount);
                            passiveScrollBar.value += passiveScrollMoveAmount;
                            passiveScrollBar.value = Mathf.Clamp01(passiveScrollBar.value);
                        }
                    }

                    if (passiveInventoryItemList[value].itemIDName != null)
                    {
                        itemNameText.text = itemDatabase.itemContentDict
                            [(passiveInventoryItemList[value].itemIDName + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemEffectText.text = itemDatabase.itemContentDict
                            [(passiveInventoryItemList[value].itemIDName + "Effect").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemDescriptionText.text = itemDatabase.itemContentDict
                            [(passiveInventoryItemList[value].itemIDName + "Description").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                    }
                    else
                    {
                        itemNameText.text = string.Empty;
                        itemEffectText.text = string.Empty;
                        itemDescriptionText.text = string.Empty;
                    }
                    for (int i = 0; i < passiveInventoryImageList.Count; i++)
                    {
                        ChangeAlpha(passiveInventoryImageList[i], 0);
                    }
                }

                selectedInventorySlotCount = value;
                if (!isActiveInventory)
                {
                    return;
                }
                if (routine != null)
                {
                    StopCoroutine(routine);
                }

                routine = StartCoroutine(BlinkImage(RowCount == 0 ?
                    activeInventoryImageList[value] : passiveInventoryImageList[value], 255, 100));
            }
        }

        int maxActiveInventorySlotCount;
        int maxPassiveInventorySlotCount;

        List<GameObject> activeInventorySlotList = new List<GameObject>();
        List<GameObject> passiveInventorySlotList = new List<GameObject>();
        List<Image> activeInventoryImageList = new List<Image>();
        List<Image> passiveInventoryImageList = new List<Image>();
        List<Text> activeInventoryTextList = new List<Text>();
        List<Text> passiveInventoryTextList = new List<Text>();
        List<Item> activeInventoryItemList = new List<Item>();
        List<Item> passiveInventoryItemList = new List<Item>();

        ItemDatabase itemDatabase;
        GameManager gameManager;

        public Scrollbar activeScrollBar;
        public Scrollbar passiveScrollBar;
        float activeScrollMoveAmount;
        float passiveScrollMoveAmount;
        int maxVisibleSlotCount = 7;
        float moveCount = 0;

        public GameObject descriptionArea;
        Text itemNameText;
        Text itemEffectText;
        Text itemDescriptionText;

        public Text activeSlotText;
        public Text passiveSlotText;
        public Text inventorySlotText;

        int?[] equippedActiveItemSelectNumber;
        int?[] equippedPassiveItemSelectNumber;
        Item[] equippedActiveItem;
        Item[] equippedPassiveItem;

        public GameObject activeScrollArea;
        public GameObject passiveScrollArea;

        protected override void Awake()
        {
            localizeManager = LocalizeManager.instance;
            gameManager = GameManager.instance;
            itemDatabase = ItemDatabase.instance;

            inventoryMenuDynamic.SetActive(false);
            inventoryMenuStatic.SetActive(false);

            activeSlots = new GameObject[activeSlotDynamic.transform.childCount];
            activeSlotImages = new Image[activeSlotDynamic.transform.childCount];
            activeSlotItemImages = new Image[activeSlotDynamic.transform.childCount];
            passiveSlots = new GameObject[passiveSlotDynamic.transform.childCount];
            passiveSlotImages = new Image[passiveSlotDynamic.transform.childCount];
            passiveSlotItemImages = new Image[passiveSlotDynamic.transform.childCount];

            equippedActiveItemSelectNumber = new int?[activeSlotDynamic.transform.childCount];
            equippedActiveItem = new Item[activeSlotDynamic.transform.childCount];
            equippedPassiveItemSelectNumber = new int?[passiveSlotDynamic.transform.childCount];
            equippedPassiveItem = new Item[passiveSlotDynamic.transform.childCount];

            maxActiveSlotCount = activeSlotDynamic.transform.childCount - 1;
            maxPassiveSlotCount = passiveSlotDynamic.transform.childCount - 1;

            for (int i = 0; i < activeSlotDynamic.transform.childCount; i++)
            {
                activeSlots[i] = activeSlotDynamic.transform.GetChild(i).gameObject;
                activeSlotImages[i] = activeSlots[i].transform.GetChild(0).GetComponent<Image>();
                activeSlotItemImages[i] = activeSlots[i].GetComponent<Image>();
                ChangeAlpha(activeSlotImages[i], 0);
            }

            for (int i = 0; i < passiveSlotDynamic.transform.childCount; i++)
            {
                passiveSlots[i] = passiveSlotDynamic.transform.GetChild(i).gameObject;
                passiveSlotImages[i] = passiveSlots[i].transform.GetChild(0).GetComponent<Image>();
                passiveSlotItemImages[i] = passiveSlots[i].GetComponent<Image>();
                ChangeAlpha(passiveSlotImages[i], 0);
            }

            activeInventorySlotList.Add(activeContentSlot.transform.GetChild(0).gameObject);
            activeInventoryImageList.Add(activeContentSlot.transform.GetChild(0).GetComponent<Image>());
            activeInventoryTextList.Add(activeInventorySlotList[0].transform.GetChild(0).GetComponent<Text>());
            activeInventoryItemList.Add(new Item(null, ItemType.ActiveItem));

            passiveInventorySlotList.Add(passiveContentSlot.transform.GetChild(0).gameObject);
            passiveInventoryImageList.Add(passiveContentSlot.transform.GetChild(0).GetComponent<Image>());
            passiveInventoryTextList.Add(passiveInventorySlotList[0].transform.GetChild(0).GetComponent<Text>());
            passiveInventoryItemList.Add(new Item(null, ItemType.ActiveItem));

            for (int i = 0; i < activeContentSlot.transform.childCount; i++)
            {
                ChangeAlpha(activeInventoryImageList[i], 0);
            }
            for (int i = 0; i < passiveContentSlot.transform.childCount; i++)
            {
                ChangeAlpha(passiveInventoryImageList[i], 0);
            }

            itemNameText = descriptionArea.transform.GetChild(0).GetComponent<Text>();
            itemEffectText = descriptionArea.transform.GetChild(1).GetComponent<Text>();
            itemDescriptionText = descriptionArea.transform.GetChild(2).GetComponent<Text>();

            activeSlotText.text = localizeManager.descriptionsDict["ActiveSlotDesc".GetHashCode()]
                [localizeManager.CurrentLanguage];
            passiveSlotText.text = localizeManager.descriptionsDict["PassiveSlotDesc".GetHashCode()]
                [localizeManager.CurrentLanguage];
            inventorySlotText.text = localizeManager.descriptionsDict["InventorySlotDesc".GetHashCode()]
                [localizeManager.CurrentLanguage];

            EventManager.instance.AddListener(EventType.LanguageChange, OnLanguageChange);
            base.Awake();
        }

        void OnDestroy()
        {
            EventManager.instance.UnsubscribeEvent(EventType.LanguageChange, OnLanguageChange);
        }

        void OnLanguageChange()
        {
            activeSlotText.text = localizeManager.descriptionsDict["ActiveSlotDesc".GetHashCode()]
                [localizeManager.CurrentLanguage];
            passiveSlotText.text = localizeManager.descriptionsDict["PassiveSlotDesc".GetHashCode()]
                [localizeManager.CurrentLanguage];
            inventorySlotText.text = localizeManager.descriptionsDict["InventorySlotDesc".GetHashCode()]
                [localizeManager.CurrentLanguage];

            for(int i = 1; i < activeInventoryTextList.Count; i++)
            {
                activeInventoryTextList[i].text = itemDatabase.itemContentDict
                    [(activeInventoryItemList[i].itemIDName + "Name").GetHashCode()]
                    [localizeManager.CurrentLanguage];
            }
            for (int i = 1; i < passiveInventoryTextList.Count; i++)
            {
                passiveInventoryTextList[i].text = itemDatabase.itemContentDict
                    [(passiveInventoryItemList[i].itemIDName + "Name").GetHashCode()]
                    [localizeManager.CurrentLanguage];
            }
        }

        int activeEquippedCount = 0;
        int passiveEquippedCount = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            backgroundMenu.SetBackgroundAlpha(1f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarLowPos);
            inventoryMenuDynamic.SetActive(true);
            inventoryMenuStatic.SetActive(true);

            //for(int i = 0; i < equippedActiveItem.Length; i++)
            //{
            //    if(equippedActiveItem[i] != null)
            //    {
            //        activeEquippedCount += 1;
            //    }
            //}
            //for (int i = 0; i < equippedPassiveItem.Length; i++)
            //{
            //    if (equippedActiveItem[i] != null)
            //    {
            //        passiveEquippedCount += 1;
            //    }
            //}

            if (gameManager.playerData.activeItemIDList != null)
            {
                if (gameManager.playerData.activeItemIDList.Count > activeInventorySlotList.Count - 1)
                {
                    for (int i = activeInventorySlotList.Count - 1;
                        i < gameManager.playerData.activeItemIDList.Count; i++)
                    {
                        GameObject obj = Instantiate(inventorySlot, activeContentSlot.transform);
                        activeInventorySlotList.Add(obj);

                        Image img = obj.GetComponent<Image>();
                        ChangeAlpha(img, 0);
                        activeInventoryImageList.Add(img);

                        Text text = obj.transform.GetChild(0).GetComponent<Text>();
                        text.text = itemDatabase.itemContentDict[(itemDatabase.itemIDDict
                            [gameManager.playerData.activeItemIDList[i]].itemIDName + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        activeInventoryTextList.Add(text);

                        Item item = itemDatabase.itemIDDict[gameManager.playerData.activeItemIDList[i]];
                        activeInventoryItemList.Add(item);
                    }
                }
            }
            if (gameManager.playerData.passiveItemIDList != null)
            {
                if (gameManager.playerData.passiveItemIDList.Count > passiveInventorySlotList.Count - 1)
                {
                    for (int i = passiveInventorySlotList.Count - 1;
                        i < gameManager.playerData.passiveItemIDList.Count; i++)
                    {
                        GameObject obj = Instantiate(inventorySlot, passiveContentSlot.transform);
                        passiveInventorySlotList.Add(obj);

                        Image img = obj.GetComponent<Image>();
                        ChangeAlpha(img, 0);
                        passiveInventoryImageList.Add(img);

                        Text text = obj.transform.GetChild(0).GetComponent<Text>();
                        text.text = itemDatabase.itemContentDict[(itemDatabase.itemIDDict
                            [gameManager.playerData.passiveItemIDList[i]].itemIDName + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        passiveInventoryTextList.Add(text);

                        Item item = itemDatabase.itemIDDict[gameManager.playerData.passiveItemIDList[i]];
                        passiveInventoryItemList.Add(item);
                    }
                }
            }

            maxActiveInventorySlotCount = activeInventorySlotList.Count - 1 - activeEquippedCount;
            maxPassiveInventorySlotCount = passiveInventorySlotList.Count - 1 - passiveEquippedCount;

            activeScrollMoveAmount = 1.0f / (maxActiveInventorySlotCount - maxVisibleSlotCount);
            passiveScrollMoveAmount = 1.0f / (maxPassiveInventorySlotCount - maxVisibleSlotCount);

            RowCount = 0;
            SelectedSlotCount = 0;
            SelectedInventorySlotCount = 0;
            passiveScrollBar.value = 1f;
            activeScrollBar.value = 1f;
        }

        public void ChangeAlpha(Image image, float newAlpha)
        {
            Color color = image.color;
            color.a = newAlpha / 255f;
            image.color = color;
        }

        IEnumerator BlinkImage(Image image, float maxAlpha, float minAlpha)
        {
            float alpha = maxAlpha;
            ChangeAlpha(image, alpha);
            while (true)
            {
                while (alpha > minAlpha)
                {
                    ChangeAlpha(image, alpha);
                    alpha -= blinkAmount;
                    yield return null;
                }
                while (alpha < maxAlpha)
                {
                    ChangeAlpha(image, alpha);
                    alpha += blinkAmount;
                    yield return null;
                }
                yield return null;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            isActiveInventory = false;
            if (routine != null)
            {
                StopCoroutine(routine);

                for (int i = 0; i < activeInventoryImageList.Count; i++)
                {
                    ChangeAlpha(activeInventoryImageList[i], 0);
                }
                for (int i = 0; i < passiveInventoryImageList.Count; i++)
                {
                    ChangeAlpha(passiveInventoryImageList[i], 0);
                }
                //ChangeAlpha(rowCount == 0 ? activeSlotImages[SelectedSlotCount]
                //    : passiveSlotImages[SelectedSlotCount], 0);
                //ChangeAlpha(rowCount == 0 ? activeInventoryImageList[SelectedInventorySlotCount]
                //    : passiveInventoryImageList[SelectedInventorySlotCount], 0);
            }
            inventoryMenuDynamic.SetActive(false);
            inventoryMenuStatic.SetActive(false);
        }

        protected override void OperateMenuConfirm()
        {
            if (!isActiveInventory)
            {
                if (routine != null)
                {
                    StopCoroutine(routine);
                    ChangeAlpha(rowCount == 0 ? activeSlotImages[SelectedSlotCount]
                        : passiveSlotImages[SelectedSlotCount], 0);
                }
                isActiveInventory = true;
                SelectedInventorySlotCount = 0;
                return;
            }
            if(RowCount == 0)
            {
                if (activeSlotItemImages[SelectedSlotCount].sprite != null)
                {
                    if(SelectedInventorySlotCount != 0)
                    {
                        Debug.Log(equippedActiveItemSelectNumber
                            [SelectedSlotCount] ?? default);
                        activeInventorySlotList[equippedActiveItemSelectNumber
                            [SelectedSlotCount] ?? default].SetActive(true);

                        activeInventorySlotList[SelectedInventorySlotCount].SetActive(false);
                        equippedActiveItemSelectNumber[SelectedSlotCount] = SelectedInventorySlotCount;
                        equippedActiveItem[SelectedSlotCount] = activeInventoryItemList[SelectedInventorySlotCount];

                        GameManager.instance.SetSlotItem(SelectedSlotCount,
                            activeInventoryItemList[SelectedInventorySlotCount]);
                    }
                    else
                    {
                        activeInventorySlotList[equippedActiveItemSelectNumber
                            [SelectedSlotCount] ?? default].SetActive(true);

                        equippedActiveItemSelectNumber[SelectedSlotCount] = null;
                        equippedActiveItem[SelectedSlotCount] = null;
                        activeEquippedCount -= 1;

                        GameManager.instance.SetSlotItem(SelectedSlotCount, null);
                    }
                }
                else
                {
                    if (SelectedInventorySlotCount != 0)
                    {
                        activeInventorySlotList[SelectedInventorySlotCount].SetActive(false);
                        activeEquippedCount += 1;
                        equippedActiveItemSelectNumber[SelectedSlotCount] = SelectedInventorySlotCount;
                        equippedActiveItem[SelectedSlotCount] = activeInventoryItemList[SelectedInventorySlotCount];

                        GameManager.instance.SetSlotItem(SelectedSlotCount, 
                            activeInventoryItemList[SelectedInventorySlotCount]);
                    }
                    else
                    {

                    }
                }
                if (activeInventoryItemList[SelectedInventorySlotCount].itemIcon != null)
                {
                    ChangeAlpha(activeSlotItemImages[SelectedSlotCount], 255);
                    activeSlotItemImages[SelectedSlotCount].sprite 
                        = activeInventoryItemList[SelectedInventorySlotCount].itemIcon;

                    //GameManager.instance.SetSlotItem(SelectedSlotCount, null);
                }
                else
                {
                    ChangeAlpha(activeSlotItemImages[SelectedSlotCount], 0);
                    activeSlotItemImages[SelectedSlotCount].sprite = null;
                }
            }
            if (RowCount == 1)
            {
                if (passiveSlotItemImages[SelectedSlotCount].sprite != null)
                {
                    if (SelectedInventorySlotCount != 0)
                    {;
                        passiveInventorySlotList[equippedPassiveItemSelectNumber
                            [SelectedSlotCount] ?? default].SetActive(true);

                        passiveInventorySlotList[SelectedInventorySlotCount].SetActive(false);
                        equippedPassiveItemSelectNumber[SelectedSlotCount] = SelectedInventorySlotCount;
                        equippedPassiveItem[SelectedSlotCount] = passiveInventoryItemList[SelectedInventorySlotCount];
                    }
                    else
                    {
                        passiveInventorySlotList[equippedPassiveItemSelectNumber
                            [SelectedSlotCount] ?? default].SetActive(true);

                        equippedPassiveItemSelectNumber[SelectedSlotCount] = null;
                        equippedPassiveItem[SelectedSlotCount] = null;
                        passiveEquippedCount -= 1;
                    }
                }
                else
                {
                    if (SelectedInventorySlotCount != 0)
                    {
                        passiveInventorySlotList[SelectedInventorySlotCount].SetActive(false);
                        passiveEquippedCount += 1;
                        equippedPassiveItemSelectNumber[SelectedSlotCount] = SelectedInventorySlotCount;
                        equippedPassiveItem[SelectedSlotCount] = passiveInventoryItemList[SelectedInventorySlotCount];

                    }
                    else
                    {

                    }
                }
                if (passiveInventoryItemList[SelectedInventorySlotCount].itemIcon != null)
                {
                    ChangeAlpha(passiveSlotItemImages[SelectedSlotCount], 255);
                    passiveSlotItemImages[SelectedSlotCount].sprite
                        = passiveInventoryItemList[SelectedInventorySlotCount].itemIcon;
                }
                else
                {
                    ChangeAlpha(passiveSlotItemImages[SelectedSlotCount], 0);
                    passiveSlotItemImages[SelectedSlotCount].sprite = null;
                }
            }
            if (isActiveInventory)
            {
                isActiveInventory = false;
                if (routine != null)
                {
                    StopCoroutine(routine);
                    ChangeAlpha(rowCount == 0 ? activeInventoryImageList[SelectedInventorySlotCount]
                        : passiveInventoryImageList[SelectedInventorySlotCount], 0);
                }
                routine = StartCoroutine(BlinkImage(RowCount == 0 ? activeSlotImages[SelectedSlotCount]
                : passiveSlotImages[SelectedSlotCount], maxSlotBlinkAlpha, 0));

                if(RowCount == 0)
                {
                    if (activeSlotItemImages[SelectedSlotCount].sprite != null)
                    {
                        itemNameText.text = itemDatabase.itemContentDict
                            [(activeSlotItemImages[SelectedSlotCount].sprite.name + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemEffectText.text = itemDatabase.itemContentDict
                            [(activeSlotItemImages[SelectedSlotCount].sprite.name + "Effect").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemDescriptionText.text = itemDatabase.itemContentDict
                            [(activeSlotItemImages[SelectedSlotCount].sprite.name + "Description").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                    }
                    else
                    {
                        itemNameText.text = string.Empty;
                        itemEffectText.text = string.Empty;
                        itemDescriptionText.text = string.Empty;

                    }
                }
                else if(RowCount == 1)
                {
                    if (passiveSlotItemImages[SelectedSlotCount].sprite != null)
                    {
                        itemNameText.text = itemDatabase.itemContentDict
                            [(passiveSlotItemImages[SelectedSlotCount].sprite.name + "Name").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemEffectText.text = itemDatabase.itemContentDict
                            [(passiveSlotItemImages[SelectedSlotCount].sprite.name + "Effect").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                        itemDescriptionText.text = itemDatabase.itemContentDict
                            [(passiveSlotItemImages[SelectedSlotCount].sprite.name + "Description").GetHashCode()]
                            [localizeManager.CurrentLanguage];
                    }
                    else
                    {
                        itemNameText.text = string.Empty;
                        itemEffectText.text = string.Empty;
                        itemDescriptionText.text = string.Empty;
                    }
                }

                SelectedInventorySlotCount = 0;
                activeScrollBar.value = 1f;
                passiveScrollBar.value = 1f;
                SelectedSlotCount = SelectedSlotCount;
                return;
            }
        }
        protected override void OperateMenuCancle()
        {
            if (isActiveInventory)
            {
                isActiveInventory = false;
                if (routine != null)
                {
                    StopCoroutine(routine);
                    ChangeAlpha(rowCount == 0 ? activeInventoryImageList[SelectedInventorySlotCount]
                        : passiveInventoryImageList[SelectedInventorySlotCount], 0);
                }
                routine = StartCoroutine(BlinkImage(RowCount == 0 ? activeSlotImages[SelectedSlotCount]
                : passiveSlotImages[SelectedSlotCount], maxSlotBlinkAlpha, 0));

                SelectedInventorySlotCount = 0;
                activeScrollBar.value = 1f;
                passiveScrollBar.value = 1f;
                SelectedSlotCount = SelectedSlotCount;

                return;
            }
            base.OperateMenuCancle();
        }
        public override void CheckArrowKey()
        {
            //if (equippedActiveItem[0] != null)
            //{
            //    Debug.Log(equippedActiveItem[0].itemCount);
            //}
            if (isActiveInventory)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    SelectedInventorySlotCount += 1;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    SelectedInventorySlotCount -= 1;
                }
                return;
            }

            //if (isActiveInventory)
            //{
            //    if (Input.GetKeyDown(KeyCode.DownArrow))
            //    {
            //        if (SelectedInventorySlotCount + 1 > (RowCount == 0 ?
            //            activeInventorySlotList.Count - 1 :
            //            passiveInventorySlotList.Count - 1 ))
            //        {
            //            SelectedInventorySlotCount = 0;
            //            if(RowCount == 0)
            //            {
            //                activeScrollBar.value = 1f;
            //            }
            //            else if(RowCount == 1)
            //            {
            //                passiveScrollBar.value = 1f;
            //            }
            //        }
            //        else
            //        {
            //            SelectedInventorySlotCount += 1;
            //        }
            //        moveCount = (1 - (RowCount == 0 ? activeScrollBar.value : passiveScrollBar.value)) * (RowCount == 0 ?
            //            (activeInventorySlotList.Count - 1 - activeEquippedCount - maxVisialbeSlotCount) :
            //            (passiveInventorySlotList.Count - 1 - passiveEquippedCount - maxVisialbeSlotCount));
            //        Debug.Log(Mathf.CeilToInt(moveCount));
            //        if (SelectedInventorySlotCount >= maxVisialbeSlotCount + Mathf.CeilToInt(moveCount) + (RowCount == 0 ?
            //            activeEquippedCount : passiveEquippedCount))
            //        {
            //            if(RowCount == 0)
            //            {
            //                activeScrollMoveAmount = 1.0f / 
            //                    (activeInventorySlotList.Count - activeEquippedCount - maxVisialbeSlotCount);
            //                activeScrollBar.value -= activeScrollMoveAmount;
            //                activeScrollBar.value = Mathf.Clamp01(activeScrollBar.value);
            //            }
            //            else if (RowCount == 1)
            //            {
            //                passiveScrollMoveAmount = 1.0f / (
            //                    passiveInventorySlotList.Count - passiveEquippedCount - maxVisialbeSlotCount);
            //                passiveScrollBar.value -= passiveScrollMoveAmount;
            //                passiveScrollBar.value = Mathf.Clamp01(passiveScrollBar.value);
            //            }
            //        }

            //        return;
            //    }
            //    else if (Input.GetKeyDown(KeyCode.UpArrow))
            //    {
            //        if (SelectedInventorySlotCount - 1 < 0)
            //        {
            //            SelectedInventorySlotCount = (RowCount == 0 ?
            //                activeInventorySlotList.Count - 1  :
            //                passiveInventorySlotList.Count - 1);
            //            if (RowCount == 0)
            //            {
            //                activeScrollBar.value = 0f;
            //            }
            //            else if (RowCount == 1)
            //            {
            //                passiveScrollBar.value = 0f;
            //            }
            //        }
            //        else
            //        {
            //            SelectedInventorySlotCount -= 1;
            //        }
            //        moveCount = (1 - (RowCount == 0 ? activeScrollBar.value : passiveScrollBar.value)) * (RowCount == 0 ?
            //            (activeInventorySlotList.Count - 1 - activeEquippedCount - maxVisialbeSlotCount) :
            //            (passiveInventorySlotList.Count - 1 - passiveEquippedCount - maxVisialbeSlotCount));
            //        Debug.Log(Mathf.CeilToInt(moveCount));
            //        if (SelectedInventorySlotCount <= Mathf.FloorToInt(moveCount) + (RowCount == 0 ?
            //            activeEquippedCount : passiveEquippedCount))
            //        {
            //            if (RowCount == 0)
            //            {
            //                activeScrollMoveAmount = 1.0f /
            //                    (activeInventorySlotList.Count - activeEquippedCount - maxVisialbeSlotCount);
            //                activeScrollBar.value += activeScrollMoveAmount;
            //                activeScrollBar.value = Mathf.Clamp01(activeScrollBar.value);
            //            }
            //            else if (RowCount == 1)
            //            {
            //                passiveScrollMoveAmount = 1.0f / (
            //                    passiveInventorySlotList.Count - passiveEquippedCount - maxVisialbeSlotCount);
            //                passiveScrollBar.value += passiveScrollMoveAmount;
            //                passiveScrollBar.value = Mathf.Clamp01(passiveScrollBar.value);
            //            }
            //        }
            //        //activeScrollBar.value = 1f;
            //        return;
            //    }
            //    return;
            //}
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if(RowCount + 1 > maxRowCount)
                {
                    RowCount = 0;
                }
                else
                {
                    RowCount += 1;
                }
                return;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (RowCount - 1 < 0)
                {
                    RowCount = maxRowCount;
                }
                else
                {
                    RowCount -= 1;
                }
                return;
            }
            if(RowCount == 0)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (SelectedSlotCount + 1 > maxActiveSlotCount)
                    {
                        SelectedSlotCount = 0;
                    }
                    else
                    {
                        SelectedSlotCount += 1;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (SelectedSlotCount - 1 < 0)
                    {
                        SelectedSlotCount = maxActiveSlotCount;
                    }
                    else
                    {
                        SelectedSlotCount -= 1;
                    }
                }
            }
            else if(RowCount == 1)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (SelectedSlotCount + 1 > maxPassiveSlotCount)
                    {
                        SelectedSlotCount = 0;
                    }
                    else
                    {
                        SelectedSlotCount += 1;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (SelectedSlotCount - 1 < 0)
                    {
                        SelectedSlotCount = maxPassiveSlotCount;
                    }
                    else
                    {
                        SelectedSlotCount -= 1;
                    }
                }
            }
        }
    }
}