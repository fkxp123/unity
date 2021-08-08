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
        public List<string> dialogueNameList = new List<string>();
        public Dictionary<int, Dictionary<Language, List<string>>> dialogueDictionary = 
            new Dictionary<int, Dictionary<Language, List<string>>>();

        public Dictionary<Language, int> letterSizeDictionary = new Dictionary<Language, int>();

        float currentTotalTypingDelay;

        LocalizeManager localizeManager;
        public bool ableToPassNext;

        Text interactionText;
        string talkText;

        public bool isTimelineChatting;
        public string currentDialogueName;
        public int currentContentLine;

        void Start()
        {
            localizeManager = LocalizeManager.instance;

            backgroundRectTransform = chatBox.transform.GetChild(0).GetComponent<RectTransform>();
            nextButtonRectTransform = nextButton.transform.GetComponent<RectTransform>();

            typingDelay = new WaitForSeconds(typingCycle);
            blinkTime = new WaitForSeconds(blinkCycle);

            SetLetterSizeDict();

            SetDialougeNameList();
            GetDialoguesData();

            interactionText = interactionBox.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>();
            talkText = localizeManager.descriptionsDict["TalkDesc".GetHashCode()][localizeManager.CurrentLanguage];
            interactionText.text = talkText;

            EventManager.instance.AddListener(EventType.LanguageChange, OnLanguageChange);
        }

        void OnDestroy()
        {
            EventManager.instance.UnsubscribeEvent(EventType.LanguageChange, OnLanguageChange);
        }

        void OnLanguageChange()
        {
            HideChatBox();
            talkText = localizeManager.descriptionsDict["TalkDesc".GetHashCode()][localizeManager.CurrentLanguage];
            interactionText.text = talkText;
        }

        public void SetLetterSizeDict()
        {
            letterSizeDictionary.Add(Language.Korean, 10);
            letterSizeDictionary.Add(Language.English, 6);
        }


        #region get data from StreamingAsset
        //void GetDialoguesData()
        //{
        //    string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "StreamingAssets", "Dialogues"), "*.*");
        //    foreach (string sourceFile in files)
        //    {
        //        string fileName = Path.GetFileName(sourceFile);
        //        if (fileName.Substring(fileName.Length - 4) == ".csv")
        //        {
        //            string dialogueName = fileName.Substring(0, fileName.Length - 4);
        //            Dictionary<Language, List<string>> localDictionary = new Dictionary<Language, List<string>>();
        //            for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        //            {
        //                localDictionary.Add((Language)i, GetDialogue(dialogueName, i));
        //            }
        //            dialogueDictionary.Add(dialogueName.GetHashCode(), localDictionary);
        //        }               
        //    }
        //}
        //public List<string> GetDialogue(string csvFileName, int localNumber)
        //{
        //    List<string> dialogueList = new List<string>();
        //    string path = Path.Combine(Application.dataPath, "StreamingAssets", "Dialogues", csvFileName + ".csv");
        //    StreamReader streamReader = new StreamReader(path);
        //    bool endLine = false;

        //    streamReader.ReadLine();
        //    while (!endLine)
        //    {
        //        string csvData = streamReader.ReadLine();
        //        if (csvData == null)
        //        {
        //            endLine = true;
        //            break;
        //        }

        //        var row = csvData.Split(',');


        //        string context = row[localNumber];

        //        context = context.Replace("<br>", "\n");
        //        context = context.Replace("<c>", ",");
        //        context = context.Replace("<..>", "..");

        //        dialogueList.Add(context);
        //    }
        //    return dialogueList;
        //}
        #endregion

        #region get data from Resources
        void SetDialougeNameList()
        {
            dialogueNameList.Add("Prologue");
        }
        void GetDialoguesData()
        {
            for (int i = 0; i < dialogueNameList.Count; i++)
            {
                Dictionary<Language, List<string>> localDictionary = new Dictionary<Language, List<string>>();
                for (int j = 0; j < System.Enum.GetValues(typeof(Language)).Length; j++)
                {
                    localDictionary.Add((Language)j, GetDialogue(dialogueNameList[i], j));
                }
                dialogueDictionary.Add(dialogueNameList[i].GetHashCode(), localDictionary);
            }
        }

        public List<string> GetDialogue(string csvFilename, int localNumber)
        {
            List<string> dialogueList = new List<string>();
            TextAsset csvData = Resources.Load<TextAsset>(Path.Combine("Dialogues", csvFilename));

            string[] data = csvData.text.Split(new char[] { '\n' });

            for (int i = 1; i < data.Length - 1; i++)
            {
                string[] row = data[i].Split(new char[] { ',' });
                string content = row[localNumber];
                content = content.Replace("<br>", "\n");
                content = content.Replace("<c>", ",");
                content = content.Replace("<..>", "..");

                dialogueList.Add(content);
            }

            return dialogueList;
        }
        #endregion
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
            currentDialogueName = dialogueName;
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
            currentContentLine = contextCount;
            dialogueContext = dialogueDictionary[dialogueName.GetHashCode()]
                [localizeManager.CurrentLanguage][contextCount];
            StartCoroutine(TypeContexts(dialogueContext));
            SetChatBox(dialogueDictionary[dialogueName.GetHashCode()]
                [localizeManager.CurrentLanguage][contextCount]);
            Vector3 backgroundSize = GetBackgroundSize();
            chatBox.transform.position = new Vector3(chatBox.transform.position.x,
                transform.position.y + backgroundSize.y * 0.035f, transform.position.y);
            contextCount++;
        }

        public void ShowChatBox(string dialogueName, int contextLine)
        {
            currentDialogueName = dialogueName;
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
            currentContentLine = contextCount;
            dialogueContext = dialogueDictionary[dialogueName.GetHashCode()]
                [localizeManager.CurrentLanguage][contextLine];
            StartCoroutine(TypeContexts(dialogueContext));
            SetChatBox(dialogueDictionary[dialogueName.GetHashCode()]
                [localizeManager.CurrentLanguage][contextLine]);
            Vector3 backgroundSize = GetBackgroundSize();
            chatBox.transform.position = new Vector3(chatBox.transform.position.x,
                transform.position.y + backgroundSize.y * 0.035f, transform.position.y);
        }

        public void HideChatBox()
        {
            isTyping = false;
            isChatting = false;
            isTimelineChatting = false;
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

        //IEnumerator StartDialogue()
        //{
        //    yield return null;
        //    if (dialogueName != null)
        //    {
        //        isChatting = true;
        //        DialogueManager.instance.ShowChatBox(dialogueName);
        //        DialogueManager.instance.transform.position =
        //            TimelineManager.instance.playableDirectorInfoDict
        //            [TimelineManager.instance.currentPlayableDirector].talkerPos;
        //    }
        //}

    }
}
