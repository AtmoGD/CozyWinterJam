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
    public int Money { get; private set; } = 100;
    public float WorldTimeScale { get; private set; } = 1f;
    public Tile SelectedTile { get; private set; } = null;
    public Placeable SelectedObject { get; private set; } = null;
    public GameObject PreviewObject { get; private set; } = null;
    public List<Tile> LastSelectedTiles { get; private set; } = new List<Tile>();
    #endregion

    private void Start()
    {
        grid.DeleteGrid();
        grid.CreateGrid();
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Playing:
                PlayingState();
                break;
            case GameState.Building:
                BuildingState();
                break;
            case GameState.Paused:
                break;
            default:
                break;
        }

        if (!uiController.IsBuildingUIActive)
            cameraController.MoveCamera(MousePosition);
    }

    public void PlayingState()
    {

    }

    public void BuildingState()
    {
        switch (buildState)
        {
            case BuildState.New:
                if (PreviewObject != null)
                {
                    Tile currentTile = grid.GetGridElement(MouseWorldPosition);
                    if (currentTile)
                        PreviewObject.transform.position = currentTile.transform.position;

                    LastSelectedTiles.ForEach(t => t?.Deselect());

                    List<Tile> tiles = grid.GetGridElements(MouseWorldPosition, SelectedObject.Size);
                    tiles.ForEach(t => t?.Select());

                    LastSelectedTiles = tiles;
                }
                break;
            case BuildState.Edit:
                break;
            case BuildState.Delete:
                break;
            default:
                break;
        }
    }

    public void StartPlacingObject(Placeable gameObject)
    {
        SelectedObject = gameObject;
        buildState = BuildState.New;
        gameState = GameState.Building;

        PreviewObject = Instantiate(SelectedObject.PreviewPrefab, MouseWorldPosition, Quaternion.identity);
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
            switch (gameState)
            {
                case GameState.Playing:
                    CheckClickPlaying();
                    break;
                case GameState.Building:
                    CheckClickPlayingBuilding();
                    break;
            }
        }
    }

    public void CheckClickPlaying()
    {
        if (SelectedTile)
            SelectedTile.Deselect();

        SelectedTile = grid.GetGridElement(MouseWorldPosition);

        if (SelectedTile)
            SelectedTile.Select();
    }

    public void CheckClickPlayingBuilding()
    {
        switch (buildState)
        {
            case BuildState.New:
                if (SelectedObject != null)
                {
                    List<Tile> tiles = grid.GetGridElements(MouseWorldPosition, SelectedObject.Size);
                    if (tiles.TrueForAll(t => t != null && t.currentObject == null))
                    {
                        Money -= SelectedObject.Cost;

                        Tile tile = grid.GetGridElement(MouseWorldPosition);
                        GameObject newObject = Instantiate(SelectedObject.Prefab, tile.transform.position, Quaternion.identity);

                        tiles.ForEach(t => t.currentObject = newObject);

                        Destroy(PreviewObject);
                        PreviewObject = null;
                        SelectedObject = null;
                        gameState = GameState.Playing;
                        buildState = BuildState.New;

                        LastSelectedTiles.ForEach(t => t?.Deselect());
                        LastSelectedTiles.Clear();
                    }
                }
                break;
            case BuildState.Edit:
                break;
            case BuildState.Delete:
                break;
        }
    }
}