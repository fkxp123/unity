using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;


namespace MomodoraCopy
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public GameObject chatBox;
        public GameObject interactionBox;
        public Text chatContext;
        public GameObject nextButton;
        RectTransform backgroundRectTransform;
        public RectTransform nextButtonRectTransform;

        void Start()
        {
            backgroundRectTransform = chatBox.transform.GetChild(0).GetComponent<RectTransform>();
            nextButtonRectTransform = nextButton.transform.GetComponent<RectTransform>();
        }

        public List<string> GetDialogue(string _CSVFileName, int localNumber)
        {
            List<string> dialogueList = new List<string>();
            TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

            string[] data = csvData.text.Split(new char[] { '\n' });

            for (int i = 1; i < data.Length - 1; i++)
            {
                string[] row = data[i].Split(new char[] { ',' });
                string context = row[localNumber];
                context = context.Replace("<br>", "\n");
                context = context.Replace("<c>", ",");
                context = context.Replace("<..>", "..");

                dialogueList.Add(context);
            }

            return dialogueList;
        }
        public void SetChatBox(string context)
        {
            string[] contexts = context.Split(new char[] { '\n' });

            int maxLength = contexts[0].Length;
            for(int i = 0; i < contexts.Length; i++)
            {
                if(maxLength < contexts[i].Length)
                {
                    maxLength = contexts[i].Length;
                }
            }

            backgroundRectTransform.sizeDelta = 
                new Vector2((maxLength * 10) + 8, (contexts.Length) * 10 + 22);
            backgroundRectTransform.anchoredPosition = 
                new Vector3(backgroundRectTransform.sizeDelta.x * 0.5f, 
                backgroundRectTransform.sizeDelta.y * 0.5f, 0);
            nextButtonRectTransform.anchoredPosition =
                new Vector3((backgroundRectTransform.sizeDelta.x * 0.5f) - 4,
                (-1 * backgroundRectTransform.sizeDelta.y * 0.5f) + 2 + 4, 0);
        }
        public Vector3 GetBackgroundSize()
        {
            return backgroundRectTransform.sizeDelta;
        }
    }
}
