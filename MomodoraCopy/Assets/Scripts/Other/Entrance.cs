using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MomodoraCopy
{
    public class Entrance : MonoBehaviour
    {
        public bool isStartingLeft;
        public GameObject startPoint;

        void Start()
        {
            GameManager.instance.DisablePlayer();
            //GameManager.instance.transform.position =
            //    new Vector3(startPoint.transform.position.x, startPoint.transform.position.y + 0.015f, 0);
            //GameManager.instance.playerPhysics.transform.position =
            //    new Vector3(startPoint.transform.position.x, startPoint.transform.position.y + 0.015f, 0);
            //GameManager.instance.EnablePlayer();
            StartCoroutine(TranslatePlayer());


            //gamemanager.instance.load() : playermovement -> levelgenerator ?gamemanager

        }

        IEnumerator TranslatePlayer()
        {
            yield return null;
            GameManager.instance.transform.position =
                new Vector3(startPoint.transform.position.x, startPoint.transform.position.y + 0.015f, 0);
            GameManager.instance.playerPhysics.transform.position =
                new Vector3(startPoint.transform.position.x, startPoint.transform.position.y + 0.015f, 0);
            GameManager.instance.EnablePlayer();
        }
    }

}