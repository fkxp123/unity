using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

namespace MomodoraCopy
{
    public class MapInitializer : MonoBehaviour
    {
        public Tilemap mainTilemap;
        public GameObject[] nearMap;

        void Awake()
        {
            if(mainTilemap == null)
            {
                mainTilemap = transform.GetChild(0).transform.GetChild(0).GetComponent<Tilemap>();
            }

            MapManager.instance.AddMapLinkData(mainTilemap, nearMap);
        }
        void Start()
        {
            gameObject.SetActive(false);
        }
    }
}