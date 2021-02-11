using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class HealthBar : MonoBehaviour
    {
        public Image fill;
        public Image effectImg;
        public Image blinkImg;

        public static float currentHp;
        public static float maxHp;

        public float hurtSpeed = 0.005f;
        public static float setPositionX;

        public GameObject playerObject;
        PlayerStatus playerStatus;

        // Start is called before the first frame update
        void Start()
        {
            playerStatus = playerObject.GetComponent<PlayerStatus>();
            setPositionX = blinkImg.rectTransform.anchoredPosition.x;
            maxHp = playerStatus.maxHp;
            currentHp = maxHp;
        }

        IEnumerator LoseFillAmount()
        {
            fill.fillAmount = currentHp / maxHp;
            while(effectImg.fillAmount != fill.fillAmount)
            {
                if (effectImg.fillAmount > fill.fillAmount)
                {
                    effectImg.fillAmount -= hurtSpeed;
                }
                else
                {
                    effectImg.fillAmount = fill.fillAmount;
                }
                SetBlinkImg();
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void LoseHp(float hp)
        {
            currentHp -= hp;
            StartCoroutine("LoseFillAmount");
        }

        public void SetBlinkImg()
        {
            float moveDistance = (1 - effectImg.fillAmount) * 100;
            blinkImg.rectTransform.anchoredPosition =
                new Vector2(setPositionX - moveDistance, blinkImg.rectTransform.anchoredPosition.y);
        }
    }

}