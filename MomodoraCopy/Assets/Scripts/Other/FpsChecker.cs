using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class FpsChecker : MonoBehaviour
    {
        [Range(1, 100)]
        public int fontSize;
        [Range(0, 1)]
        public float red, green, blue;

        float deltaTime = 0.0f;

        void Start()
        {
            fontSize = fontSize == 0 ? 50 : fontSize;
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            int w = Screen.width; 
            int h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = h * 2 / fontSize;
            style.normal.textColor = new Color(red, green, blue, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }

}