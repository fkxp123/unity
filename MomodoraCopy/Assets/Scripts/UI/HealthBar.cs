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
        public Image itemSlotImage;
        public Image scoreImage;

        public static float currentHp;
        public static float maxHp;

        public float hurtSpeed = 0.005f;
        public static float setPositionX;

        public GameObject playerObject;
        PlayerStatus playerStatus;

        public float blinkCycle;
        WaitForSeconds blinkTime;

        Animator blinkAnimator;

        // Start is called before the first frame update
        void Start()
        {
            playerStatus = playerObject.GetComponent<PlayerStatus>();
            blinkAnimator = blinkImage.GetComponent<Animator>();
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
        void OnGamePause()
        {
            ChangeAlpha(fillImage, 0);
            ChangeAlpha(effectImage, 0);
            ChangeAlpha(blinkImage, 0);
            ChangeAlpha(backGroundImage, 0);
            ChangeAlpha(itemSlotImage, 0);
            ChangeAlpha(scoreImage, 0);
            blinkAnimator.enabled = false;
        }
        void OnGameResume()
        {
            ChangeAlpha(fillImage, 1);
            ChangeAlpha(effectImage, 1);
            ChangeAlpha(blinkImage, 1);
            ChangeAlpha(backGroundImage, 1);
            ChangeAlpha(itemSlotImage, 1);
            ChangeAlpha(scoreImage, 1);
            blinkAnimator.enabled = true;
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