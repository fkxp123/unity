using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class HealthBar : MonoBehaviour
    {
        public Image fillImage;
        public Image effectImage;
        public Image blinkImage;
        public Image backGroundImage;
        public Image itemSlotBackground;
        public Image scoreImage;
        public Text scoreText;
        public GameObject itemSlots;

        Image[] itemImages = new Image[3];
        Text[] itemTexts = new Text[3];

        public static float currentHp;
        public static float maxHp;

        public float hurtSpeed = 0.005f;
        public static float setPositionX;

        public GameObject playerObject;
        PlayerStatus playerStatus;

        public float blinkCycle;
        WaitForSeconds blinkTime;

        Animator blinkAnimator;

        void Awake()
        {
            playerStatus = playerObject.GetComponent<PlayerStatus>();
            blinkAnimator = blinkImage.GetComponent<Animator>();

            for (int i = 0; i < itemSlots.transform.childCount; i++)
            {
                itemImages[i] = itemSlots.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>();
                itemTexts[i] = itemSlots.transform.GetChild(i).transform.GetChild(1).GetComponent<Text>();
            }
        }

        void Start()
        {
            setPositionX = blinkImage.rectTransform.anchoredPosition.x;
            maxHp = playerStatus.maxHp;
            currentHp = maxHp;
            blinkCycle = 0.01f;
            blinkTime = new WaitForSeconds(blinkCycle);

            EventManager.instance.AddListener(EventType.GamePause, OnGamePause);
            EventManager.instance.AddListener(EventType.GameResume, OnGameResume);
        }
        public void ChangeAlpha(Image image, float newAlpha)
        {
            Color color = image.color;
            color.a = newAlpha;
            image.color = color;
        }
        public void ChangeAlpha(Text text, float newAlpha)
        {
            Color color = text.color;
            color.a = newAlpha;
            text.color = color;
        }
        void OnGamePause()
        {
            HideUI();
        }
        void OnGameResume()
        {
            ShowUI();
        }

        IEnumerator LoseFillAmount()
        {
            fillImage.fillAmount = currentHp / maxHp;
            while(effectImage.fillAmount != fillImage.fillAmount)
            {
                if (effectImage.fillAmount > fillImage.fillAmount)
                {
                    effectImage.fillAmount -= hurtSpeed;
                }
                else
                {
                    effectImage.fillAmount = fillImage.fillAmount;
                }
                SetBlinkImg();
                yield return blinkTime;
            }
        }

        public void HideUI()
        {
            ChangeAlpha(fillImage, 0);
            ChangeAlpha(effectImage, 0);
            ChangeAlpha(blinkImage, 0);
            ChangeAlpha(backGroundImage, 0);
            ChangeAlpha(itemSlotBackground, 0);
            ChangeAlpha(scoreImage, 0);
            ChangeAlpha(scoreText, 0);
            for(int i = 0; i < itemSlots.transform.childCount; i++)
            {
                ChangeAlpha(itemImages[i], 0);
                ChangeAlpha(itemTexts[i], 0);
            }
            blinkAnimator.enabled = false;
        }
        public void ShowUI()
        {
            ChangeAlpha(fillImage, 1);
            ChangeAlpha(effectImage, 1);
            ChangeAlpha(blinkImage, 1);
            ChangeAlpha(backGroundImage, 1);
            ChangeAlpha(itemSlotBackground, 1);
            ChangeAlpha(scoreImage, 1);
            ChangeAlpha(scoreText, 1);
            for (int i = 0; i < itemSlots.transform.childCount; i++)
            {
                ChangeAlpha(itemImages[i], 1);
                ChangeAlpha(itemTexts[i], 1);
            }
            blinkAnimator.enabled = true;
        }

        public void PoisonedHealth()
        {
            fillImage.color = new Color(0, 255, 0);
            effectImage.color = new Color(0, 150, 0);
            backGroundImage.color = new Color(0, 150, 0);
        }
        public void NormalHealth()
        {
            fillImage.color = new Color(255, 255, 255);
            effectImage.color = new Color(255, 255, 255);
            backGroundImage.color = new Color(255, 255, 255);
        }

        public void LoseHp(float hp)
        {
            currentHp -= hp;
            StartCoroutine("LoseFillAmount");
        }

        public void SetBlinkImg()
        {
            float moveDistance = (1 - effectImage.fillAmount) * 100;
            blinkImage.rectTransform.anchoredPosition =
                new Vector2(setPositionX - moveDistance, blinkImage.rectTransform.anchoredPosition.y);
        }
    }

}