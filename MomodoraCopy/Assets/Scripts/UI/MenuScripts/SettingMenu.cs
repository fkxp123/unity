using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace MomodoraCopy
{
    public class ArrowMenuContent
    {
        public Image leftArrow;
        public Image rightArrow;
        public RectTransform leftArrowRect;
        public RectTransform rightArrowRect;
        public Text localLanguageText;
    }
    public class SettingMenu : AbstractMenu
    {
        #region SettingMenu
        public GameObject settingMenuSlot;
        public GameObject selectedSettingMenuSlot;
        public GameObject selectedContentSlot;
        public Text selectedMenuInfo;

        [Header("SettingMenu")]
        public GameObject keyChangeMenuObject;

        MainMenu mainMenu;
        KeyChangeMenu keyChangeMenu;

        GameObject[] selectedSettingMenuSlots;
        GameObject[] selectedContentSlots;
        GameObject[] settingMenuSlots;
        Text[] descriptionText;
        Image[] slotImage;
        int slotCount;
        #endregion

        string settingsText;
        LocalizeManager localizeManager;

        ArrowMenuContent arrowContent;
        bool arrowContentActivated;
        float blinkArrowCycle;
        WaitForSeconds blinkTime;

        int localLanguageCount;
        
        protected override void Awake()
        {
            base.Awake();
            localizeManager = LocalizeManager.instance;
            settingMenuSlot.SetActive(false);
            selectedSettingMenuSlot.SetActive(false);
            selectedContentSlot.SetActive(false);
            keyChangeMenu = keyChangeMenuObject.GetComponent<KeyChangeMenu>();

            selectedSettingMenuSlots = new GameObject[selectedSettingMenuSlot.transform.childCount];
            selectedContentSlots = new GameObject[selectedContentSlot.transform.childCount];
            settingMenuSlots = new GameObject[settingMenuSlot.transform.childCount];
            slotImage = new Image[selectedSettingMenuSlot.transform.childCount];
            descriptionText = new Text[settingMenuSlot.transform.childCount];

            for (int i = 0; i < selectedSettingMenuSlot.transform.childCount; i++)
            {
                selectedSettingMenuSlots[i] = selectedSettingMenuSlot.transform.GetChild(i).gameObject;
                slotImage[i] = selectedSettingMenuSlots[i].GetComponent<Image>();
            }

            for(int i = 0; i < settingMenuSlot.transform.childCount; i++)
            {
                settingMenuSlots[i] = settingMenuSlot.transform.GetChild(i).gameObject;
                descriptionText[i] = settingMenuSlots[i].transform.GetChild(0).GetComponent<Text>();
            }

            for(int i = 0; i < selectedContentSlot.transform.childCount; i++)
            {
                selectedContentSlots[i] = selectedContentSlot.transform.GetChild(i).gameObject;
            }

            settingsText = localizeManager.descriptionsDict
                ["SettingsDesc".GetHashCode()][localizeManager.CurrentLanguage];

            for(int i = 0; i < settingMenuSlot.transform.childCount; i++)
            {
                descriptionText[i].text = localizeManager.descriptionsDict
                    [descriptionText[i].name.GetHashCode()][localizeManager.CurrentLanguage];
            }

            arrowContent = new ArrowMenuContent
            {
                localLanguageText = selectedContentSlots[7].transform.GetChild(0).GetComponent<Text>(),
                leftArrow = selectedContentSlots[7].transform.GetChild(1).GetComponent<Image>(),
                rightArrow = selectedContentSlots[7].transform.GetChild(2).GetComponent<Image>(),
                leftArrowRect = selectedContentSlots[7].transform.GetChild(1).GetComponent<RectTransform>(),
                rightArrowRect = selectedContentSlots[7].transform.GetChild(2).GetComponent<RectTransform>()
            };
            arrowContent.localLanguageText.text = localizeManager.descriptionsDict
                [arrowContent.localLanguageText.name.GetHashCode()][localizeManager.CurrentLanguage];

            blinkArrowCycle = 0.1f;
            blinkTime = new WaitForSeconds(blinkArrowCycle);

            EventManager.instance.AddListener(EventType.LanguageChanged, OnLanguageChanged);
        }

        IEnumerator BlinkArrowImage(RectTransform leftRect, RectTransform rightRect)
        {
            float i = 0;
            while (arrowContentActivated)
            {
                i++;
                if (i % 2 == 0)
                {
                    leftRect.anchoredPosition =
                        new Vector3(leftRect.anchoredPosition.x - 1, leftRect.anchoredPosition.y, 0);
                    rightRect.anchoredPosition =
                        new Vector3(rightRect.anchoredPosition.x + 1, rightRect.anchoredPosition.y, 0);
                }
                else
                {
                    leftRect.anchoredPosition =
                        new Vector3(leftRect.anchoredPosition.x + 1, leftRect.anchoredPosition.y, 0);
                    rightRect.anchoredPosition =
                        new Vector3(rightRect.anchoredPosition.x - 1, rightRect.anchoredPosition.y, 0);
                }
                //why doesnt work?? : yield return blinkTime;
                for(int k = 0; k < 20; k++)
                {
                    yield return null;
                }
            }
        }

        void OnLanguageChanged()
        {
            settingsText = localizeManager.descriptionsDict
                ["SettingsDesc".GetHashCode()][localizeManager.CurrentLanguage];
            selectedMenuInfo.text = settingsText;

            for (int i = 0; i < settingMenuSlot.transform.childCount; i++)
            {
                descriptionText[i].text = localizeManager.descriptionsDict
                    [descriptionText[i].name.GetHashCode()][localizeManager.CurrentLanguage];
            }

            arrowContent.localLanguageText.text = localizeManager.descriptionsDict
                [arrowContent.localLanguageText.name.GetHashCode()][localizeManager.CurrentLanguage];
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
            selectedContentSlot.SetActive(true);
            selectedMenuInfo.gameObject.SetActive(true);
            selectedMenuInfo.text = settingsText;
            backgroundMenu.SetBackgroundAlpha(1f);
            backgroundMenu.SetKeyDescriptionBarPosition(KeyDescriptionBarLowPos);

            ChangeSelectedSlotMenu(slotImage, slotCount);

            localLanguageCount = (int)localizeManager.CurrentLanguage;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            arrowContent.localLanguageText.text = localizeManager.descriptionsDict
                [arrowContent.localLanguageText.name.GetHashCode()][localizeManager.CurrentLanguage];

            settingMenuSlot.SetActive(false);
            selectedSettingMenuSlot.SetActive(false);
            selectedContentSlot.SetActive(false);
            selectedMenuInfo.gameObject.SetActive(false);
        }

        protected override void OperateMenuConfirm()
        {
            if (arrowContentActivated)
            {
                arrowContentActivated = false;
                return;
            }
            if (slotCount == 6)
            {
                enabled = false;
                MenuManager.instance.menuMemento.SetMenuMemento(gameObject);
                keyChangeMenu.enabled = true;
            }
            else if (slotCount == 7)
            {
                arrowContentActivated = true;
                StartCoroutine(BlinkArrowImage(arrowContent.leftArrowRect, arrowContent.rightArrowRect));
            }
            else if(slotCount == 9)
            {
                localizeManager.CurrentLanguage = (Language)localLanguageCount;
            }
        }
        protected override void OperateMenuCancle()
        {
            if (arrowContentActivated)
            {
                arrowContentActivated = false;
                return;
            }
            base.OperateMenuCancle();
        }

        public override void CheckArrowKey()
        {
            if (!arrowContentActivated)
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
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if(slotCount == 7)
                    {
                        localLanguageCount -= 1;
                        if(localLanguageCount < 0)
                        {
                            localLanguageCount = System.Enum.GetValues(typeof(Language)).Length - 1;
                        }
                        arrowContent.localLanguageText.text = localizeManager.descriptionsDict
                            [arrowContent.localLanguageText.name.GetHashCode()]
                            [(Language)localLanguageCount];
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (slotCount == 7)
                    {
                        localLanguageCount += 1;
                        if (localLanguageCount > System.Enum.GetValues(typeof(Language)).Length - 1)
                        {
                            localLanguageCount = 0;
                        }
                        arrowContent.localLanguageText.text = localizeManager.descriptionsDict
                            [arrowContent.localLanguageText.name.GetHashCode()]
                            [(Language)localLanguageCount];
                    }
                }
            }
        }

    }
}