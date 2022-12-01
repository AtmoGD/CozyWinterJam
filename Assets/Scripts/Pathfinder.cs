using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System;
using Unity.Jobs;
using Unity.Burst;

// THANKS Code Monkey
// https://www.youtube.com/watch?v=1bO1FdEThnU&t=728s

namespace Gridsystem
{
    public class Pathfinder : MonoBehaviour
    {
        public const int MOVE_STRAIGHT_COST = 10;
        public const int MOVE_DIAGONAL_COST = 14;

        public List<Vector2Int> FindPath(Grid grid, Vector2Int _startPos, Vector2Int _targetPos)
        {
            NativeList<int2> pathList = new NativeList<int2>(grid.Width * grid.Height, Allocator.TempJob);
            NativeArray<TileData> gridElems = new NativeArray<TileData>(grid.Width * grid.Height, Allocator.TempJob);
            foreach (TileData tile in grid.GridArray)
            {
                gridElems[tile.index] = tile;
            }

            FindPathJob findPathJob = new FindPathJob
            {
                startPos = new int2(_startPos.x, _startPos.y),
                targetPos = new int2(_targetPos.x, _targetPos.y),
                gridElements = gridElems,
                gridSize = new int2(grid.Width, grid.Height),
                path = pathList
            };

            findPathJob.Run();

            List<Vector2Int> path = new List<Vector2Int>();

            for (int i = 0; i < findPathJob.path.Length; i++)
            {
                path.Add(new Vector2Int(findPathJob.path[i].x, findPathJob.path[i].y));
            }

            findPathJob.path.Dispose();
            findPathJob.gridElements.Dispose();

            return path;
        }



        [BurstCompile]
        public struct FindPathJob : IJob
        {
            public NativeArray<TileData> gridElements;
            public int2 startPos;
            public int2 targetPos;
            public int2 gridSize;
            public NativeList<int2> path;

            public void Execute()
            {
                FindPath(startPos, targetPos);
            }


            private void FindPath(int2 _startPosition, int2 _endPosition)
            {

                NativeArray<TileData> pathNodeArray = new NativeArray<TileData>(gridSize.x * gridSize.y, Allocator.Temp);

                foreach (TileData gridElement in gridElements)
                {
                    TileData pathNode = new TileData();
                    pathNode.x = gridElement.x;
                    pathNode.y = gridElement.y;
                    pathNode.index = CalculateIndex(pathNode.x, pathNode.y, gridSize.x);
                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(pathNode.x, pathNode.y), _endPosition);
                    pathNode.CalculateFCost();
                    pathNode.isWalkable = gridElement.isWalkable;
                    pathNode.cameFromNodeIndex = -1;
                    pathNodeArray[pathNode.index] = pathNode;
                }

                NativeArray<int2> neighbourOffsets = new NativeArray<int2>(8, Allocator.Temp);
                neighbourOffsets[0] = new int2(-1, 0);  // Left
                neighbourOffsets[1] = new int2(+1, 0);  // Right
                neighbourOffsets[2] = new int2(0, +1);  // Up
                neighbourOffsets[3] = new int2(0, -1);  // Down
                neighbourOffsets[4] = new int2(-1, -1); // Down Left
                neighbourOffsets[5] = new int2(+1, -1); // Down Right
                neighbourOffsets[6] = new int2(-1, +1); // Up Left
                neighbourOffsets[7] = new int2(+1, +1); // Up Right

                int endNodeIndex = CalculateIndex(_endPosition.x, _endPosition.y, gridSize.x);

                TileData startNode = pathNodeArray[CalculateIndex(_startPosition.x, _startPosition.y, gridSize.x)];
                startNode.gCost = 0;
                startNode.CalculateFCost();
                pathNodeArray[startNode.index] = startNode;

                NativeList<int> openList = new NativeList<int>(Allocator.Temp);
                NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

                openList.Add(startNode.index);

                while (openList.Length > 0)
                {
                    int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
                    TileData currentNode = pathNodeArray[currentNodeIndex];

                    if (currentNode.index == endNodeIndex)
                    {
                        break;
                    }

                    for (int i = openList.Length - 1; i >= 0; i--)
                    {
                        if (openList[i] == currentNodeIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }

                    closedList.Add(currentNodeIndex);

                    for (int i = 0; i < neighbourOffsets.Length; i++)
                    {
                        int2 neighbourOffset = neighbourOffsets[i];
                        int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                        {
                            continue;
                        }

                        int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                        if (closedList.Contains(neighbourNodeIndex))
                        {
                            continue;
                        }

                        TileData neighbourNode = pathNodeArray[neighbourNodeIndex];
                        if (!neighbourNode.isWalkable)
                        {
                            continue;
                        }

                        int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.cameFromNodeIndex = currentNodeIndex;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.hCost = CalculateDistanceCost(neighbourPosition, _endPosition);
                            neighbourNode.CalculateFCost();
                            pathNodeArray[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNodeIndex))
                            {
                                openList.Add(neighbourNodeIndex);
                            }
                        }
                    }
                }

                TileData endNode = pathNodeArray[endNodeIndex];
                if (endNode.cameFromNodeIndex == -1)
                {
                    Debug.Log("No Path Found");
                }
                else
                {
                    // path = CalculatePath(pathNodeArray, endNode);
                }

                neighbourOffsets.Dispose();
                openList.Dispose();
                closedList.Dispose();
            }

            public NativeList<int2> CalculatePath(NativeArray<TileData> _pathNodeArray, TileData _endNode)
            {
                if (_endNode.cameFromNodeIndex == -1)
                {
                    Debug.Log("No Path Found");
                    return new NativeList<int2>(Allocator.Temp);
                }
                else
                {
                    path.Add(new int2(_endNode.x, _endNode.y));

                    TileData currentNode = _endNode;
                    while (currentNode.cameFromNodeIndex != -1)
                    {
                        TileData cameFromNode = _pathNodeArray[currentNode.cameFromNodeIndex];
                        path.Add(new int2(cameFromNode.x, cameFromNode.y));
                        currentNode = cameFromNode;
                    }
                    return path;
                }
            }

            public bool IsPositionInsideGrid(int2 _position, int2 _gridSize)
            {
                return _position.x >= 0 && _position.x < _gridSize.x && _position.y >= 0 && _position.y < _gridSize.y;
            }

            public int CalculateIndex(int _x, int _y, int _width)
            {
                return _x + _y * _width;
            }

            public int CalculateDistanceCost(int2 _a, int2 _b)
            {
                int xDistance = math.abs(_a.x - _b.x);
                int yDistance = math.abs(_a.y - _b.y);
                int remaining = math.abs(xDistance - yDistance);
                return MOVE_STRAIGHT_COST * math.min(xDistance, yDistance) + MOVE_DIAGONAL_COST * remaining;
            }

            public int GetLowestCostFNodeIndex(NativeList<int> _openList, NativeArray<TileData> _pathNodeArray)
            {
                TileData lowestCostPathNode = _pathNodeArray[_openList[0]];
                for (int i = 0; i < _openList.Length; i++)
                {
                    TileData testPathNode = _pathNodeArray[_openList[i]];
                    if (testPathNode.fCost < lowestCostPathNode.fCost)
                    {
                        lowestCostPathNode = testPathNode;
                    }
                }
                return lowestCostPathNode.index;
            }
        }
    }
}