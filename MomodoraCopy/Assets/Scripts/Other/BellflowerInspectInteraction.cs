using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

namespace MomodoraCopy
{
    public class BellflowerInspectInteraction : MonoBehaviour, IInpsectInteraction
    {
        GameObject bellflower;
        GameObject cutedBellflower;

        public ParticleSystem littleFlashEffect;

        Item item;

        void Start()
        {
            bellflower = transform.GetChild(0).gameObject;
            cutedBellflower = transform.GetChild(1).gameObject;

            bellflower.SetActive(true);
            cutedBellflower.SetActive(false);

            item = new Item("Bellflower", ItemType.ActiveItem);
        }

        public void InspectInteract()
        {
            littleFlashEffect.Stop();
            GameManager.instance.ShowGotItemMessageBox(item);
            bellflower.SetActive(false);
            cutedBellflower.SetActive(true);
            GameManager.instance.GetItem(item);
        }
    }

}