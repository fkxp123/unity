using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject playerObject;
        public GameObject mainCameraObject;
        MonoBehaviour[] cameraComponents;
        MonoBehaviour[] playerComponents;
        public void Start()
        {
            playerComponents = playerObject.GetComponents<MonoBehaviour>();
            cameraComponents = mainCameraObject.GetComponents<MonoBehaviour>();
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            foreach (MonoBehaviour component in playerComponents)
            {
                component.enabled = true;
            }
            foreach (MonoBehaviour component in cameraComponents)
            {
                component.enabled = true;
            }
        }
        public void Pause()
        {
            Time.timeScale = 0f;
            foreach (MonoBehaviour component in playerComponents)
            {
                component.enabled = false;
            }
            foreach (MonoBehaviour component in cameraComponents)
            {
                component.enabled = false;
            }
        }

    }

}
