using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MomodoraCopy;
using System.IO;

namespace InventorySystem
{
    public class ItemDatabase : Singleton<ItemDatabase>
    {
        public Dictionary<int, Item> itemIDDict = new Dictionary<int, Item>();
        public Dictionary<int, Dictionary<Language, string>> itemContentDict 
            = new Dictionary<int, Dictionary<Language, string>>();

        public override void Awake()
        {
            base.Awake();
            InitItemData();
            GetItemData();
        }
        void InitItemData()
        {
            Bellflower bellFlower = new Bellflower("Bellflower", ItemType.ActiveItem)
            {
                itemCount = 3,
                itemIcon = GetItemSprite("BellFlower"),
            };
            TaintedMissive taintedMissive = new TaintedMissive("TaintedMissive", ItemType.ActiveItem)
            {
                itemCount = 3,
                itemIcon = GetItemSprite("TaintedMissive"),
            };
            Passiflora passiFlora = new Passiflora("Passiflora", ItemType.ActiveItem)
            {
                itemCount = 1,
                itemIcon = GetItemSprite("PassiFlora"),
            };
            EdeasPearl edeasPearl = new EdeasPearl("Edea'sPearl", ItemType.PassiveItem)
            {
                itemIcon = GetItemSprite("Edea'sPearl"),
            };
            MagnetStone magnetStone = new MagnetStone("MagnetStone", ItemType.PassiveItem)
            {
                itemIcon = GetItemSprite("MagnetStone"),
            };

            itemIDDict.Add(bellFlower.itemIDName.GetHashCode(), bellFlower);
            itemIDDict.Add(taintedMissive.itemIDName.GetHashCode(), taintedMissive);
            itemIDDict.Add(passiFlora.itemIDName.GetHashCode(), passiFlora);
            itemIDDict.Add(edeasPearl.itemIDName.GetHashCode(), edeasPearl);
            itemIDDict.Add(magnetStone.itemIDName.GetHashCode(), magnetStone);
        }
        Sprite GetItemSprite(string itemName)
        {
            return Resources.Load<Sprite>(Path.Combine("Items", "ItemSprite", itemName));
        }
        void GetItemData()
        {
            TextAsset csvData = Resources.Load<TextAsset>(Path.Combine("Items", "ItemData"));

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
                itemContentDict.Add(content[0].GetHashCode(), contentDict);
            }
        }


        //public List<string> GetDialogue(string _CSVFileName, int localNumber)
        //{
        //    List<string> dialogueList = new List<string>();
        //    TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        //    string[] data = csvData.text.Split(new char[] { '\n' });

        //    for (int i = 1; i < data.Length - 1; i++)
        //    {
        //        string[] row = data[i].Split(new char[] { ',' });
        //        string context = row[localNumber];
        //        context = context.Replace("<br>", "\n");
        //        context = context.Replace("<c>", ",");
        //        context = context.Replace("<..>", "..");

        //        dialogueList.Add(context);
        //    }

        //    return dialogueList;
        //}
    }

}