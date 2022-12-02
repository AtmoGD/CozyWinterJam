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
    #region Singleton
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    #region References
    [SerializeField] private Grid grid = null;
    [SerializeField] private Pathfinder pathfinder = null;
    [SerializeField] private CameraController cameraController = null;
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
                SelectedTile.transform.localScale = Vector3.one;

            SelectedTile = grid.GetGridElement(MouseWorldPosition);

            if (SelectedTile)
                SelectedTile.transform.localScale = Vector3.one * 0.9f;
        }
    }
}
