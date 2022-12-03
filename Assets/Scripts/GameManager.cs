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
    [field: SerializeField] public Grid Grid { get; private set; } = null;
    [field: SerializeField] public Pathfinder Pathfinder { get; private set; } = null;
    [field: SerializeField] public CameraController CameraController { get; private set; } = null;
    [field: SerializeField] public UIController UIController { get; private set; } = null;
    [field: SerializeField] public Transform CustomerStart { get; private set; } = null;
    [field: SerializeField] public Transform CustomerEndTile { get; private set; } = null;
    [field: SerializeField] public Transform CustomerEnd { get; private set; } = null;
    [field: SerializeField] public Person CustomerPrefab { get; private set; } = null;
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
    public int Money { get; private set; } = 1000;
    public float WorldTimeScale { get; private set; } = 1f;
    public List<PlaceableObject> PlacedObjects { get; private set; } = new List<PlaceableObject>();
    public Tile SelectedTile { get; private set; } = null;
    public Placeable SelectedObject { get; private set; } = null;
    public GameObject PreviewObject { get; private set; } = null;
    public List<Tile> LastSelectedTiles { get; private set; } = new List<Tile>();
    #endregion

    #region Data
    [field: SerializeField] public float CustomerSpawnChance { get; private set; } = 0.1f;
    [field: SerializeField] private float CustomerSpawnTime { get; set; } = 1f;
    [field: SerializeField] private float MoneyMultiplier { get; set; } = 1f;
    #endregion

    #region Private
    private float customerSpawnTimer = 0f;
    private List<Person> customers = new List<Person>();
    #endregion

    private void Start()
    {
        Grid.DeleteGrid();
        Grid.CreateGrid();
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
                return;
            default:
                break;
        }

        if (!UIController.IsBuildingUIActive)
            CameraController.MoveCamera(MousePosition);

        SpawnCustomer();
    }

    public void PlayingState()
    {

    }

    public void SpawnCustomer()
    {
        if (PlacedObjects.Count == 0) return;

        customerSpawnTimer += Time.deltaTime;
        if (customerSpawnTimer >= CustomerSpawnTime)
        {
            customerSpawnTimer = 0f;
            if (Random.Range(0f, 1f) <= CustomerSpawnChance)
            {
                Person customer = Instantiate(CustomerPrefab, CustomerStart.position, Quaternion.identity);
                customers.Add(customer);
            }
        }
    }

    public void BuildingState()
    {
        switch (buildState)
        {
            case BuildState.New:
                if (PreviewObject != null)
                {
                    Tile currentTile = Grid.GetGridElement(MouseWorldPosition);
                    if (currentTile)
                        PreviewObject.transform.position = currentTile.transform.position;

                    LastSelectedTiles.ForEach(t => t?.Deselect());

                    List<Tile> tiles = Grid.GetGridElements(MouseWorldPosition, SelectedObject.Size);
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

        SelectedTile = Grid.GetGridElement(MouseWorldPosition);

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
                    List<Tile> tiles = Grid.GetGridElements(MouseWorldPosition, SelectedObject.Size);
                    if (tiles.TrueForAll(t => t != null && t.currentObject == null))
                    {
                        Money -= SelectedObject.Cost;

                        Tile tile = Grid.GetGridElement(MouseWorldPosition);
                        GameObject newObject = Instantiate(SelectedObject.Prefab, tile.transform.position, Quaternion.identity);

                        PlaceableObject placeableObject = newObject.GetComponent<PlaceableObject>();
                        if (placeableObject)
                            PlacedObjects.Add(placeableObject);

                        tiles.ForEach(t => { t.currentObject = newObject; Debug.Log(t.currentObject); });
                        Grid.UpdateGridArray();

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