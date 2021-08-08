using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MomodoraCopy;

namespace InventorySystem
{
    public enum ItemType
    {
        ActiveItem,
        PassiveItem,
        KeyItem,
    }

    public interface IUsable
    {
        void UseItem();
    }
    public interface IPassive
    {
        void ActivePassive();
        void InactivePassive();
    }

    [System.Serializable]
    public class Item
    {
        public string itemIDName;
        public ItemType itemType;
        public int itemCount;
        public Sprite itemIcon;

        public Item(string itemIDName, ItemType itemType)
        {
            this.itemIDName = itemIDName;
            this.itemType = itemType;
        }
    }

    public class Bellflower : Item, IUsable
    {
        public Bellflower(string itemIDName, ItemType itemType) : base(itemIDName, itemType)
        {
            this.itemIDName = itemIDName;
            this.itemType = itemType;
        }

        public void UseItem()
        {
            GameManager.instance.playerStatus.Hp += 20;
        }
    }

    public class TaintedMissive : Item, IUsable
    {
        public TaintedMissive(string itemIDName, ItemType itemType) : base(itemIDName, itemType)
        {
            this.itemIDName = itemIDName;
            this.itemType = itemType;
        }

        public void UseItem()
        {
            GameManager.instance.playerStatus.meleeAtk *= 2;
        }
    }

    public class Passiflora : Item, IUsable
    {
        public Passiflora(string itemIDName, ItemType itemType) : base(itemIDName, itemType)
        {
            this.itemIDName = itemIDName;
            this.itemType = itemType;
        }

        public void UseItem()
        {
            GameManager.instance.playerStatus.Hp = GameManager.instance.playerStatus.maxHp;
        }
    }

    public class MagnetStone : Item, IPassive
    {
        public MagnetStone(string itemIDName, ItemType itemType) : base(itemIDName, itemType)
        {
            this.itemIDName = itemIDName;
            this.itemType = itemType;
        }

        public void ActivePassive()
        {

        }
        public void InactivePassive()
        {

        }
    }

    public class EdeasPearl : Item, IPassive
    {
        public EdeasPearl(string itemIDName, ItemType itemType) : base(itemIDName, itemType)
        {
            this.itemIDName = itemIDName;
            this.itemType = itemType;
        }

        public void ActivePassive()
        {

        }
        public void InactivePassive()
        {

        }
    }


}