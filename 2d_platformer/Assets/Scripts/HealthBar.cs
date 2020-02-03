using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public static HealthBar instance;
    public Image fill;
    public Image effectImg;
    public Image blinkImg;

    public float currentHp;
    public float Hp;
    public float hurtSpeed = 0.005f;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = Hp;
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
        if(effectImg.fillAmount > fill.fillAmount)
        {
            effectImg.fillAmount -= hurtSpeed;
        }
        else
        {
            effectImg.fillAmount = fill.fillAmount;
        } 
    }
    public void SetBlinkImg(float moveDistance)
    {
        if(currentHp >= 0)
        {
            blinkImg.rectTransform.anchoredPosition =
            new Vector2(blinkImg.rectTransform.anchoredPosition.x - moveDistance, blinkImg.rectTransform.anchoredPosition.y);
        }
    }
}
