using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gridsystem
{
    public class Tile : MonoBehaviour
    {
        public int x;
        public int y;

        private Vector2 startScale;

        private void Start()
        {
            startScale = transform.localScale;
        }

        public void Select()
        {
            transform.localScale = startScale * 1.1f;
        }

        public void Deselect()
        {
            transform.localScale = startScale;
        }
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