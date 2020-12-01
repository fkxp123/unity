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
        
        void Awake()
        {
            keyDescriptionBar.transform.GetChild(0).GetComponent<Text>().text = 
                string.Format("확인 : {0}", KeyboardManager.instance.JumpKey.ToString());
            keyDescriptionBar.transform.GetChild(1).GetComponent<Text>().text =
                string.Format("취소 : {0}", KeyboardManager.instance.AttackKey.ToString());
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
