using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class LevelGenerator : MonoBehaviour
    {
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
        int rows;
        int cols;
        int roomWidth;
        int roomHeight;
        int grid;

        enum PathDirection
        {
            None, Left, Right, Down
        };
        enum RoomType
        {
            Void, LR, LRB, LRBT, LRT
        };
        enum TileType
        {
            Platform, Ladder, LadderPlatform, ObstacleGround, ObstacleAir, Spike, Stairs
        };
        List<Vector3Int> gridList = new List<Vector3Int>();
        Dictionary<int, Vector3Int> globalPosDictionary = new Dictionary<int, Vector3Int>();
        //Dictionary<Vector3Int, TileType> cellDicitonary = new Dictionary<Vector3Int, TileType>();
        Dictionary<int, Vector3Int> cellDicitonary = new Dictionary<int, Vector3Int>();
        Dictionary<RoomType, Dictionary<Vector3Int, TileType>> roomDictionary =
            new Dictionary<RoomType, Dictionary<Vector3Int, TileType>>();
        List<int> excludedPathList = new List<int>();
        List<int> pathList = new List<int>();

        GameObject enterObject;
        GameObject exitObject;

        TileType platform;
        TileType ladder;
        TileType spike;

        void Start()
        {
            grid = 4;
            roomWidth = 40;
            roomHeight = 32;
            enterObject = Instantiate(entrance);
            exitObject = Instantiate(exit);

            platform = TileType.Platform;
            ladder = TileType.Ladder;
            spike = TileType.Spike;

            Vector3Int cell = platformTileMap.WorldToCell(Vector3.zero);
            DrawOutFrame(cell, roomWidth * grid + 1, roomHeight * grid + 1);
            SetGridList();
            SetCellDict();
            SetExcludedPathList();

            SetSolutionPath();
        }

        void InitLevelGenerator()
        {

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
        void DrawOutFrame(Vector3Int origin, int width, int height)
        {
            Vector3Int cell = origin;
            cell.x -= 1;
            cell.y -= 1;
            for (int i = 0; i < width; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.x += 1;
            }
            for (int i = 0; i < height; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.y += 1;
            }
            for (int i = 0; i < width; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.x -= 1;
            }
            for (int i = 0; i < height; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.y -= 1;
            }
            cell.x -= 1;
            cell.y -= 1;
            for (int i = 0; i < width + 2; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.x += 1;
            }
            for (int i = 0; i < height + 2; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.y += 1;
            }
            for (int i = 0; i < width + 2; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.x -= 1;
            }
            for (int i = 0; i < height + 2; i++)
            {
                platformTileMap.SetTile(cell, platformTile);
                cell.y -= 1;
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
                    gridList.Add(cell);
                }
            }
            //foreach (Vector3Int v in gridList)
            //{
            //    Debug.Log(v);
            //}
        }

        void SetTile(int cellNumber, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellNumber];
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
        void SetTile(int startCell, int endCell, int roomNumber, TileType tileType)
        {
            int start = startCell;
            int end = endCell;
            if (startCell > endCell)
            {
                start = endCell;
                end = startCell;
            }

            Tilemap targetTileMap;
            TileBase targetTile;

            switch (tileType)
            {
                case TileType.Platform:
                    targetTileMap = platformTileMap;
                    targetTile = platformTile;
                    break;
                case TileType.Ladder:
                    targetTileMap = ladderTileMap;
                    targetTile = ladderTile;
                    break;
                case TileType.Spike:
                    targetTileMap = spikeTileMap;
                    targetTile = spikeTile;
                    break;
                default:
                    targetTileMap = platformTileMap;
                    targetTile = platformTile;
                    break;
            }

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                targetTileMap.SetTile(globalPosition, targetTile);
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

            Tilemap targetTileMap;
            TileBase targetTile;

            switch (tileType)
            {
                case TileType.Platform:
                    targetTileMap = platformTileMap;
                    targetTile = platformTile;
                    break;
                case TileType.Ladder:
                    targetTileMap = ladderTileMap;
                    targetTile = ladderTile;
                    break;
                case TileType.Spike:
                    targetTileMap = spikeTileMap;
                    targetTile = spikeTile;
                    break;
                default:
                    targetTileMap = platformTileMap;
                    targetTile = platformTile;
                    break;
            }

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                targetTileMap.SetTile(globalPosition, targetTile);
            }
        }
        void SetTile(Vector3Int localPosition, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(localPosition.x + gridList[roomNumber].x * roomWidth,
                localPosition.y + gridList[roomNumber].y * roomHeight, 0);
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

        void RemoveTile(int cellNumber, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellNumber];
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
        void RemoveTile(int startCell, int endCell, int roomNumber, TileType tileType)
        {
            int start = startCell;
            int end = endCell;
            if (startCell > endCell)
            {
                start = endCell;
                end = startCell;
            }

            Tilemap targetTileMap;

            switch (tileType)
            {
                case TileType.Platform:
                    targetTileMap = platformTileMap;
                    break;
                case TileType.Ladder:
                    targetTileMap = ladderTileMap;
                    break;
                case TileType.Spike:
                    targetTileMap = spikeTileMap;
                    break;
                default:
                    targetTileMap = platformTileMap;
                    break;
            }

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                targetTileMap.SetTile(globalPosition, null);
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

            Tilemap targetTileMap;

            switch (tileType)
            {
                case TileType.Platform:
                    targetTileMap = platformTileMap;
                    break;
                case TileType.Ladder:
                    targetTileMap = ladderTileMap;
                    break;
                case TileType.Spike:
                    targetTileMap = spikeTileMap;
                    break;
                default:
                    targetTileMap = platformTileMap;
                    break;
            }

            for (int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                targetTileMap.SetTile(globalPosition, null);
            }
        }
        void RemoveTile(Vector3Int localPosition, int roomNumber, TileType tileType)
        {
            Vector3Int globalPosition = new Vector3Int(localPosition.x + gridList[roomNumber].x * roomWidth,
                localPosition.y + gridList[roomNumber].y * roomHeight, 0);
            platformTileMap.SetTile(globalPosition, null);
        }
        
        void SetEntranceObject(int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber + 2;
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = globalPosition;
            enterObject.transform.position = position;
        }
        void SetEntranceObject(int colsNumber, int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber + 2 + colsNumber * roomWidth;
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = globalPosition;
            enterObject.transform.position = position;
        }

        void SetExitObject(int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber + 2;
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = globalPosition;
            exitObject.transform.position = position;
        }
        void SetExitObject(int colsNumber, int cellNumber, int roomNumber)
        {
            int cellPosition = cellNumber + 2 + colsNumber * roomWidth;
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellPosition];
            Vector3 position = globalPosition;
            exitObject.transform.position = position;
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
            SetEntranceObject(6, 33, roomNumber);
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
            SetEntranceObject(6, 4, roomNumber);
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
            SetEntranceObject(6, 33, roomNumber);
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
            SetEntranceObject(6, 4, roomNumber);
            yield return null;
        }

        #endregion

        #region LRRoom Templets

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
            SetExitObject(6, 33, roomNumber);
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
            SetExitObject(6, 4, roomNumber);
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
            SetExitObject(6, 33, roomNumber);
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
            SetExitObject(6, 4, roomNumber);
            yield return null;
        }
        #endregion

        void DrawCell()
        {

        }

        void ClearRoom(int roomNumber)
        {
            for (int i = 0; i < roomHeight; i++)
            {
                RemoveTile(i, 0, roomWidth - 1, roomNumber, platform);
                RemoveTile(i, 0, roomWidth - 1, roomNumber, ladder);
                RemoveTile(i, 0, roomWidth - 1, roomNumber, spike);
            }
        }
        void DrawRoom(int roomNumber, RoomType roomType)
        {
            if (roomNumber == pathList[0])
            {
                DrawEntrance(roomNumber, roomType);
            }
            else
            {
                switch (roomType)
                {
                    case RoomType.LR:
                        DrawLR(roomNumber);
                        break;
                    case RoomType.LRB:
                        DrawLRB(roomNumber);
                        break;
                    case RoomType.LRBT:
                        DrawLRBT(roomNumber);
                        break;
                    case RoomType.LRT:
                        DrawLRT(roomNumber);
                        break;
                    default:
                        break;
                }
            }
        }

        void DrawLR(int roomNumber)
        {
            if (roomNumber >= grid * grid)
            {
                return;
            }
            ClearRoom(roomNumber);
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
        }
        void DrawLRB(int roomNumber)
        {
            if (roomNumber >= grid * grid)
            {
                return;
            }
            ClearRoom(roomNumber);
            //int templet = Random.Range(0, 4);
            //switch (templet)
            //{
            //    case 0:
            //        Vector3Int cell = tileMap.WorldToCell(Vector3.zero);
            //        tileMap.SetTile(cell, ruleTile);
            //        break;
            //    case 1:
            //        break;
            //    default:
            //        break;
            //}
            SetTile(31, 0, 39, roomNumber, platform);
            SetTile(30, 0, 39, roomNumber, platform);
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 19, roomNumber, platform);
            }
        }
        void DrawLRBT(int roomNumber)
        {
            if (roomNumber >= grid * grid)
            {
                return;
            }
            ClearRoom(roomNumber);
            SetTile(31, 0, 19, roomNumber, platform);
            SetTile(30, 0, 19, roomNumber, platform);
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 19, roomNumber, platform);
            }
        }
        void DrawLRT(int roomNumber)
        {
            if (roomNumber >= grid * grid)
            {
                return;
            }
            ClearRoom(roomNumber);
            SetTile(31, 0, 19, roomNumber, platform);
            SetTile(30, 0, 19, roomNumber, platform);
            for (int i = 0; i < 5; i++)
            {
                SetTile(i, 0, 39, roomNumber, platform);
            }
        }
        void DrawEntrance(int roomNumber, RoomType roomType)
        {
            int random = Random.Range(0, 2);
            string str = string.Concat("Get", roomType.ToString(), "EntranceTemplets", random.ToString());
            StartCoroutine(str, roomNumber);
        }
        void DrawExit(int roomNumber, RoomType roomType)
        {
            int random = Random.Range(0, 2);
            string str = string.Concat("Get", roomType.ToString(), "ExitTemplets", random.ToString());
            StartCoroutine(str, roomNumber);
        }
        void SetSolutionPath()
        {
            //int rows = 0;
            int cols = 0;
            int currentPath = 0;
            int desicion = 0;
            PathDirection currentDirection = PathDirection.None;

            currentPath = Random.Range(0, 4);
            pathList.Add(currentPath);
            excludedPathList.Remove(currentPath);
            DrawEntrance(currentPath, RoomType.LR);

            while (cols < grid)
            {
                desicion = Random.Range(0, 5);
                if (desicion == 0 || desicion == 1)
                {
                    if (currentDirection == PathDirection.Left)
                    {
                        DrawRoom(currentPath, RoomType.LRB);
                        currentPath += grid;
                        DrawRoom(currentPath, RoomType.LRT);
                        currentDirection = PathDirection.Down;
                        cols += 1;
                    }
                    else
                    {
                        if (currentPath % grid == grid - 1)
                        {
                            if (currentDirection == PathDirection.Down)
                            {
                                DrawRoom(currentPath, RoomType.LRBT);
                            }
                            else
                            {
                                DrawRoom(currentPath, RoomType.LRB);
                            }
                            currentPath += grid;
                            DrawRoom(currentPath, RoomType.LRT);
                            currentDirection = PathDirection.Down;
                            cols += 1;
                        }
                        else
                        {
                            currentPath += 1;
                            DrawRoom(currentPath, RoomType.LR);
                            currentDirection = PathDirection.Right;
                        }
                    }
                }
                else if (desicion == 2 || desicion == 3)
                {
                    if (currentDirection == PathDirection.Right)
                    {
                        DrawRoom(currentPath, RoomType.LRB);
                        currentPath += grid;
                        DrawRoom(currentPath, RoomType.LRT);
                        currentDirection = PathDirection.Down;
                        cols += 1;
                    }
                    else
                    {
                        if (currentPath % grid == 0)
                        {
                            if (currentDirection == PathDirection.Down)
                            {
                                DrawRoom(currentPath, RoomType.LRBT);
                            }
                            else
                            {
                                DrawRoom(currentPath, RoomType.LRB);
                            }
                            currentPath += grid;
                            DrawRoom(currentPath, RoomType.LRT);
                            currentDirection = PathDirection.Down;
                            cols += 1;
                        }
                        else
                        {
                            currentPath -= 1;
                            DrawRoom(currentPath, RoomType.LR);
                            currentDirection = PathDirection.Left;
                        }
                    }
                }
                else
                {
                    if (currentDirection == PathDirection.Down)
                    {
                        DrawRoom(currentPath, RoomType.LRBT);
                    }
                    else
                    {
                        DrawRoom(currentPath, RoomType.LRB);
                    }
                    currentPath += grid;
                    DrawRoom(currentPath, RoomType.LRT);
                    cols++;
                    if (cols < grid)
                    {
                        currentDirection = PathDirection.Down;
                    }
                }
                if (cols >= grid)
                {
                    currentPath -= grid;
                    if (currentDirection == PathDirection.Down)
                    {
                        DrawExit(currentPath, RoomType.LRT);
                    }
                    else
                    {
                        DrawExit(currentPath, RoomType.LR);
                    }
                    currentDirection = PathDirection.None;

                    for (int i = 0; i < excludedPathList.Count; i++)
                    {
                        DrawRoom(excludedPathList[i], RoomType.LR);
                    }

                    string s = string.Empty;
                    for (int i = 0; i < pathList.Count; i++)
                    {
                        s = string.Concat(s, "-", pathList[i]);
                    }
                    Debug.Log(s);
                }

                excludedPathList.Remove(currentPath);
                pathList.Add(currentPath);
            }
        }

        // do late so that the player has a chance to move in update if necessary
        //private void LateUpdate()
        //{
        //    // get current grid location
        //    Vector3Int currentCell = tileMap.WorldToCell(Vector3.zero);
        //    // add one in a direction (you'll have to change this to match your directional control)
        //    currentCell.x += 1;

        //    // if the position has changed
        //    if (currentCell != previous)
        //    {
        //        // set the new tile
        //        tileMap.SetTile(currentCell, ruleTile);

        //        // erase previous
        //        tileMap.SetTile(previous, null);

        //        // save the new position for next frame
        //        previous = currentCell;
        //    }
        //}
    }

}