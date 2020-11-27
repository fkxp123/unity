using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public abstract class GUI : IMenu
    {
        public GameObject MenuBackground;
        public GameObject KeyDescriptionBar;
        MonoBehaviour[] Components { get; set; }
        Image menuBackgroundImage { get; set; }

        public MenuManager menuManager = MenuManager.instance;

        public virtual void Start()
        {
            //Components = GetComponents<MonoBehaviour>();
            //menuBackgroundImage = MenuBackground.GetComponent<Image>();
        }
        public virtual void UpdateMenu()
        {
            CheckArrowKey();
            CheckConfirmKey();
            CheckCancleKey();
            CheckEscapeKey();
        }

        public virtual void OnStartMenu()
        {
            MenuBackground.SetActive(true);
            KeyDescriptionBar.SetActive(true);
        }

        public virtual void CheckEscapeKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //menuManager.Resume();
                ExitMenu();
            }
        }
        public virtual void CheckArrowKey()
        {

        }
        public virtual void CheckConfirmKey()
        {

        }
        public virtual void CheckCancleKey()
        {

        }

        public void SetBackgroundAlpha(float alpha)
        {
            Color temp = menuBackgroundImage.color;
            temp.a = alpha;
            menuBackgroundImage.color = temp;
        }
        public void SetKeyDescriptionBarPosition(Vector3 position)
        {
            //menuManager.keyDescriptionBar.GetComponent<RectTransform>().localPosition = position;
        }
        public void ExitMenu()
        {
            foreach(MonoBehaviour component in Components)
            {
                component.enabled = false;
            }
        }
    }
}