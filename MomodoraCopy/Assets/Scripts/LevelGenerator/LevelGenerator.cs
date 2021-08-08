using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

namespace MomodoraCopy
{
    //public class Room
    //{
    //    public int roomNumber;
    //    public int roomWidth;
    //    public int roomHeight;

    //    public List<Vector3Int> cellList = new List<Vector3Int>();

    //    public bool isOpenLeft;
    //    public bool isOpenRight;
    //    public bool isOpenBottom;
    //    public bool isOpenTop;
    //}

    public enum RoomType
    {
        Void, LR, LRB, LRBT, LRT, L, LB, LBT, LT, R, RB, RBT, RT, BT, T, B
    };
    public enum TileType
    {
        Platform, Ladder, LadderPlatform, ObstacleGround, ObstacleAir, Spike, Stairs
    };
    public enum ObjectType
    {
        Entrance, Exit, PushBlock, MovingTrap, ElevatorTrap
    };
    //SetPushObject, SetEntranceObject -> SetObject(Vector3Int pos, int roomnumber, ObjectType obj)

    public class LevelGenerator : MonoBehaviour
    {
        public Tilemap outFrame;
        public Tilemap platformTileMap;
        public Tilemap ladderTileMap;
        public Tilemap spikeTileMap;

        public RuleTile platformTile;
        public RuleTile ladderTile;
        public Tile spikeTile;
        public Tile stairsTile;

        public GameObject pushBlock;
        public GameObject movingTrap;
        public GameObject entrance;
        public GameObject exit;

        private Vector3Int previous;

        int roomWidth;
        int roomHeight;
        int grid;

        enum PathDirection
        {
            None, Left, Right, Down
        };

        List<Vector3Int> roomGridPositionList = new List<Vector3Int>();
        Dictionary<int, Vector3Int> cellDicitonary = new Dictionary<int, Vector3Int>();
        Dictionary<int, RoomType> roomDictionary = new Dictionary<int, RoomType>();
        List<int> excludedPathList = new List<int>();
        List<int> pathList = new List<int>();

        GameObject enterObject;
        GameObject exitObject;

        public PoolingObjectInfo pushBlockInfo;
        Dictionary<int, List<GameObject>> pushBlockObjectDict = new Dictionary<int, List<GameObject>>();

        TileType platform;
        TileType ladder;
        TileType spike;

        string superDirectoryPath;

        Dictionary<int, List<Vector3Int>> openPlaceableDict = new Dictionary<int, List<Vector3Int>>();
        Dictionary<int, List<Vector3Int>> closedPlaceableDict = new Dictionary<int, List<Vector3Int>>();

        void Start()
        {
            System.DateTime startTime = System.DateTime.Now;
            Debug.Log("start :" + startTime);
            grid = 4;
            roomWidth = 40;
            roomHeight = 32;
            enterObject = Instantiate(entrance);
            exitObject = Instantiate(exit);

            platform = TileType.Platform;
            ladder = TileType.Ladder;
            spike = TileType.Spike;

            superDirectoryPath = Path.Combine(Application.dataPath, "RoomTemplets");

            Vector3Int cell = platformTileMap.WorldToCell(Vector3.zero);
            DrawOutFrame(4, cell, roomWidth * grid + 1, roomHeight * grid + 1);
            SetGridList();
            SetCellDict();
            SetExcludedPathList();
            SetOpenPlaceableDict();
            SetPushBlockObjectDict();
            
            InitLevelGenerator();
            System.DateTime afterInit = System.DateTime.Now;
            System.TimeSpan afterInitTime = afterInit - startTime;
            Debug.Log("afterInit :" + afterInitTime);

            SetSolutionPath();
            System.DateTime afterSolutionPath = System.DateTime.Now;
            System.TimeSpan afterSPTime = afterSolutionPath - startTime;
            Debug.Log("afterSP :" + afterSPTime);

            SetExcludedPath();
            System.DateTime afterExcludedPath = System.DateTime.Now;
            System.TimeSpan afterEPTime = afterExcludedPath - startTime;
            Debug.Log("afterEP :" + afterEPTime);

            DrawAllRooms();
            System.DateTime afterDrawRooms = System.DateTime.Now;
            System.TimeSpan afterDRTime = afterDrawRooms - startTime;
            Debug.Log("afterDR :" + afterDRTime);
            System.DateTime endTime = System.DateTime.Now;
            Debug.Log("end :" + endTime);
            System.TimeSpan totalTime = endTime - startTime;
            Debug.Log("total generate time :" + totalTime);

            StartCoroutine(ActivateObjects());

            //MapManager.instance.CurrentTilemap = platformTileMap;
            MapManager.instance.SetCurrentTilemap(platformTileMap);
        }

        void InitLevelGenerator()
        {
            platformTileMap.gameObject.SetActive(true);
            ladderTileMap.gameObject.SetActive(true);
            spikeTileMap.gameObject.SetActive(true);

            pushBlockInfo = new PoolingObjectInfo
            {
                prefab = pushBlock,
                spawner = gameObject,
                position = transform.position,
                objectRotation = transform.rotation
            };
            //ObjectPooler.instance.CreatePoolingObjectQueue(pushBlockInfo, roomHeight * roomWidth * grid * grid / 4);
            ObjectPooler.instance.CreatePoolingObjects(pushBlockInfo, 100);
        }
        IEnumerator ActivateObjects()
        {
            yield return null;
            for (int i = 0; i < pushBlockObjectDict.Count; i++)
            {
                for (int j = 0; j < pushBlockObjectDict[i].Count; j++)
                {
                    pushBlockObjectDict[i][j].SetActive(true);
                }
            }
        }

        void SetExcludedPathList()
        {
            for (int i = 0; i < grid * grid; i++)
            {
                excludedPathList.Add(i);
            }
        }
        void SetCellDict()
        {
            for (int i = 0; i < roomHeight; i++)
            {
                for (int j = 0; j < roomWidth; j++)
                {
                    cellDicitonary.Add(i * roomWidth + j, new Vector3Int(j, i, 0));
                }
            }
        }
        void DrawOutFrame(int lineNumber, Vector3Int origin, int width, int height)
        {
            Vector3Int cell = origin;
            for(int k = 0; k < lineNumber; k++)
            {
                cell.x -= 1;
                cell.y -= 1;
                for (int i = 0; i < width + k * 2; i++)
                {
                    platformTileMap.SetTile(cell, platformTile);
                    cell.x += 1;
                }
                for (int i = 0; i < height + k * 2; i++)
                {
                    platformTileMap.SetTile(cell, platformTile);
                    cell.y += 1;
                }
                for (int i = 0; i < width + k * 2; i++)
                {
                    platformTileMap.SetTile(cell, platformTile);
                    cell.x -= 1;
                }
                for (int i = 0; i < height + k * 2; i++)
                {
                    platformTileMap.SetTile(cell, platformTile);
                    cell.y -= 1;
                }
            }
        }
        void SetGridList()
        {
            Vector3Int cell = platformTileMap.WorldToCell(Vector3.zero);
            for (int i = grid - 1; i >= 0; i--)
            {
                cell.y = i;
                for (int j = 0; j < grid; j++)
                {
                    cell.x = j;
                    roomGridPositionList.Add(cell);
                }
            }
            //foreach (Vector3Int v in roomGridPositionList)
            //{
            //    Debug.Log(v);
            //}
        }
        void SetOpenPlaceableDict()
        {
            for (int i = 0; i < grid * grid; i++)
            {
                List<Vector3Int> cell = new List<Vector3Int>();
                for (int j = 0; j < cellDicitonary.Count; j++)
                {
                    cell.Add(cellDicitonary[j]);
                }
                openPlaceableDict.Add(i, cell);
            }
        }
        void SetPushBlockObjectDict()
        {
            for(int i = 0; i < grid * grid; i++)
            {
                pushBlockObjectDict.Add(i, new List<GameObject>());
            }
        }

        #region Draw & Erase Tile Functions
        void DrawTile(Vector3Int globalPosition, TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Platform:
                    platformTileMap.SetTile(globalPosition, platformTile);
                    break;
                case TileType.Ladder:
                    ladderTileMap.SetTile(globalPosition, ladderTile);
                    break;
                case TileType.Spike:
                    spikeTileMap.SetTile(globalPosition, spikeTile);
                    break;
                default:
                    platformTileMap.SetTile(globalPosition, platformTile);
                    break;
            }
        }
        void EraseTile(Vector3Int globalPosition, TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Platform:
                    platformTileMap.SetTile(globalPosition, null);
                    break;
                case TileType.Ladder:
                    ladderTileMap.SetTile(globalPosition, null);
                    break;
                case TileType.Spike:
                    spikeTileMap.SetTile(globalPosition, null);
                    break;
                default:
                    platformTileMap.SetTile(globalPosition, null);
                    break;
            }
        }

        void SetTile(int cellNumber, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellNumber];
            DrawTile(globalPosition, tileType);
        }
        void SetTile(int startCell, int endCell, int roomNumber, TileType tileType)
        {
            int start = startCell;
            int end = endCell;
            if (startCell > endCell)
            {
                start = endCell;
                end = startCell;
            }

            //Tilemap targetTileMap;
            //TileBase targetTile;
            //
            //switch (tileType)
            //{
            //    case TileType.Platform:
            //        targetTileMap = platformTileMap;
            //        targetTile = platformTile;
            //        break;
            //    case TileType.Ladder:
            //        targetTileMap = ladderTileMap;
            //        targetTile = ladderTile;
            //        break;
            //    case TileType.Spike:
            //        targetTileMap = spikeTileMap;
            //        targetTile = spikeTile;
            //        break;
            //    default:
            //        targetTileMap = platformTileMap;
            //        targetTile = platformTile;
            //        break;
            //}

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                //targetTileMap.SetTile(globalPosition, targetTile);
                DrawTile(globalPosition, tileType);
            }
        }
        void SetTile(int colsNumber, int startCell, int endCell, int roomNumber, TileType tileType)
        {
            int start = startCell + colsNumber * roomWidth;
            int end = endCell + colsNumber * roomWidth;
            if (startCell > endCell)
            {
                start = endCell + colsNumber * roomWidth;
                end = startCell + colsNumber * roomWidth;
            }

            //Tilemap targetTileMap;
            //TileBase targetTile;
            //
            //switch (tileType)
            //{
            //    case TileType.Platform:
            //        targetTileMap = platformTileMap;
            //        targetTile = platformTile;
            //        break;
            //    case TileType.Ladder:
            //        targetTileMap = ladderTileMap;
            //        targetTile = ladderTile;
            //        break;
            //    case TileType.Spike:
            //        targetTileMap = spikeTileMap;
            //        targetTile = spikeTile;
            //        break;
            //    default:
            //        targetTileMap = platformTileMap;
            //        targetTile = platformTile;
            //        break;
            //}

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                DrawTile(globalPosition, tileType);
            }
        }
        void SetTile(Vector3Int localPosition, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(localPosition.x + roomGridPositionList[roomNumber].x * roomWidth,
                localPosition.y + roomGridPositionList[roomNumber].y * roomHeight, 0);
            DrawTile(globalPosition, tileType);
        }

        void RemoveTile(int cellNumber, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellNumber];
            EraseTile(globalPosition, tileType);
        }
        void RemoveTile(int startCell, int endCell, int roomNumber, TileType tileType)
        {
            int start = startCell;
            int end = endCell;
            if (startCell > endCell)
            {
                start = endCell;
                end = startCell;
            }

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                EraseTile(globalPosition, tileType);
            }
        }
        void RemoveTile(int colsNumber, int startCell, int endCell, int roomNumber, TileType tileType)
        {
            int start = startCell + colsNumber * roomWidth;
            int end = endCell + colsNumber * roomWidth;
            if (startCell > endCell)
            {
                start = endCell + colsNumber * roomWidth;
                end = startCell + colsNumber * roomWidth;
            }

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                EraseTile(globalPosition, tileType);
            }
        }
        void RemoveTile(Vector3Int localPosition, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(localPosition.x + roomGridPositionList[roomNumber].x * roomWidth,
                localPosition.y + roomGridPositionList[roomNumber].y * roomHeight, 0);
            EraseTile(globalPosition, tileType);
        }       

        void DrawTileVerticalLine(int startCell, int lineLength, int roomNumber, TileType tileType)
        {
            float length = lineLength;
            if (startCell + lineLength > roomHeight)
            {
                length = roomHeight - startCell;
            }
            for (int i = 0; i < length; i++)
            {
                SetTile(startCell + i * roomWidth, roomNumber, tileType);
            }
        }
        void DrawTileHorizontalLine(int startCell, int lineLength, int roomNumber, TileType tileType)
        {
            float length = lineLength;
            if (startCell + lineLength > roomWidth)
            {
                length = roomWidth - startCell;
            }
            for (int i = 0; i < length; i++)
            {
                SetTile(startCell + i, roomNumber, tileType);
            }
        }
        void DrawTileVerticalLine(int startCell, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomHeight - startCell; i++)
            {
                SetTile(startCell + i * roomWidth, roomNumber, tileType);
            }
        }
        void DrawTileHorizontalLine(int startCell, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomWidth - startCell; i++)
            {
                SetTile(startCell * roomHeight + i, roomNumber, tileType);
            }
        }
        void DrawVerticalLine(int lineNumber, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(lineNumber + i * roomWidth, roomNumber, tileType);
            }
        }
        void DrawHorizontalLine(int lineNumber, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomWidth; i++)
            {
                SetTile(lineNumber * roomWidth + i, roomNumber, tileType);
            }
        }

        void RemoveTileVerticalLine(int startCell, int lineLength, int roomNumber, TileType tileType)
        {
            float length = lineLength;
            if (startCell + lineLength > roomHeight)
            {
                length = roomHeight - startCell;
            }
            for (int i = 0; i < length; i++)
            {
                RemoveTile(startCell + i * roomWidth, roomNumber, tileType);
            }
        }
        void RemoveTileHorizontalLine(int startCell, int lineLength, int roomNumber, TileType tileType)
        {
            float length = lineLength;
            if (startCell + lineLength > roomWidth)
            {
                length = roomWidth - startCell;
            }
            for (int i = 0; i < length; i++)
            {
                RemoveTile(startCell + i, roomNumber, tileType);
            }
        }
        void RemoveTileVerticalLine(int startCell, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomHeight - startCell; i++)
            {
                RemoveTile(startCell + i * roomWidth, roomNumber, tileType);
            }
        }
        void RemoveTileHorizontalLine(int startCell, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomWidth - startCell; i++)
            {
                RemoveTile(startCell * roomHeight + i, roomNumber, tileType);
            }
        }
        void RemoveVerticalLine(int lineNumber, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomHeight; i++)
            {
                RemoveTile(lineNumber + i * roomWidth, roomNumber, tileType);
            }
        }
        void RemoveHorizontalLine(int lineNumber, int roomNumber, TileType tileType)
        {
            for (int i = 0; i < roomWidth; i++)
            {
                RemoveTile(lineNumber * roomWidth + i, roomNumber, tileType);
            }
        }
        #endregion

        #region Draw Objects Functions
        void SetEntranceObject(Vector3Int localPosition, int roomNumber)
        {
            if (!cellDicitonary.ContainsValue(localPosition))
            {
                return;
            }
            Vector3Int globalPosition = new Vector3Int(localPosition.x + roomGridPositionList[roomNumber].x * roomWidth,
                localPosition.y + roomGridPositionList[roomNumber].y * roomHeight, 0);
            Vector3 position = new Vector3(globalPosition.x, globalPosition.y, globalPosition.z);
            enterObject.transform.position = position;
        }
        void SetEntranceObject(int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber;
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = new Vector3(globalPosition.x + 2, globalPosition.y + 2, globalPosition.z);
            enterObject.transform.position = position;
        }
        void SetEntranceObject(int colsNumber, int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber + colsNumber * roomWidth;
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = new Vector3(globalPosition.x + 2, globalPosition.y + 2, globalPosition.z);
            enterObject.transform.position = position;
        }

        void SetExitObject(Vector3Int localPosition, int roomNumber)
        {
            if (!cellDicitonary.ContainsValue(localPosition))
            {
                return;
            }
            Vector3Int globalPosition = new Vector3Int(localPosition.x + roomGridPositionList[roomNumber].x * roomWidth,
                localPosition.y + roomGridPositionList[roomNumber].y * roomHeight, 0);
            Vector3 position = new Vector3(globalPosition.x, globalPosition.y, globalPosition.z);
            exitObject.transform.position = position;
        }
        void SetExitObject(int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber;
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = new Vector3(globalPosition.x + 2, globalPosition.y + 2, globalPosition.z);
            exitObject.transform.position = position;
        }
        void SetExitObject(int colsNumber, int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber + colsNumber * roomWidth;
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = new Vector3(globalPosition.x + 2, globalPosition.y + 2, globalPosition.z);
            exitObject.transform.position = position;
        }

        void SetPushBlockObject(Vector3Int localPosition, int roomNumber)
        {
            if (!cellDicitonary.ContainsValue(localPosition))
            {
                return;
            }
            Vector3Int globalPosition = new Vector3Int(localPosition.x + roomGridPositionList[roomNumber].x * roomWidth,
                localPosition.y + roomGridPositionList[roomNumber].y * roomHeight, 0);
            Vector3 position = new Vector3(globalPosition.x, globalPosition.y, globalPosition.z);
            GameObject pushBlockObject = ObjectPooler.instance.GetStaticPoolingObject(pushBlockInfo);
            pushBlockObject.transform.position = position;
            List<GameObject> pushBlockObjectList = pushBlockObjectDict[roomNumber];
            pushBlockObjectList.Add(pushBlockObject);
            pushBlockObjectDict.Remove(roomNumber);
            pushBlockObjectDict.Add(roomNumber, pushBlockObjectList);
            pushBlockObject.SetActive(false);
        }
        void SetPushBlockObject(int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber;
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = new Vector3(globalPosition.x + 1, globalPosition.y + 1, globalPosition.z);
            GameObject pushBlockObject = ObjectPooler.instance.GetStaticPoolingObject(pushBlockInfo);
            pushBlockObject.transform.position = position;
            List<GameObject> pushBlockObjectList = pushBlockObjectDict[roomNumber];
            pushBlockObjectList.Add(pushBlockObject);
            pushBlockObjectDict.Remove(roomNumber);
            pushBlockObjectDict.Add(roomNumber, pushBlockObjectList);
            pushBlockObject.SetActive(false);
        }
        void SetPushBlockObject(int colsNumber, int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber + colsNumber * roomWidth;
            Vector3Int globalPosition = new Vector3Int(roomGridPositionList[roomNumber].x * roomWidth, roomGridPositionList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = new Vector3(globalPosition.x + 1, globalPosition.y + 1, globalPosition.z);
            GameObject pushBlockObject = ObjectPooler.instance.GetStaticPoolingObject(pushBlockInfo);
            pushBlockObject.transform.position = position;
            List<GameObject> pushBlockObjectList = pushBlockObjectDict[roomNumber];
            pushBlockObjectList.Add(pushBlockObject);
            pushBlockObjectDict.Remove(roomNumber);
            pushBlockObjectDict.Add(roomNumber, pushBlockObjectList);
            pushBlockObject.SetActive(false);
        }
        #endregion

        #region Draw & Clear Room Functions
        void ClearRoom(int roomNumber)
        {
            for (int i = 0; i < roomHeight; i++)
            {
                RemoveTile(i, 0, roomWidth - 1, roomNumber, platform);
                RemoveTile(i, 0, roomWidth - 1, roomNumber, ladder);
                RemoveTile(i, 0, roomWidth - 1, roomNumber, spike);
            }

            ResetOpenPlaceableDict(roomNumber);

            for(int i = 0; i < pushBlockObjectDict[roomNumber].Count; i++)
            {
                ObjectPooler.instance.RecyclePoolingObject(pushBlockInfo, pushBlockObjectDict[roomNumber][i]);
            }
            pushBlockObjectDict.Remove(roomNumber);
            pushBlockObjectDict.Add(roomNumber, new List<GameObject>());
        }
        void DrawTiles(int roomNumber, TempletInfo templet)
        {
            for (int i = 0; i < templet.platformTileList.Count; i++)
            {
                openPlaceableDict[roomNumber].Remove(templet.platformTileList[i]);
                SetTile(templet.platformTileList[i], roomNumber, platform);
            }
            for (int i = 0; i < templet.ladderTileList.Count; i++)
            {
                openPlaceableDict[roomNumber].Remove(templet.ladderTileList[i]);
                SetTile(templet.ladderTileList[i], roomNumber, ladder);
            }
            for (int i = 0; i < templet.spikeTileList.Count; i++)
            {
                openPlaceableDict[roomNumber].Remove(templet.spikeTileList[i]);
                SetTile(templet.spikeTileList[i], roomNumber, spike);
            }

            for (int i = 0; i < templet.pushBlockList.Count; i++)
            {
                openPlaceableDict[roomNumber].Remove(templet.pushBlockList[i]);
                SetPushBlockObject(templet.pushBlockList[i], roomNumber);
            }
        }
        void ResetOpenPlaceableDict(int roomNumber)
        {
            List<Vector3Int> cell = new List<Vector3Int>();
            for (int i = 0; i < cellDicitonary.Count; i++)
            {
                cell.Add(cellDicitonary[i]);
            }
            openPlaceableDict.Add(roomNumber, cell);
        }
        void DrawRoom(int roomNumber, RoomType roomType)
        {
            if (roomNumber == pathList[0])
            {
                DrawEntrance(roomNumber, roomType);
            }
            else if (roomNumber == pathList[pathList.Count - 1])
            {
                DrawExit(roomNumber, roomType);
            }
            else
            {
                string directoryName = string.Concat(roomType.ToString(), "Templets");
                string directoryPath = Path.Combine(superDirectoryPath, directoryName);
                int templetCount = 0;
                if (Directory.Exists(directoryPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(directoryPath);
                    FileInfo[] files = dir.GetFiles("*.Json");
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i].Extension.Equals(".Json", System.StringComparison.OrdinalIgnoreCase))
                        {
                            templetCount++;
                        }
                    }
                }
                else
                {
                    templetCount = 1;
                }

                int random = Random.Range(0, templetCount);
                string templetName = string.Concat(directoryName, random.ToString(), ".Json");
                if (File.Exists(Path.Combine(directoryPath, templetName)))
                {
                    string path = Path.Combine(directoryPath, templetName);
                    string jsonData = File.ReadAllText(path);
                    TempletInfo templet = JsonUtility.FromJson<TempletInfo>(jsonData);

                    DrawTiles(roomNumber, templet);
                }
                else
                {
                    string str = string.Concat("Get", roomType.ToString(), "Templets0");
                    StartCoroutine(str, roomNumber);
                }
            }
        }
        //void DrawRoom(int roomNumber, RoomType roomType)
        //{
        //    if (roomNumber == pathList[0])
        //    {
        //        DrawEntrance(roomNumber, roomType);
        //    }
        //    else if(roomNumber == pathList[pathList.Count - 1])
        //    {
        //        DrawExit(roomNumber, roomType);
        //    }
        //    else
        //    {
        //        switch (roomType)
        //        {
        //            case RoomType.LR:
        //                DrawLR(roomNumber);
        //                break;
        //            case RoomType.LRB:
        //                DrawLRB(roomNumber);
        //                break;
        //            case RoomType.LRBT:
        //                DrawLRBT(roomNumber);
        //                break;
        //            case RoomType.LRT:
        //                DrawLRT(roomNumber);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}
        void DrawDetails(int roomNumber, RoomType roomType)
        {

        }

        //void DrawLR(int roomNumber)
        //{
        //    //string directoryName = "LRTemplets";
        //    //string directoryPath = Path.Combine(superDirectoryPath, directoryName);
        //    //int templetCount = 0;
        //    //if (Directory.Exists(directoryPath))
        //    //{
        //    //    DirectoryInfo dir = new DirectoryInfo(directoryPath);
        //    //    FileInfo[] files = dir.GetFiles("*.Json");
        //    //    for (int i = 0; i < files.Length; i++)
        //    //    {
        //    //        if (files[i].Extension.Contains(directoryName))
        //    //        {
        //    //            templetCount++;
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    templetCount = 1;
        //    //}

        //    //int random = Random.Range(0, templetCount);
        //    //string templetName = string.Concat(directoryName, random.ToString(), ".Json");
        //    //if (File.Exists(Path.Combine(directoryPath, templetName)))
        //    //{
        //    //    string path = Path.Combine(directoryPath, templetName);
        //    //    string jsonData = File.ReadAllText(path);
        //    //    TempletInfo templet = JsonUtility.FromJson<TempletInfo>(jsonData);

        //    //    for (int i = 0; i < templet.platformTileList.Count; i++)
        //    //    {
        //    //        openPlaceableDict[roomNumber].Remove(templet.platformTileList[i]);
        //    //        SetTile(templet.platformTileList[i], roomNumber, platform);
        //    //    }
        //    //    for (int i = 0; i < templet.ladderTileList.Count; i++)
        //    //    {
        //    //        openPlaceableDict[roomNumber].Remove(templet.ladderTileList[i]);
        //    //        SetTile(templet.ladderTileList[i], roomNumber, ladder);
        //    //    }
        //    //    for (int i = 0; i < templet.spikeTileList.Count; i++)
        //    //    {
        //    //        openPlaceableDict[roomNumber].Remove(templet.spikeTileList[i]);
        //    //        SetTile(templet.spikeTileList[i], roomNumber, spike);
        //    //    }

        //    //    SetEntranceObject(CheckObjectPlaceable(4, roomNumber, templet), roomNumber);
        //    //    for (int i = 0; i < 20; i++)
        //    //    {
        //    //        SetPushBlockObject(CheckObjectPlaceable(2, roomNumber, templet), roomNumber);

        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    string str = "GetLRemplets0";
        //    //    StartCoroutine(str, roomNumber);
        //    //}

        //    if (roomNumber >= grid * grid)
        //    {
        //        return;
        //    }
        //    SetTile(31, 0, 39, roomNumber, platform);
        //    SetTile(30, 0, 39, roomNumber, platform);
        //    for (int i = 0; i < 5; i++)
        //    {
        //        SetTile(i, 0, 39, roomNumber, platform);
        //    }
        //}
        //void DrawLRB(int roomNumber)
        //{
        //    if (roomNumber >= grid * grid)
        //    {
        //        return;
        //    }
        //    //int templet = Random.Range(0, 4);
        //    //switch (templet)
        //    //{
        //    //    case 0:
        //    //        Vector3Int cell = tileMap.WorldToCell(Vector3.zero);
        //    //        tileMap.SetTile(cell, ruleTile);
        //    //        break;
        //    //    case 1:
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //    SetTile(31, 0, 39, roomNumber, platform);
        //    SetTile(30, 0, 39, roomNumber, platform);
        //    for (int i = 0; i < 5; i++)
        //    {
        //        SetTile(i, 0, 19, roomNumber, platform);
        //    }
        //}
        //void DrawLRBT(int roomNumber)
        //{
        //    if (roomNumber >= grid * grid)
        //    {
        //        return;
        //    }
        //    SetTile(31, 0, 19, roomNumber, platform);
        //    SetTile(30, 0, 19, roomNumber, platform);
        //    for (int i = 0; i < 5; i++)
        //    {
        //        SetTile(i, 0, 19, roomNumber, platform);
        //    }
        //}
        //void DrawLRT(int roomNumber)
        //{
        //    if (roomNumber >= grid * grid)
        //    {
        //        return;
        //    }
        //    SetTile(31, 0, 19, roomNumber, platform);
        //    SetTile(30, 0, 19, roomNumber, platform);
        //    for (int i = 0; i < 5; i++)
        //    {
        //        SetTile(i, 0, 39, roomNumber, platform);
        //    }
        //}
        void DrawEntrance(int roomNumber, RoomType roomType)
        {
            string directoryName = string.Concat(roomType.ToString(), "EntranceTemplets");
            string directoryPath = Path.Combine(superDirectoryPath, directoryName);
            int templetCount = 0;
            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo dir = new DirectoryInfo(directoryPath);
                FileInfo[] files = dir.GetFiles("*.Json");
                for(int i = 0; i < files.Length; i++)
                {
                    if (files[i].Extension.Equals(".Json", System.StringComparison.OrdinalIgnoreCase))
                    {
                        templetCount++;
                    }
                }
            }
            else
            {
                templetCount = 1;
            }
            int random = Random.Range(0, templetCount);
            string templetName = string.Concat(directoryName, random.ToString(), ".Json");
            if (File.Exists(Path.Combine(directoryPath, templetName)))
            {
                string path = Path.Combine(directoryPath, templetName);
                string jsonData = File.ReadAllText(path);
                TempletInfo templet = JsonUtility.FromJson<TempletInfo>(jsonData);

                DrawTiles(roomNumber, templet);

                SetEntranceObject(CheckObjectPlaceable(4, roomNumber, templet), roomNumber);
                //for (int i = 0; i < 20; i++)
                //{
                //    SetPushBlockObject(CheckObjectPlaceable(2, roomNumber, templet), roomNumber);

                //}
            }
            else
            {
                string str = string.Concat("Get", roomType.ToString(), "EntranceTemplets0");
                StartCoroutine(str, roomNumber);
            }
        }
        void DrawExit(int roomNumber, RoomType roomType)
        {
            string directoryName = string.Concat(roomType.ToString(), "ExitTemplets");
            string directoryPath = Path.Combine(superDirectoryPath, directoryName);
            int templetCount = 0;
            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo dir = new DirectoryInfo(directoryPath);
                FileInfo[] files = dir.GetFiles("*.Json");
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Extension.Equals(".Json", System.StringComparison.OrdinalIgnoreCase))
                    {
                        templetCount++;
                    }
                }
            }
            else
            {
                templetCount = 1;
            }
            int random = Random.Range(0, templetCount);
            string templetName = string.Concat(directoryName, random.ToString(), ".Json");
            if (File.Exists(Path.Combine(directoryPath, templetName)))
            {
                string path = Path.Combine(directoryPath, templetName);
                string jsonData = File.ReadAllText(path);
                TempletInfo templet = JsonUtility.FromJson<TempletInfo>(jsonData);

                DrawTiles(roomNumber, templet);

                SetExitObject(CheckObjectPlaceable(4, roomNumber, templet), roomNumber);
            }
            else
            {
                string str = string.Concat("Get", roomType.ToString(), "ExitTemplets0");
                StartCoroutine(str, roomNumber);
            }
        }
        #endregion

        Vector3Int CheckObjectPlaceable(int objectGrid, int roomNumber, TempletInfo templet)
        {
            for (int i = 0; i < templet.platformTileList.Count; i++)
            {
                openPlaceableDict[roomNumber].Remove(templet.platformTileList[i]);
            }
            for (int i = 0; i < templet.ladderTileList.Count; i++)
            {
                openPlaceableDict[roomNumber].Remove(templet.ladderTileList[i]);
            }
            for (int i = 0; i < templet.spikeTileList.Count; i++)
            {
                openPlaceableDict[roomNumber].Remove(templet.spikeTileList[i]);
            }

            List<Vector3Int> closedPlaceableList = new List<Vector3Int>();
            for (int i = 0; i < openPlaceableDict[roomNumber].Count; i++)
            {
                bool canPlace = true;
                Vector3Int origin = openPlaceableDict[roomNumber][i];
                for (int j = -1; j < objectGrid + 1; j++)
                {
                    for (int k = 0; k < objectGrid; k++)
                    {
                        if (GetTile(origin + Vector3Int.right * j + Vector3Int.up * k, roomNumber, platform) != null ||
                            GetTile(origin + Vector3Int.right * j + Vector3Int.up * k, roomNumber, spike) != null ||
                            GetTile(origin + Vector3Int.right * j + Vector3Int.down, roomNumber, platform) == null ||
                            !openPlaceableDict[roomNumber].Contains(origin + Vector3Int.right * j + Vector3Int.up * k))
                        {
                            canPlace = false;
                        }
                    }
                }
                if (canPlace)
                {
                    closedPlaceableList.Add(origin);
                }
            }
            if(closedPlaceableList.Count == 0)
            {
                return new Vector3Int(-1, -1, 0);
            }
            int random = Random.Range(0, closedPlaceableList.Count);
            //foreach (Vector3Int v in closedPlaceableList)
            //{
            //    Debug.Log(v);
            //}
            for (int k = -1; k < objectGrid + 1; k++)
            {
                openPlaceableDict[roomNumber].Remove(closedPlaceableList[random] + Vector3Int.right * k);
            }
            return closedPlaceableList[random] + new Vector3Int(objectGrid / 2, objectGrid / 2, 0);

        }
        TileBase GetTile(Vector3Int localPosition, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(localPosition.x + roomGridPositionList[roomNumber].x * roomWidth,
                localPosition.y + roomGridPositionList[roomNumber].y * roomHeight, 0);
            switch (tileType)
            {
                case TileType.Platform:
                    return platformTileMap.GetTile(globalPosition);
                case TileType.Ladder:
                    return ladderTileMap.GetTile(globalPosition);
                case TileType.Spike:
                    return spikeTileMap.GetTile(globalPosition);
                default:
                    return platformTileMap.GetTile(globalPosition);
            }
            //if (platformTileMap.GetTile(globalPosition) != null)
            //{
            //    return platformTileMap.GetTile(globalPosition);
            //}
            //else if (ladderTileMap.GetTile(globalPosition) != null)
            //{
            //    return ladderTileMap.GetTile(globalPosition);
            //}
            //else
            //{
            //    return spikeTileMap.GetTile(globalPosition);
            //}
        }

        #region EntranceRoom Templets
        IEnumerator GetLREntranceTemplets0(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetEntranceObject(5, 33, roomNumber);
            SetPushBlockObject(5, 20, roomNumber);
            yield return null;
        }
        IEnumerator GetLREntranceTemplets1(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetEntranceObject(5, 0, roomNumber);
            SetPushBlockObject(5, 20, roomNumber);
            yield return null;
        }

        IEnumerator GetLRBEntranceTemplets0(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            for (int i = 0; i < 5; i++)
            {
                RemoveTile(i, 10, 29, roomNumber, platform);
            }
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetEntranceObject(5, 33, roomNumber);
            yield return null;
        }
        IEnumerator GetLRBEntranceTemplets1(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            for (int i = 0; i < 5; i++)
            {
                RemoveTile(i, 10, 29, roomNumber, platform);
            }
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetEntranceObject(5, 4, roomNumber);
            yield return null;
        }
        #endregion

        #region LRRoom Templets
        IEnumerator GetLRTemplets0(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            yield return null;
        }
        #endregion

        #region LRBRoom Templets

        #endregion

        #region LRBTRoom Templets

        #endregion

        #region LRTRoom Templets

        #endregion

        #region ExitRoom Templets
        IEnumerator GetLRExitTemplets0(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetExitObject(5, 33, roomNumber);
            yield return null;
        }
        IEnumerator GetLRExitTemplets1(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetExitObject(5, 4, roomNumber);
            yield return null;
        }

        IEnumerator GetLRTExitTemplets0(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            RemoveTile(31, 10, 29, roomNumber, platform);
            RemoveTile(30, 10, 29, roomNumber, platform);
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetExitObject(5, 33, roomNumber);
            yield return null;
        }
        IEnumerator GetLRTExitTemplets1(int roomNumber)
        {
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < roomHeight; i++)
            {
                SetTile(i * 40, roomNumber, platform);
            }
            for (int i = 1; i <= roomHeight; i++)
            {
                SetTile(i * 40 - 1, roomNumber, platform);
            }
            RemoveTile(31, 10, 29, roomNumber, platform);
            RemoveTile(30, 10, 29, roomNumber, platform);
            for (int i = 10; i < roomHeight - 10; i++)
            {
                RemoveTile(i * 40, roomNumber, platform);
            }
            for (int i = 11; i <= roomHeight - 10; i++)
            {
                RemoveTile(i * 40 - 1, roomNumber, platform);
            }
            SetExitObject(5, 4, roomNumber);
            yield return null;
        }
        #endregion

        void SetSolutionPath()
        {
            int cols = 0;
            int currentPath = 0;
            int desicion = 0;
            PathDirection currentDirection = PathDirection.None;

            currentPath = Random.Range(0, grid);
            pathList.Add(currentPath);
            excludedPathList.Remove(currentPath);
            if(currentPath % grid == 0)
            {
                roomDictionary.Add(currentPath, RoomType.R);
            }
            else if(currentPath % grid == grid - 1)
            {
                roomDictionary.Add(currentPath, RoomType.L);
            }
            else
            {
                roomDictionary.Add(currentPath, RoomType.LR);
            }

            while (cols < grid)
            {
                desicion = Random.Range(0, 5);
                //Move Right
                if (desicion == 0 || desicion == 1)
                {
                    if (currentDirection == PathDirection.Left)
                    {
                        roomDictionary.Remove(currentPath);
                        roomDictionary.Add(currentPath, RoomType.LRB);
                        currentPath += grid;
                        roomDictionary.Add(currentPath, RoomType.LRT);
                        cols += 1;
                        if (cols < grid)
                        {
                            currentDirection = PathDirection.Down;
                        }
                    }
                    else
                    {
                        if (currentPath % grid == grid - 1)
                        {
                            if (currentDirection == PathDirection.Down)
                            {
                                roomDictionary.Remove(currentPath);
                                roomDictionary.Add(currentPath, RoomType.LBT);
                            }
                            else
                            {
                                roomDictionary.Remove(currentPath);
                                roomDictionary.Add(currentPath, RoomType.LB);
                            }
                            currentPath += grid;
                            roomDictionary.Add(currentPath, RoomType.LT);
                            cols += 1;
                            if (cols < grid)
                            {
                                currentDirection = PathDirection.Down;
                            }
                        }
                        else
                        {
                            currentPath += 1;
                            roomDictionary.Add(currentPath, RoomType.LR);
                            currentDirection = PathDirection.Right;
                        }
                    }
                }
                //Move Left
                else if (desicion == 2 || desicion == 3)
                {
                    if (currentDirection == PathDirection.Right)
                    {
                        roomDictionary.Remove(currentPath);
                        roomDictionary.Add(currentPath, RoomType.LRB);
                        currentPath += grid;
                        roomDictionary.Add(currentPath, RoomType.LRT);
                        cols += 1;
                        if (cols < grid)
                        {
                            currentDirection = PathDirection.Down;
                        }
                    }
                    else
                    {
                        if (currentPath % grid == 0)
                        {
                            if (currentDirection == PathDirection.Down)
                            {
                                roomDictionary.Remove(currentPath);
                                roomDictionary.Add(currentPath, RoomType.RBT);
                            }
                            else
                            {
                                roomDictionary.Remove(currentPath);
                                roomDictionary.Add(currentPath, RoomType.RB);
                            }
                            currentPath += grid;
                            roomDictionary.Add(currentPath, RoomType.RT);
                            cols += 1;
                            if (cols < grid)
                            {
                                currentDirection = PathDirection.Down;
                            }
                        }
                        else
                        {
                            currentPath -= 1;
                            roomDictionary.Add(currentPath, RoomType.LR);
                            currentDirection = PathDirection.Left;
                        }
                    }
                }
                //Move Down
                else
                {
                    if (currentDirection == PathDirection.Down)
                    {
                        roomDictionary.Remove(currentPath);
                        roomDictionary.Add(currentPath, RoomType.LRBT);
                    }
                    else
                    {
                        roomDictionary.Remove(currentPath);
                        roomDictionary.Add(currentPath, RoomType.LRB);
                    }
                    currentPath += grid;
                    roomDictionary.Add(currentPath, RoomType.LRT);
                    cols++;
                    if (cols < grid)
                    {
                        currentDirection = PathDirection.Down;
                    }
                }
                //Last
                if (cols >= grid)
                {
                    currentPath -= grid;
                    if (currentDirection == PathDirection.Down)
                    {
                        roomDictionary.Remove(currentPath);
                        roomDictionary.Add(currentPath, RoomType.LRT);
                    }
                    else
                    {
                        roomDictionary.Remove(currentPath);
                        roomDictionary.Add(currentPath, RoomType.LR);
                    }
                    currentDirection = PathDirection.None;

                    string s = string.Empty;
                    string rs = string.Empty;
                    for (int i = 0; i < pathList.Count; i++)
                    {
                        s = string.Concat(s, "-", pathList[i]);
                        rs = string.Concat(rs, "-", roomDictionary[pathList[i]].ToString());
                    }
                    Debug.Log("Solution Path : " + s);
                    Debug.Log("Solution Path : " + rs);
                }

                excludedPathList.Remove(currentPath);
                pathList.Add(currentPath);
            }
        }
        void SetExcludedPath()
        {
            #region branch
            List<int> tempList = new List<int>();
            List<int> subList = new List<int>();

            for (int i = 0; i < excludedPathList.Count; i++)
            {
                tempList.Add(excludedPathList[i]);
            }

            for (int i = 0; i < excludedPathList.Count; i++)
            {
                string s = string.Empty;

                if (excludedPathList[i] % grid == 0)
                {
                    if (pathList.Contains(excludedPathList[i] + 1) ||
                       (pathList.Contains(excludedPathList[i] + grid) && excludedPathList[i] + grid < grid * grid) ||
                       (pathList.Contains(excludedPathList[i] - grid) && excludedPathList[i] - grid >= 0))
                    {
                        if (pathList.Contains(excludedPathList[i] + 1) ||
                            excludedPathList.Contains(excludedPathList[i] + 1))
                        {
                            s = string.Concat(s, "R");
                        }
                        if ((pathList.Contains(excludedPathList[i] + grid) ||
                            excludedPathList.Contains(excludedPathList[i] + grid)) &&
                            excludedPathList[i] + grid < grid * grid)
                        {
                            s = string.Concat(s, "B");
                        }
                        if ((pathList.Contains(excludedPathList[i] - grid) ||
                            excludedPathList.Contains(excludedPathList[i] - grid)) &&
                            excludedPathList[i] - grid >= 0)
                        {
                            s = string.Concat(s, "T");
                        }
                    }
                }
                else if (excludedPathList[i] % grid == grid - 1)
                {
                    if (pathList.Contains(excludedPathList[i] - 1) ||
                       (pathList.Contains(excludedPathList[i] + grid) && excludedPathList[i] + grid < grid * grid) ||
                       (pathList.Contains(excludedPathList[i] - grid) && excludedPathList[i] - grid >= 0))
                    {
                        if (pathList.Contains(excludedPathList[i] - 1) ||
                            excludedPathList.Contains(excludedPathList[i] - 1))
                        {
                            s = string.Concat(s, "L");
                        }
                        if ((pathList.Contains(excludedPathList[i] + grid) ||
                            excludedPathList.Contains(excludedPathList[i] + grid)) &&
                            excludedPathList[i] + grid < grid * grid)
                        {
                            s = string.Concat(s, "B");
                        }
                        if ((pathList.Contains(excludedPathList[i] - grid) ||
                            excludedPathList.Contains(excludedPathList[i] - grid)) &&
                            excludedPathList[i] - grid >= 0)
                        {
                            s = string.Concat(s, "T");
                        }
                    }
                }
                else
                {
                    if (pathList.Contains(excludedPathList[i] + 1) ||
                        pathList.Contains(excludedPathList[i] - 1) ||
                       (pathList.Contains(excludedPathList[i] + grid) && excludedPathList[i] + grid < grid * grid) ||
                       (pathList.Contains(excludedPathList[i] - grid) && excludedPathList[i] - grid >= 0))
                    {
                        if (pathList.Contains(excludedPathList[i] - 1) ||
                            excludedPathList.Contains(excludedPathList[i] - 1))
                        {
                            s = string.Concat(s, "L");
                        }
                        if (pathList.Contains(excludedPathList[i] + 1) ||
                            excludedPathList.Contains(excludedPathList[i] + 1))
                        {
                            s = string.Concat(s, "R");
                        }
                        if ((pathList.Contains(excludedPathList[i] + grid) ||
                            excludedPathList.Contains(excludedPathList[i] + grid)) &&
                            excludedPathList[i] + grid < grid * grid)
                        {
                            s = string.Concat(s, "B");
                        }
                        if ((pathList.Contains(excludedPathList[i] - grid) ||
                            excludedPathList.Contains(excludedPathList[i] - grid)) &&
                            excludedPathList[i] - grid >= 0)
                        {
                            s = string.Concat(s, "T");
                        }
                    }
                }
                if (s != string.Empty)
                {
                    RoomType roomType = (RoomType)System.Enum.Parse(typeof(RoomType), s);
                    roomDictionary.Add(excludedPathList[i], roomType);
                    tempList.Remove(excludedPathList[i]);
                    subList.Add(excludedPathList[i]);
                }
            }

            for (int row = 0; row < grid; row++)
            {
                List<int> tpList = new List<int>();
                for (int i = 0; i < tempList.Count; i++)
                {
                    tpList.Add(tempList[i]);
                }
                List<int> removedList = new List<int>();
                for (int i = 0; i < subList.Count; i++)
                {
                    removedList.Add(subList[i]);
                }
                subList.Clear();
                for (int i = 0; i < tpList.Count; i++)
                {
                    string s = string.Empty;

                    if (tpList[i] % grid == 0)
                    {
                        if (excludedPathList.Contains(tpList[i] + 1))
                        {
                            s = string.Concat(s, "R");
                        }
                        if (excludedPathList.Contains(tpList[i] + grid) &&
                            tpList[i] + grid < grid * grid)
                        {
                            s = string.Concat(s, "B");
                        }
                        if (excludedPathList.Contains(tpList[i] - grid) &&
                            tpList[i] - grid >= 0)
                        {
                            s = string.Concat(s, "T");
                        }
                    }
                    else if (tpList[i] % grid == grid - 1)
                    {
                        if (excludedPathList.Contains(tpList[i] - 1))
                        {
                            s = string.Concat(s, "L");
                        }
                        if (excludedPathList.Contains(tpList[i] + grid) &&
                            tpList[i] + grid < grid * grid)
                        {
                            s = string.Concat(s, "B");
                        }
                        if (excludedPathList.Contains(tpList[i] - grid) &&
                            tpList[i] - grid >= 0)
                        {
                            s = string.Concat(s, "T");
                        }
                    }
                    else
                    {
                        if (excludedPathList.Contains(tpList[i] - 1))
                        {
                            s = string.Concat(s, "L");
                        }
                        if (excludedPathList.Contains(tpList[i] + 1))
                        {
                            s = string.Concat(s, "R");
                        }
                        if (excludedPathList.Contains(tpList[i] + grid) &&
                            tpList[i] + grid < grid * grid)
                        {
                            s = string.Concat(s, "B");
                        }
                        if (excludedPathList.Contains(tpList[i] - grid) &&
                            tpList[i] - grid >= 0)
                        {
                            s = string.Concat(s, "T");
                        }
                    }
                    if (s != string.Empty)
                    {
                        subList.Add(tpList[i]);
                        if (tpList.Count == subList.Count)
                        {
                            int rand = Random.Range(0, s.Length);
                            char[] c = s.ToCharArray();
                            s = c[rand].ToString();
                        }
                        tempList.Remove(tpList[i]);
                        RoomType roomType = (RoomType)System.Enum.Parse(typeof(RoomType), s);
                        roomDictionary.Add(tpList[i], roomType);
                    }
                }
            }
            #endregion

            #region all LR
            //for (int i = 0; i < excludedPathList.Count; i++)
            //{
            //    roomDictionary.Add(excludedPathList[i], RoomType.LR);
            //}
            #endregion

            #region side-separate
            //for (int i = 0; i < excludedPathList.Count; i++)
            //{
            //    //LeftSide
            //    if (excludedPathList[i] % grid == 0)
            //    {
            //        int random = Random.Range(0, 4);
            //        if (random == 0)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.R);
            //        }
            //        else if (random == 1)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.RB);
            //        }
            //        else if (random == 2)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.RT);
            //        }
            //        else if (random == 3)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.RBT);
            //        }
            //        else if (random == 3)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.T);
            //        }
            //        else if (random == 3)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.B);
            //        }
            //    }
            //    //RightSide
            //    else if (excludedPathList[i] % grid == grid - 1)
            //    {
            //        int random = Random.Range(0, 4);
            //        if (random == 0)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.L);
            //        }
            //        else if (random == 1)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.LB);
            //        }
            //        else if (random == 2)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.LT);
            //        }
            //        else if (random == 3)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.LBT);
            //        }
            //    }
            //    else
            //    {
            //        int random = Random.Range(0, 4);
            //        if (random == 0)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.LR);
            //        }
            //        else if (random == 1)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.LRB);
            //        }
            //        else if (random == 2)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.LRT);
            //        }
            //        else if (random == 3)
            //        {
            //            roomDictionary.Add(excludedPathList[i], RoomType.LRBT);
            //        }
            //    }
            //}
            #endregion

            #region random
            //for (int i = 0; i < excludedPathList.Count; i++)
            //{
            //    int random = Random.Range(0, 15);
            //    if (random == 0)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.L);
            //    }
            //    else if (random == 1)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.R);
            //    }
            //    else if (random == 2)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.B);
            //    }
            //    else if (random == 3)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.T);
            //    }
            //    else if (random == 4)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.LR);
            //    }
            //    else if (random == 5)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.LT);
            //    }
            //    else if (random == 6)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.LB);
            //    }
            //    else if (random == 7)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.RB);
            //    }
            //    else if (random == 8)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.RT);
            //    }
            //    else if (random == 9)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.BT);
            //    }
            //    else if (random == 10)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.LRB);
            //    }
            //    else if (random == 11)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.LRT);
            //    }
            //    else if (random == 12)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.LBT);
            //    }
            //    else if (random == 13)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.RBT);
            //    }
            //    else if (random == 14)
            //    {
            //        roomDictionary.Add(excludedPathList[i], RoomType.LRBT);
            //    }
            //}
            #endregion
        }
        void DrawAllRooms()
        {
            string s = string.Empty;

            for (int i = 0; i < roomDictionary.Count - 1; i++)
            {
                DrawRoom(i, roomDictionary[i]);
                s = string.Concat(s, "-(", i, " : ",roomDictionary[i].ToString(), ")");
            }
            Debug.Log(s);
        }
    }

}