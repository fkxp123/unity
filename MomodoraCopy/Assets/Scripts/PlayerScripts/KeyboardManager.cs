using UnityEngine;
using System.Collections.Generic;

namespace MomodoraCopy
{
    public class KeyboardManager : Singleton<KeyboardManager>
    {
        public KeyCode UpKey { get; set; }
        public KeyCode DownKey { get; set; }
        public KeyCode LeftKey { get; set; }
        public KeyCode RightKey { get; set; }
        public KeyCode JumpKey { get; set; }
        public KeyCode AttackKey { get; set; }
        public KeyCode BowAttackKey { get; set; }
        public KeyCode RollKey { get; set; }
        public KeyCode UseItemKey { get; set; }
        public KeyCode ChangeItemKey { get; set; }
        public KeyCode MappingKey { get; set; }
        public KeyCode MenuKey { get; set; }
        public KeyCode ConfirmKey { get; set; }
        public KeyCode CancleKey { get; set; }

        public List<KeyCode> keyCodeList = new List<KeyCode>();
        public List<string> keyNameList = new List<string>();

        public override void Awake()
        {
            base.Awake();

            keyNameList.Add("Up");
            keyNameList.Add("Down");
            keyNameList.Add("Left");
            keyNameList.Add("Right");
            keyNameList.Add("Jump");
            keyNameList.Add("Attack");
            keyNameList.Add("BowAttack");
            keyNameList.Add("Roll");
            keyNameList.Add("UseItem");
            keyNameList.Add("ChangeItem");
            keyNameList.Add("Mapping");
            keyNameList.Add("Menu");

            UpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[0], "UpArrow"));
            DownKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[1], "DownArrow"));
            LeftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[2], "LeftArrow"));
            RightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[3], "RightArrow"));
            JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[4], "A"));
            AttackKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[5], "S"));
            BowAttackKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[6], "D"));
            RollKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[7], "Q"));
            UseItemKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[8], "W"));
            ChangeItemKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[9], "E"));
            MappingKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[10], "Tab"));
            MenuKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(keyNameList[11], "Escape"));

            keyCodeList.Add(UpKey);
            keyCodeList.Add(DownKey);
            keyCodeList.Add(LeftKey);
            keyCodeList.Add(RightKey);
            keyCodeList.Add(JumpKey);
            keyCodeList.Add(AttackKey);
            keyCodeList.Add(BowAttackKey);
            keyCodeList.Add(RollKey);
            keyCodeList.Add(UseItemKey);
            keyCodeList.Add(ChangeItemKey);
            keyCodeList.Add(MappingKey);
            keyCodeList.Add(MenuKey);
        }
    }

}