using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gridsystem
{
    public class Tile : MonoBehaviour
    {
        public int x;
        public int y;

        public GameObject currentObject = null;

        [SerializeField] private GameObject canBePlacedVizualizer = null;
        [SerializeField] private GameObject canNotBePlacedVizualizer = null;

        private void Start()
        {
            canBePlacedVizualizer.SetActive(false);
            canNotBePlacedVizualizer.SetActive(false);
        }

        public void Select()
        {
            if (currentObject == null)
            {
                canBePlacedVizualizer.SetActive(true);
                canNotBePlacedVizualizer.SetActive(false);
            }
            else
            {
                canBePlacedVizualizer.SetActive(false);
                canNotBePlacedVizualizer.SetActive(true);
            }
        }

        public void Deselect()
        {
            canBePlacedVizualizer.SetActive(false);
            canNotBePlacedVizualizer.SetActive(false);
        }
    }

    public struct PathNode
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

        public void SetIsWalkable(bool value)
        {
            isWalkable = value;
        }

        public PathNode GetCopy()
        {
            PathNode copy = new PathNode();
            copy.x = x;
            copy.y = y;
            copy.index = index;
            copy.gCost = gCost;
            copy.hCost = hCost;
            copy.fCost = fCost;
            copy.isWalkable = isWalkable;
            copy.cameFromNodeIndex = cameFromNodeIndex;
            return copy;
        }
    }
}