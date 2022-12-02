using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Gridsystem;
using Grid = Gridsystem.Grid;

public enum GameState
{
    Playing,
    Building,
    Paused
}

public enum BuildState
{
    New,
    Edit,
    Delete
}

public class GameManager : MonoBehaviour
{
    #region References
    [SerializeField] private Grid grid = null;
    [SerializeField] private Pathfinder pathfinder = null;
    [SerializeField] private CameraController cameraController = null;
    [SerializeField] private UIController uiController = null;
    #endregion

    #region States
    [SerializeField] private GameState gameState = GameState.Playing;
    [SerializeField] private BuildState buildState = BuildState.New;
    #endregion

    #region Input
    public Vector2 MousePosition { get; private set; } = Vector2.zero;
    public Vector3 MouseWorldPosition { get { return Camera.main.ScreenToWorldPoint(MousePosition); } }
    public Vector2 MouseDelta { get; private set; } = Vector2.zero;
    #endregion

    #region Properties
    public float WorldTimeScale { get; private set; } = 1f;
    public Tile SelectedTile { get; private set; } = null;
    public GameObject SelectedObject { get; private set; } = null;
    #endregion

    private void Start()
    {
        grid.DeleteGrid();
        grid.CreateGrid();
    }

    private void Update()
    {
        if (!uiController.IsBuildingUIActive)
            cameraController.MoveCamera(MousePosition);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }

    public void OnMouseDelta(InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (SelectedTile)
                SelectedTile.Deselect();

            SelectedTile = grid.GetGridElement(MouseWorldPosition);

            if (SelectedTile)
                SelectedTile.Select();
        }
    }
}