using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections;

namespace Gridsystem
{
    [Serializable]
    public class Grid : MonoBehaviour
    {
        [field: SerializeField] public int Width { get; private set; } = 10;
        [field: SerializeField] public int Height { get; private set; } = 10;
        [field: SerializeField] public float CellSize { get; private set; } = 1f;
        [field: SerializeField] public Vector3 OriginPosition { get; private set; } = Vector3.zero;

        [field: SerializeField] private Tile gridObject = null;

        [field: SerializeField] public Tile[,] GridElements { get; private set; } = null;
        // [field: SerializeField] public List<PathNode> GridArray { get; set; } = null;
        [field: SerializeField] public PathNode[] GridArray { get; set; } = null;


        public void CreateGrid()
        {
            GridElements = new Tile[Width, Height];
            GridArray = new PathNode[Width * Height];

            float xPosition = OriginPosition.x;
            float yPosition = OriginPosition.y;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Vector3 worldPosition = new Vector3(xPosition, yPosition);
                    Tile newTile = Instantiate(gridObject, worldPosition, Quaternion.identity, transform);
                    newTile.name = "Tile " + x + ", " + y;
                    newTile.x = x;
                    newTile.y = y;
                    GridElements[x, y] = newTile;

                    PathNode tileData = new PathNode();
                    tileData.x = x;
                    tileData.y = y;
                    tileData.index = x + y * Width;
                    tileData.isWalkable = true;
                    GridArray[tileData.index] = tileData;

                    yPosition += CellSize;
                }
                yPosition = OriginPosition.y;
                xPosition += CellSize;
            }
        }

        public void UpdateGridArray()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    PathNode tileData = GridArray[x + y * Width];
                    tileData.isWalkable = GridElements[x, y].currentObject == null;
                    GridArray[x + y * Width] = tileData;
                }
            }
        }

        public void DeleteGrid()
        {
            if (transform.childCount > 0)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }

            GridElements = null;
            GridArray = null;
        }

        public void GetGridElement(int x, int y, out Tile tile)
        {
            x = Mathf.Clamp(x, 0, Width - 1);
            y = Mathf.Clamp(y, 0, Height - 1);

            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                tile = GridElements[x, y];
            }
            else
            {
                tile = null;
            }
        }

        public Tile GetGridElement(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            GetGridElement(x, y, out Tile tile);
            return tile;
        }

        public void GetGridElement(Vector3 worldPosition, out Tile tile)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            GetGridElement(x, y, out tile);
        }

        public List<Tile> GetGridElements(Vector3 worldPosition, Vector2Int size)
        {
            List<Tile> tiles = new List<Tile>();

            int x, y;
            GetXY(worldPosition, out x, out y);

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    GetGridElement(x + i, y + j, out Tile tile);
                    tiles.Add(tile);
                }
            }

            return tiles;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.RoundToInt((worldPosition - OriginPosition).x / CellSize);
            y = Mathf.RoundToInt((worldPosition - OriginPosition).y / CellSize);
        }
    }
}