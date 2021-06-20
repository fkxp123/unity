using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class BackgroundMenu : MonoBehaviour
    {
        public GameObject menuBackground;
        public GameObject keyDescriptionBar;

        Text confirmKeyText;
        Text cancleKeyText;

        LocalizeManager localizeManager;

        void Start()
        {
            localizeManager = LocalizeManager.instance;

            confirmKeyText = keyDescriptionBar.transform.GetChild(0).GetComponent<Text>();
            cancleKeyText = keyDescriptionBar.transform.GetChild(1).GetComponent<Text>();
            confirmKeyText.text = string.Format("{0} : A", localizeManager.descriptionsDict
                ["ConfirmKeyDesc".GetHashCode()][localizeManager.CurrentLanguage]);
            cancleKeyText.text = string.Format("{0} : S", localizeManager.descriptionsDict
                ["CancleKeyDesc".GetHashCode()][localizeManager.CurrentLanguage]);

            EventManager.instance.AddListener(EventType.LanguageChanged, OnLanguageChanged);
        }

        void OnLanguageChanged()
        {
            confirmKeyText.text = string.Format("{0} : A", localizeManager.descriptionsDict
                ["ConfirmKeyDesc".GetHashCode()][localizeManager.CurrentLanguage]);
            cancleKeyText.text = string.Format("{0} : S", localizeManager.descriptionsDict
                ["CancleKeyDesc".GetHashCode()][localizeManager.CurrentLanguage]);
        }

        public void SetBackgroundAlpha(float alpha)
        {
            Image img = menuBackground.GetComponent<Image>();
            Color temp = img.color;
            temp.a = alpha;
            img.color = temp;
        }
        public void SetKeyDescriptionBarPosition(Vector3 position)
        {
            keyDescriptionBar.GetComponent<RectTransform>().localPosition = position;
        }
    }

}
