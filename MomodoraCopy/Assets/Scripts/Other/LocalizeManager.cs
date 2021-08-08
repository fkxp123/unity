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
                EventManager.instance.PostNotification(EventType.LanguageChange);
            }
        }

        public Dictionary<int, Dictionary<Language, string>> descriptionsDict =
                new Dictionary<int, Dictionary<Language, string>>();

        public Dictionary<int, Dictionary<Language, string>> guidesDict =
                new Dictionary<int, Dictionary<Language, string>>();

        public override void Awake()
        {
            base.Awake();
            currentLanguage = Language.Korean;
            GetDescriptionData();
            GetGuideData();
        }

        #region get data from Resources
        public void GetDescriptionData()
        {
            TextAsset csvData = Resources.Load<TextAsset>(Path.Combine("Localize", "Descriptions"));

            string[] data = csvData.text.Split(new char[] { '\n' });
            for (int i = 1; i < data.Length - 1; i++)
            {
                var content = data[i].Split(',');
                for(int j = 0; j < content.Length; j++)
                {
                    content[j] = content[j].Replace("<br>", "\n");
                    content[j] = content[j].Replace("<c>", ",");
                    content[j] = content[j].Replace("<..>", "..");
                }
                Dictionary<Language, string> contentDict = new Dictionary<Language, string>();
                for (int j = 0; j < System.Enum.GetValues(typeof(Language)).Length; j++)
                {
                    contentDict.Add((Language)j, content[j + 1]);
                }
                descriptionsDict.Add(content[0].GetHashCode(), contentDict);

            }
        }
        public void GetGuideData()
        {
            TextAsset csvData = Resources.Load<TextAsset>(Path.Combine("Localize", "Guides"));

            string[] data = csvData.text.Split(new char[] { '\n' });
            for (int i = 1; i < data.Length - 1; i++)
            {
                var content = data[i].Split(',');
                for (int j = 0; j < content.Length; j++)
                {
                    content[j] = content[j].Replace("<br>", "\n");
                    content[j] = content[j].Replace("<c>", ",");
                    content[j] = content[j].Replace("<..>", "..");
                }
                Dictionary<Language, string> contentDict = new Dictionary<Language, string>();
                for (int j = 0; j < System.Enum.GetValues(typeof(Language)).Length; j++)
                {
                    contentDict.Add((Language)j, content[j + 1]);
                }
                guidesDict.Add(content[0].GetHashCode(), contentDict);

            }
        }
        #endregion

        #region get data from StreamingAsset
        //public void GetDescriptionData()
        //{
        //    string path = Path.Combine(Application.dataPath, "StreamingAssets", "Localize", "Descriptions.csv");
        //    StreamReader streamReader = new StreamReader(path);
        //    bool endLine = false;
        //    while (!endLine)
        //    {
        //        string csvData = streamReader.ReadLine();
        //        if(csvData == null)
        //        {
        //            endLine = true;
        //            break;
        //        }

        //        var data = csvData.Split(',');
        //        Dictionary<Language, string> contentDict = new Dictionary<Language, string>();
        //        for(int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        //        {
        //            contentDict.Add((Language)i, data[i + 1]);
        //        }
        //        descriptionsDict.Add(data[0].GetHashCode(), contentDict);
        //    }
        //}

        //public void GetGuideData()
        //{
        //    string path = Path.Combine(Application.dataPath, "StreamingAssets", "Localize", "Guides.csv");
        //    StreamReader streamReader = new StreamReader(path);
        //    bool endLine = false;
        //    while (!endLine)
        //    {
        //        string csvData = streamReader.ReadLine();
        //        if (csvData == null)
        //        {
        //            endLine = true;
        //            break;
        //        }

        //        var data = csvData.Split(',');
        //        Dictionary<Language, string> contentDict = new Dictionary<Language, string>();
        //        for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        //        {
        //            contentDict.Add((Language)i, data[i + 1]);
        //        }
        //        guidesDict.Add(data[0].GetHashCode(), contentDict);
        //    }
        //}
        #endregion
    }

}