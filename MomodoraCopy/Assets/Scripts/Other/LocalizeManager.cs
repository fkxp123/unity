using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MomodoraCopy
{
    public enum Language
    {
        Korean, English
    }

    public class LocalizeManager : Singleton<LocalizeManager>
    {
        [SerializeField]
        Language currentLanguage;
        public Language CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                currentLanguage = value;
                EventManager.instance.PostNotification(EventType.LanguageChanged);
            }
        }

        public Dictionary<int, Dictionary<Language, string>> descriptionsDict =
                new Dictionary<int, Dictionary<Language, string>>();

        public override void Awake()
        {
            base.Awake();
            currentLanguage = Language.Korean;
            GetDescriptionData();
        }

        void Start()
        {
        }

        public void GetDescriptionData()
        {
            string path = Path.Combine(Application.dataPath, "Localize", "Descriptions.csv");
            StreamReader streamReader = new StreamReader(path);
            bool endLine = false;
            while (!endLine)
            {
                string csvData = streamReader.ReadLine();
                if(csvData == null)
                {
                    endLine = true;
                    break;
                }

                var data = csvData.Split(',');
                Dictionary<Language, string> contentDict = new Dictionary<Language, string>();
                for(int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
                {
                    contentDict.Add((Language)i, data[i + 1]);
                }
                descriptionsDict.Add(data[0].GetHashCode(), contentDict);
            }
        }
    }

}