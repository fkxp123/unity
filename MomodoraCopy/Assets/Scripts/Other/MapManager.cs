using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class MapManager : Singleton<MapManager>
    {
        Tilemap tilemap;
        public Tilemap Tilemap
        {
            get
            {
                return tilemap;
            }
            set
            {
                tilemap = value;
                CameraManager.instance.SetCameraBounds(tilemap);
            }
        }

        public void SetMainTilemap(Tilemap mainTilemap)
        {
            tilemap = mainTilemap;
        }
    }

}