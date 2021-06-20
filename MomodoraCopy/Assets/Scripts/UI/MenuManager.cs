using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace MomodoraCopy
{
    public class MenuManager : Singleton<MenuManager>
    {
        public GameObject mainMenuObject;
        MainMenu mainMenu;
        public MenuMemento menuMemento;

        Canvas[] canvases;

        public bool isDisableByEscapeKey = true;

        bool isGamePaused;

        void Start()
        {
            canvases = GetComponentsInChildren<Canvas>();
            mainMenu = mainMenuObject.GetComponent<MainMenu>();
            menuMemento = new MenuMemento();

            EventManager.instance.AddListener(EventType.GamePause, OnGamePause);
            EventManager.instance.AddListener(EventType.GameResume, OnGameResume);
        }

        void OnGamePause()
        {
            isGamePaused = true;
            mainMenu.enabled = true;
        }
        void OnGameResume()
        {
            isGamePaused = false;
            mainMenu.enabled = false;
        }

        void Update()
        {
            if (!isGamePaused && Input.GetKeyDown(KeyboardManager.instance.MenuKey))
            {
                EventManager.instance.PostNotification(EventType.GamePause);
            }
            else if (isGamePaused && Input.GetKeyDown(KeyboardManager.instance.MenuKey))
            {
                EventManager.instance.PostNotification(EventType.GameResume);
            }
        }
    }

}