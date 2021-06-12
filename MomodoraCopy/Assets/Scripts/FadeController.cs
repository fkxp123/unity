using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class FadeController : MonoBehaviour
    {
        public Image mask;
        RectTransform rectTransform;
        float fadeAmount = 0;
        float fadeSpeed = 100;
        float speedAccel = 5000f;

        public GameObject ui;
        GameObject invertMask;

        void Awake()
        {
            ui.SetActive(false);
        }
        void Start()
        {
            rectTransform = mask.GetComponent<RectTransform>();
            invertMask = mask.transform.GetChild(0).gameObject;
            //invertMask.SetActive(false);
        }
        public void Fade()
        {
            Debug.Log("hi");
            invertMask.SetActive(true);
            if (fadeAmount <= 0)
            {
                StartCoroutine(FadeIn());
            }
            else
            {
                StartCoroutine(FadeOut());
            }
        }
        IEnumerator FadeIn()
        {
            yield return null;
            transform.position = GameManager.instance.playerPhysics.transform.position;
            while (fadeAmount < 1200)
            {
                fadeAmount += Time.deltaTime * fadeSpeed;
                fadeSpeed += Time.deltaTime * speedAccel;
                if(fadeAmount >= 600)
                {
                    ui.SetActive(true);
                }
                rectTransform.sizeDelta = new Vector2(fadeAmount, fadeAmount);
                yield return null;
            }
            invertMask.SetActive(false);
        }
        IEnumerator FadeOut()
        {
            yield return null;
            ui.SetActive(false);
            transform.position = GameManager.instance.playerPhysics.transform.position;
            while (fadeAmount > 0)
            {
                fadeAmount -= Time.deltaTime * fadeSpeed;
                fadeSpeed += Time.deltaTime * speedAccel;
                rectTransform.sizeDelta = new Vector2(fadeAmount, fadeAmount);
                yield return null;
            }
        }
    } 
}