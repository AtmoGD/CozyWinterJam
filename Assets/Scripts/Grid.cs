using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Gridsystem
{
    public class Grid : MonoBehaviour
    {
        [field: SerializeField] public int Width { get; private set; } = 10;
        [field: SerializeField] public int Height { get; private set; } = 10;
        [field: SerializeField] public float CellSize { get; private set; } = 1f;
        [field: SerializeField] public Vector3 OriginPosition { get; private set; } = Vector3.zero;

        [SerializeField] private Tile gridObject = null;

        [field: SerializeField] public Tile[,] GridElements { get; private set; } = null;
        [field: SerializeField] public List<TileData> GridArray { get; private set; } = null;


        public void CreateGrid()
        {
            GridElements = new Tile[Width, Height];
            GridArray = new List<TileData>();

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

                    TileData tileData = new TileData();
                    tileData.x = x;
                    tileData.y = y;
                    tileData.index = x + y * Width;
                    tileData.isWalkable = true;
                    GridArray.Add(tileData);

                    yPosition += CellSize;
                }
                yPosition = OriginPosition.y;
                xPosition += CellSize;
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
    }
}