using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class HorizontalBoundChecker : MonoBehaviour
    {
        public Tilemap enterTilemap;
        public Tilemap exitTilemap;
        Tilemap tempTilemap;

        [SerializeField]
        float passedDirection = 0;
        [SerializeField]
        public bool isLeftToRight;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                if (isLeftToRight)
                {
                    passedDirection = other.transform.position.x < transform.position.x ? 1 : -1;
                }
                else
                {
                    passedDirection = other.transform.position.x < transform.position.x ? -1 : 1;
                }
                if (passedDirection < 0)
                {
                    MapManager.instance.SetCurrentTilemap(enterTilemap);
                    EventManager.instance.PostNotification(EventType.MapChange);
                }
                else if (passedDirection > 0)
                {
                    MapManager.instance.SetCurrentTilemap(exitTilemap);
                    EventManager.instance.PostNotification(EventType.MapChange);
                }
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                if (isLeftToRight)
                {
                    if (other.transform.position.x > transform.position.x && passedDirection < 0)
                    {
                        MapManager.instance.SetCurrentTilemap(exitTilemap);
                        EventManager.instance.PostNotification(EventType.MapChange);
                    }
                    else if (other.transform.position.x < transform.position.x && passedDirection > 0)
                    {
                        MapManager.instance.SetCurrentTilemap(enterTilemap);
                        EventManager.instance.PostNotification(EventType.MapChange);
                    }
                }
            }
        }
        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireCube(checkPoint.transform.position, checkPlayerArea);
        //}
    }
}
