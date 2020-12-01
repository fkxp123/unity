using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MomodoraCopy
{
    public class MenuManager : Singleton<MenuManager>
    {
        public GameObject mainMenuObject;
        MainMenu mainMenu;
        public MenuMemento menuMemento;

        public bool isDisableByEscapeKey = true;

        bool _isGamePaused;
        public bool IsGamePaused
        {
            get { return _isGamePaused; }
            set
            {
                _isGamePaused = value;
                if (value)
                {
                    GameManager.instance.Pause();
                    mainMenu.enabled = true;
                    return;
                }
                GameManager.instance.Resume();
            }
        }

        void Start()
        {
            mainMenu = mainMenuObject.GetComponent<MainMenu>();
            menuMemento = new MenuMemento();
        }
    }

}