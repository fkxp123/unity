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
        //void OnEnable()
        //{
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}
        //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    StartCoroutine(ResetRenderCameras());
        //}
        //IEnumerator ResetRenderCameras()
        //{
        //    yield return null;
        //    Debug.Log(canvases[0].worldCamera);
        //    for (int i = 0; i < canvases.Length; i++)
        //    {
        //        if (canvases[i].worldCamera == null)
        //        {
        //            canvases[i].worldCamera = GameManager.instance.mainCameraObject.GetComponent<Camera>();
        //        }
        //    }
        //}
        //void OnDisable()
        //{
        //    SceneManager.sceneLoaded -= OnSceneLoaded;
        //}
        void Start()
        {
            canvases = GetComponentsInChildren<Canvas>();
            mainMenu = mainMenuObject.GetComponent<MainMenu>();
            menuMemento = new MenuMemento();
        }
    }

}