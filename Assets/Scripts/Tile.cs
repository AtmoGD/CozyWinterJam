using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gridsystem
{
    public class Tile : MonoBehaviour
    {
        public int x;
        public int y;
    }

    public struct TileData
    {
        public int x;
        public int y;

        public int index;
        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;

        public int cameFromNodeIndex;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}