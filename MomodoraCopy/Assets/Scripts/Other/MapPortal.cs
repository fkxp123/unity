using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class MapPortal : MonoBehaviour
    {
        public GameObject enterPoint;
        public GameObject exitPoint;
        GameObject tempPoint;

        public GameObject enterCheckPoint;
        public GameObject exitCheckPoint;
        GameObject tempCheckPoint;

        public Tilemap enterTilemap;
        public Tilemap exitTilemap;
        Tilemap tempTilemap;

        [SerializeField]
        float checkPlayerCycle;
        WaitForSeconds waitTime;

        public Vector3 enterCheckPlayerArea;
        public Vector3 exitCheckPlayerArea;

        bool isSwitched;

        void Start()
        {
            checkPlayerCycle = 0.1f;
            waitTime = new WaitForSeconds(checkPlayerCycle);
            StartCoroutine(CheckPlayer());
        }
        IEnumerator CheckPlayer()
        {
            while (true)
            {
                if (isSwitched)
                {
                    Collider2D[] collider2Ds = Physics2D.OverlapBoxAll
                        (enterCheckPoint.transform.position, enterCheckPlayerArea, 0);
                    foreach (Collider2D collider in collider2Ds)
                    {
                        if (collider.tag == "Player")
                        {
                            GameManager.instance.playerPhysics.transform.position = enterPoint.transform.position;
                            //MapManager.instance.CurrentTilemap = enterTilemap;
                            MapManager.instance.SetCurrentTilemap(enterTilemap);
                            isSwitched = isSwitched == true ? false : true;
                        }
                    }
                    yield return waitTime;
                }
                else
                {
                    Collider2D[] collider2Ds = Physics2D.OverlapBoxAll
                        (exitCheckPoint.transform.position, exitCheckPlayerArea, 0);
                    foreach (Collider2D collider in collider2Ds)
                    {
                        if (collider.tag == "Player")
                        {
                            GameManager.instance.playerPhysics.transform.position = exitPoint.transform.position;
                            //MapManager.instance.CurrentTilemap = exitTilemap;
                            MapManager.instance.SetCurrentTilemap(exitTilemap);
                            isSwitched = isSwitched == true ? false : true;
                        }
                    }
                    yield return waitTime;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(enterCheckPoint.transform.position, enterCheckPlayerArea);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(exitCheckPoint.transform.position, exitCheckPlayerArea);
        }
    } 
}
