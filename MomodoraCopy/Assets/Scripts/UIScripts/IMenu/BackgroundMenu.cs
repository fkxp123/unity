using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMenu : MonoBehaviour
{
    public GameObject menuBackground;
    public GameObject keyDescriptionBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
