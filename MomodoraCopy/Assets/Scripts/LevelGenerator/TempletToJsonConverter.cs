using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;

namespace MomodoraCopy
{
    public class TempletInfo
    {
        #region Tiles
        public List<Vector3Int> platformTileList;
        public List<Vector3Int> ladderTileList;
        public List<Vector3Int> spikeTileList;
        #endregion

        #region Objects
        public List<Vector3Int> pushBlockList;
        #endregion
    }

    public class TempletToJsonConverter : MonoBehaviour
    {
        public bool GetNewJsonFile;

        public TileBase platform;
        public TileBase ladder;
        public TileBase spike;

        public List<TempletInfo> templetList = new List<TempletInfo>();

        //[ContextMenu("Convert Templets To Json")]
        //void ConvertTempletsToJson()
        //{
        //    string superDirectoryPath = Path.Combine(Application.dataPath, "Templets");
        //    if (!Directory.Exists(superDirectoryPath))
        //    {
        //        Directory.CreateDirectory(superDirectoryPath);
        //    }

        //    for (int i = 0; i < transform.childCount; i++)
        //    {
        //        TempletInfo templet = new TempletInfo();
        //        Tilemap tilemap = transform.GetChild(i).GetComponent<Tilemap>();
        //        templetList.Add(templet);
        //        string roomTypeStr = tilemap.name.Substring(0, 4);

        //        List<Vector3Int> platformTileList = new List<Vector3Int>();
        //        List<Vector3Int> ladderTileList = new List<Vector3Int>();
        //        List<Vector3Int> spikeTileList = new List<Vector3Int>();

        //        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        //        {
        //            if (!tilemap.HasTile(position))
        //            {
        //                continue;
        //            }
        //            Vector3Int cellPos = tilemap.WorldToCell(position) - tilemap.cellBounds.min;
        //            //Debug.Log(tilemap.GetTile(position).name);
        //            if (tilemap.GetTile(position).name.Equals(platform.name))
        //            {
        //                platformTileList.Add(cellPos);
        //            }
        //            else if (tilemap.GetTile(position).name.Equals(ladder.name))
        //            {
        //                ladderTileList.Add(cellPos);
        //            }
        //            else if (tilemap.GetTile(position).name.Equals(spike.name))
        //            {
        //                spikeTileList.Add(cellPos);
        //            }
        //        }
        //        templetList[i].platformTileList = platformTileList;
        //        templetList[i].ladderTileList = ladderTileList;
        //        templetList[i].spikeTileList = spikeTileList;

        //        string jsonData = JsonUtility.ToJson(templet, true);
        //        string directoryPath = Path.Combine(superDirectoryPath, tilemap.name.Substring(0, tilemap.name.Length - 1));
        //        Directory.CreateDirectory(directoryPath);
        //        string path = Path.Combine(directoryPath, string.Format("{0}.Json", tilemap.name));
        //        File.WriteAllText(path, jsonData);
        //    }
        //}

        void Awake()
        {
            if (!GetNewJsonFile)
            {
                return;
            }
            string superDirectoryPath = Path.Combine(Application.dataPath, "RoomTemplets");
            if (!Directory.Exists(superDirectoryPath))
            {
                Directory.CreateDirectory(superDirectoryPath);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                TempletInfo templet = new TempletInfo();
                Tilemap tilemap = transform.GetChild(i).GetComponent<Tilemap>();
                templetList.Add(templet);
                string roomTypeStr = tilemap.name.Substring(0, 4);

                List<Vector3Int> platformTileList = new List<Vector3Int>();
                List<Vector3Int> ladderTileList = new List<Vector3Int>();
                List<Vector3Int> spikeTileList = new List<Vector3Int>();

                List<Vector3Int> pushBlockList = new List<Vector3Int>();
                for(int j = 0; j < tilemap.transform.childCount; j++)
                {
                    if(tilemap.transform.GetChild(j).name == "PushBlock")
                    {
                        pushBlockList.Add(Vector3Int.FloorToInt(
                            tilemap.transform.GetChild(j).transform.position)
                            - tilemap.cellBounds.min);
                    }
                }

                foreach (var position in tilemap.cellBounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(position))
                    {
                        continue;
                    }
                    Vector3Int cellPos = tilemap.WorldToCell(position) - tilemap.cellBounds.min;

                    if (tilemap.GetTile(position).name.Equals(platform.name))
                    {
                        platformTileList.Add(cellPos);
                    }
                    else if (tilemap.GetTile(position).name.Equals(ladder.name))
                    {
                        ladderTileList.Add(cellPos);
                    }
                    else if (tilemap.GetTile(position).name.Equals(spike.name))
                    {
                        spikeTileList.Add(cellPos);
                    }
                }
                templetList[i].platformTileList = platformTileList;
                templetList[i].ladderTileList = ladderTileList;
                templetList[i].spikeTileList = spikeTileList;

                templetList[i].pushBlockList = pushBlockList;

                string jsonData = JsonUtility.ToJson(templet, true);
                string directoryPath = Path.Combine(superDirectoryPath, tilemap.name.Substring(0, tilemap.name.Length - 1));
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string path = Path.Combine(directoryPath, string.Format("{0}.Json", tilemap.name));
                File.WriteAllText(path, jsonData);
            }
        }
    }
}