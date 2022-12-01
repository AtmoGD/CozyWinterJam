using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

namespace Gridsystem
{
    [CustomEditor(typeof(Pathfinder))]
    public class PathfinderTester : Editor
    {
        [SerializeField] private Grid grid = null;
        private List<Vector2Int> path;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Pathfinder pathfinder = (Pathfinder)target;

            if (GUILayout.Button("Find Path"))
            {
                path = pathfinder.FindPath(grid, new Vector2Int(0, 0), new Vector2Int(5, 5));
            }

            if (GUILayout.Button("Clear Path"))
            {
                path.Clear();
            }
        }

        private void OnSceneGUI()
        {
            if (path != null)
            {
                Handles.color = Color.red;
                foreach (Vector2Int pathNode in path)
                {
                    Handles.DrawWireCube(new Vector3(pathNode.x, 0, pathNode.y), Vector3.one * .5f);
                }
            }
        }
    }
}