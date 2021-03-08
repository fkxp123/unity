using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MomodoraCopy
{
    public class TileMapTest : MonoBehaviour
    {
        public Tilemap tileMap;
        public RuleTile ruleTile;
        public Tile spikeTile;
        public Tile pushTile;
        public Tile stairsTile;
        public Tile ladderTile;

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
            Void, Platform, Ladder, LadderPlatform, ObstacleGround, ObstacleAir, Spike, PushBlock, Stairs 
        };
        List<Vector3Int> gridList = new List<Vector3Int>();
        Dictionary<int, Vector3Int> globalPosDictionary = new Dictionary<int, Vector3Int>();
        //Dictionary<Vector3Int, TileType> cellDicitonary = new Dictionary<Vector3Int, TileType>();
        Dictionary<int, Vector3Int> cellDicitonary = new Dictionary<int, Vector3Int>();
        Dictionary<RoomType, Dictionary<Vector3Int, TileType>> roomDictionary = 
            new Dictionary<RoomType, Dictionary<Vector3Int, TileType>>();
        List<int> pathList = new List<int>();

        void Start()
        {
            grid = 4;
            roomWidth = 40;
            roomHeight = 32;
            Vector3Int cell = tileMap.WorldToCell(Vector3.zero);
            DrawOutFrame(cell, roomWidth * grid + 1, roomHeight * grid + 1);
            SetGridList();
            SetCellDict();
            SetSolutionPath();
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
                tileMap.SetTile(cell, ruleTile);
                cell.x += 1;
            }
            for (int i = 0; i < height; i++)
            {
                tileMap.SetTile(cell, ruleTile);
                cell.y += 1;
            }
            for (int i = 0; i < width; i++)
            {
                tileMap.SetTile(cell, ruleTile);
                cell.x -= 1;
            }
            for (int i = 0; i < height; i++)
            {
                tileMap.SetTile(cell, ruleTile);
                cell.y -= 1;
            }
        }
        void SetGridList()
        {
            Vector3Int cell = tileMap.WorldToCell(Vector3.zero);
            for(int i = grid - 1; i >= 0; i--)
            {
                cell.y = i;
                for(int j = 0; j < grid; j++)
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
        void SetTile(int cellNumber, int roomNumber, TileBase tile)
        {
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellNumber];
            tileMap.SetTile(globalPosition, tile);
        }
        void SetTile(int startCell, int endCell, int roomNumber, TileBase tile)
        {
            int start = startCell;
            int end = endCell;
            if(startCell > endCell)
            {
                start = endCell;
                end = startCell;
            }
            for(int i = start; i <= end; i++)
            {
                Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                tileMap.SetTile(globalPosition, tile);
            }
        }
        void SetTile(Vector3Int localPosition, int roomNumber, TileBase tile)
        {
            Vector3Int globalPosition = new Vector3Int(localPosition.x + gridList[roomNumber].x * roomWidth, 
                localPosition.y + gridList[roomNumber].y * roomHeight, 0);
            tileMap.SetTile(globalPosition, tile);
        }

        void RemoveTile(int cellNumber, int roomNumber, TileBase tile)
        {
            Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                + cellDicitonary[cellNumber];
            tileMap.SetTile(globalPosition, null);
        }
        void RemoveTile(int startCell, int endCell, int roomNumber, TileBase tile)
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
                Vector3Int globalPosition = new Vector3Int(gridList[roomNumber].x * roomWidth, gridList[roomNumber].y * roomHeight, 0)
                    + cellDicitonary[i];
                tileMap.SetTile(globalPosition, null);
            }
        }
        void RemoveTile(Vector3Int localPosition, int roomNumber, TileBase tile)
        {
            Vector3Int globalPosition = new Vector3Int(localPosition.x + gridList[roomNumber].x * roomWidth,
                localPosition.y + gridList[roomNumber].y * roomHeight, 0);
            tileMap.SetTile(globalPosition, null);
        }

        void DrawCell()
        {
            
        }
        void DrawRoom0(int roomNumber)
        {

        }
        void DrawRoom1(int roomNumber)
        {
            int templet = Random.Range(0, 4);
            switch (templet)
            {
                case 0:
                    Vector3Int cell = tileMap.WorldToCell(Vector3.zero);
                    tileMap.SetTile(cell, ruleTile);
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }
        void DrawRoom2(int roomNumber)
        {

        }
        void DrawRoom3(int roomNumber)
        {

        }
        void DrawEntrance(int roomNumber)
        {
            Vector3Int cell = tileMap.WorldToCell(Vector3.zero);
            SetTile(0, 80, roomNumber, ruleTile);
            SetTile(81, 119, roomNumber, ruleTile);
            SetTile(122, 158, roomNumber, ruleTile);
            SetTile(163, 197, roomNumber, ruleTile);
        }
        void DrawExit(int roomNumber)
        {

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
            DrawEntrance(currentPath);

            while (cols < grid)
            {
                desicion = Random.Range(0, 5);
                if (desicion == 0 || desicion == 1)
                {
                    if (currentDirection == PathDirection.Left)
                    {
                        currentPath += grid;
                        currentDirection = PathDirection.Down;
                        cols += 1;
                    }
                    else
                    {
                        if (currentPath % grid == grid - 1)
                        {
                            currentPath += grid;
                            currentDirection = PathDirection.Down;
                            cols += 1;
                        }
                        else
                        {
                            currentPath += 1;
                            currentDirection = PathDirection.Right;
                        }
                    }
                }
                else if (desicion == 2 || desicion == 3)
                {
                    if (currentDirection == PathDirection.Right)
                    {
                        currentPath += grid;
                        currentDirection = PathDirection.Down;
                        cols += 1;
                    }
                    else
                    {
                        if (currentPath % grid == 0)
                        {
                            currentPath += grid;
                            currentDirection = PathDirection.Down;
                            cols += 1;
                        }
                        else
                        {
                            currentPath -= 1;
                            currentDirection = PathDirection.Left;
                        } 
                    }
                }
                else
                {
                    currentPath += grid;
                    cols++;
                }
                if(cols >= grid)
                {
                    currentPath -= grid;
                    DrawExit(currentPath);
                    foreach (int path in pathList)
                    {
                        Debug.Log(path);
                    }
                    return;
                }

                pathList.Add(currentPath);
            }
        }
        public struct Room
        {
            public int width;
            public int height;

            Dictionary<Vector3Int, TileType> cellDicitonary;
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