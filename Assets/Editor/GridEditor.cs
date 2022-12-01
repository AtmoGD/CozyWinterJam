using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Gridsystem
{
    [CustomEditor(typeof(Grid))]
    public class GridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Grid grid = (Grid)target;

            if (GUILayout.Button("Create Grid"))
            {
                grid.CreateGrid();
            }

            if (GUILayout.Button("Delete Grid"))
            {
                grid.DeleteGrid();
            }
        }
    }
}
