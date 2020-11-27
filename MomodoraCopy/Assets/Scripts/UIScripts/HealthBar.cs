using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class HealthBar : MonoBehaviour
    {
        public static HealthBar instance;
        public Image fill;
        public Image effectImg;
        public Image blinkImg;

        public float currentHp;
        public float Hp;
        public float hurtSpeed = 0.005f;
        public float setPositionX;
        PlayerStatus ps;
        // Start is called before the first frame update
        void Start()
        {
            ps = PlayerStatus.instance;
            Hp = ps.Hp;
            currentHp = Hp;
            setPositionX = blinkImg.rectTransform.anchoredPosition.x;
        }
        #region Singleton
        private void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion Sigleton

        // Update is called once per frame
        void Update()
        {
            fill.fillAmount = currentHp / Hp;
            if (effectImg.fillAmount > fill.fillAmount)
            {
                effectImg.fillAmount -= hurtSpeed;
            }
            else
            {
                effectImg.fillAmount = fill.fillAmount;
            }
        }
        public void SetBlinkImg()
        {
            float moveDistance = (1 - currentHp / Hp) * 100;//fill.x pixel size * scale(2)
            if (currentHp >= 0)
            {
                blinkImg.rectTransform.anchoredPosition =
                new Vector2(setPositionX - moveDistance, blinkImg.rectTransform.anchoredPosition.y);
            }
        }
    }

}