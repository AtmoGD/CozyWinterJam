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
        [SerializeField] private Vector2Int startPos = new Vector2Int(0, 0);
        [SerializeField] private Vector2Int targetPos = new Vector2Int(0, 0);
        [SerializeField] private List<Vector2Int> path;
        [SerializeField] private bool showEditor = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            showEditor = EditorGUILayout.Toggle("Show Debugger Options", showEditor);
            if (!showEditor) return;


            Pathfinder pathfinder = (Pathfinder)target;

            if (!grid)
                grid = FindObjectOfType<Grid>();

            startPos = EditorGUILayout.Vector2IntField("Start Pos", startPos);
            targetPos = EditorGUILayout.Vector2IntField("Target Pos", targetPos);

            if (GUILayout.Button("Find Path"))
            {
                path = pathfinder.FindPath(grid, startPos, targetPos);
                Debug.Log("Path Length: " + path.Count);
            }

            if (GUILayout.Button("Clear Path"))
            {
                path.Clear();
            }
        }

        private void OnSceneGUI()
        {
            if (path != null && showEditor)
            {
                Handles.color = Color.red;
                // for (int i = 0; i < path.Count - 1; i++)
                // {
                //     Vector3 startPos = new Vector3(path[i].x, path[i].y);
                //     Vector3 endPos = new Vector3(path[i + 1].x, path[i + 1].y);
                //     Vector3 dir = (endPos - startPos).normalized;
                //     endPos = startPos + dir * 0.9f;
                //     Handles.DrawLine(startPos, endPos);
                // }

                Handles.color = Color.green;
                for (int j = path.Count - 1; j > 0; j--)
                {
                    Vector3 startPos = new Vector3(path[j].x, path[j].y);
                    Vector3 endPos = new Vector3(path[j - 1].x, path[j - 1].y);

                    Vector3 dir = (endPos - startPos).normalized;
                    endPos = startPos + dir * 0.9f;
                    Handles.DrawLine(startPos, endPos);
                }
            }
        }
    }
}