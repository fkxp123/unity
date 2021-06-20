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

        [TextArea]
        public string dialogueContext;
        public int contextCount;

        public float typingCycle = 0.1f;
        WaitForSeconds typingDelay;
        public bool isTyping;
        public float blinkCycle = 0.2f;
        WaitForSeconds blinkTime;

        public bool isChatting;

        public string[] dialogues;
        public Dictionary<int, Dictionary<Language, List<string>>> dialogueDictionary = 
            new Dictionary<int, Dictionary<Language, List<string>>>();

        public Dictionary<Language, int> letterSizeDictionary = new Dictionary<Language, int>();

        float currentTotalTypingDelay;

        LocalizeManager localizeManager;
        public bool ableToPassNext;

        void Start()
        {
            localizeManager = LocalizeManager.instance;

            backgroundRectTransform = chatBox.transform.GetChild(0).GetComponent<RectTransform>();
            nextButtonRectTransform = nextButton.transform.GetComponent<RectTransform>();

            typingDelay = new WaitForSeconds(typingCycle);
            blinkTime = new WaitForSeconds(blinkCycle);

            SetLetterSizeDict();

            string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "Resources"), "*.*");
            foreach (string sourceFile in files)
            {
                string fileName = Path.GetFileName(sourceFile);
                if (fileName.Substring(fileName.Length - 4) == ".csv")
                {
                    string dialogueName = fileName.Substring(0, fileName.Length - 4);
                    Dictionary<Language, List<string>> localDictionary = new Dictionary<Language, List<string>>();
                    for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
                    {
                        localDictionary.Add((Language)i, GetDialogue(dialogueName, i));
                    }
                    dialogueDictionary.Add(dialogueName.GetHashCode(), localDictionary);
                }
            }

            EventManager.instance.AddListener(EventType.LanguageChanged, OnLanguageChange);
        }

        void OnLanguageChange()
        {
            HideChatBox();
        }

        public void SetLetterSizeDict()
        {
            letterSizeDictionary.Add(Language.Korean, 10);
            letterSizeDictionary.Add(Language.English, 6);
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
                new Vector2((maxLength * letterSizeDictionary[localizeManager.CurrentLanguage]) + 8, 
                            contexts.Length * 10 + 22);
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

        public void ShowChatBox(string dialogueName)
        {
            transform.position = transform.position;
            chatBox.SetActive(true);
            if (dialogueDictionary[dialogueName.GetHashCode()][localizeManager.CurrentLanguage].Count == 0)
            {
                return;
            }
            ShowChatContext(dialogueName);
        }
        public void ShowChatContext(string dialogueName)
        {
            dialogueContext = dialogueDictionary[dialogueName.GetHashCode()][localizeManager.CurrentLanguage][contextCount];
            StartCoroutine(TypeContexts(dialogueContext));
            SetChatBox(dialogueDictionary[dialogueName.GetHashCode()][localizeManager.CurrentLanguage][contextCount]);
            Vector3 backgroundSize = GetBackgroundSize();
            chatBox.transform.position = new Vector3(chatBox.transform.position.x,
                transform.position.y + backgroundSize.y * 0.035f, transform.position.y);
            contextCount++;
        }

        public void ShowChatBox(string dialogueName, int contextLine)
        {
            transform.position = transform.position;
            chatBox.SetActive(true);
            if (dialogueDictionary[dialogueName.GetHashCode()][localizeManager.CurrentLanguage].Count == 0)
            {
                return;
            }
            ShowChatContext(dialogueName, contextLine);
        }
        public void ShowChatContext(string dialogueName, int contextLine)
        {
            dialogueContext = dialogueDictionary[dialogueName.GetHashCode()][localizeManager.CurrentLanguage][contextLine];
            StartCoroutine(TypeContexts(dialogueContext));
            SetChatBox(dialogueDictionary[dialogueName.GetHashCode()][localizeManager.CurrentLanguage][contextLine]);
            Vector3 backgroundSize = GetBackgroundSize();
            chatBox.transform.position = new Vector3(chatBox.transform.position.x,
                transform.position.y + backgroundSize.y * 0.035f, transform.position.y);
        }

        public void HideChatBox()
        {
            isTyping = false;
            isChatting = false;
            contextCount = 0;
            chatContext.text = string.Empty;
            ableToPassNext = true;
            chatBox.SetActive(false);
        }
        IEnumerator TypeContexts(string context)
        {
            ableToPassNext = false;
            isTyping = true;
            for (int i = 0; i <= context.Length; i++)
            {
                if (!isChatting)
                {
                    chatContext.text = string.Empty;
                    HideChatBox();
                    yield break;
                }
                chatContext.text = context.Substring(0, i);
                yield return typingDelay;
            }
            if (!isChatting)
            {
                chatContext.text = string.Empty;
                HideChatBox();
                yield break;
            }
            isTyping = false;
            StartCoroutine(BlinkNextButton());
            ableToPassNext = true;
        }

        IEnumerator BlinkNextButton()
        {
            float i = 0;
            RectTransform rectTransform = nextButtonRectTransform;
            while (!isTyping)
            {
                i++;
                if (i % 2 == 0)
                {
                    rectTransform.anchoredPosition =
                        new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 1, 0);
                }
                else
                {
                    rectTransform.anchoredPosition =
                        new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - 1, 0);

                }
                yield return blinkTime;
            }
        }

    }
}
