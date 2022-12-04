using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Gridsystem;
using Grid = Gridsystem.Grid;
using System;
using UnityEngine.UI;


[Serializable]
public class StartObjectData
{
    public Placeable placeable;
    public int x;
    public int y;
}

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
    [field: SerializeField] public DayTimeController DayTimeController { get; private set; } = null;
    [field: SerializeField] public GameObject ConfirmDeletionPanel { get; private set; } = null;
    [SerializeField] private List<Transform> StartPoints = new List<Transform>();
    public Transform CustomerStart
    {
        get
        {
            return StartPoints[UnityEngine.Random.Range(0, StartPoints.Count)];
        }
    }
    [SerializeField] private List<Transform> EndPoints = new List<Transform>();

    public Transform CustomerEndTile
    {
        get
        {
            return EndPoints[UnityEngine.Random.Range(0, EndPoints.Count)];
        }
    }

    public Transform CustomerEnd { get { return CustomerStart; } }
    [field: SerializeField] public Person CustomerPrefab { get; private set; } = null;
    [field: SerializeField] public List<StartObjectData> StartObjects { get; private set; } = new List<StartObjectData>();
    [field: SerializeField] public Slider CustomerSlider { get; private set; } = null;
    [field: SerializeField] public Slider LoveSlider { get; private set; } = null;
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
    [field: SerializeField] public int Money { get; set; } = 1000;
    public float WorldTimeScale { get; private set; } = 1f;
    public List<PlaceableObject> PlacedObjects { get; private set; } = new List<PlaceableObject>();
    public List<PlaceableObject> PlacedBuildings { get { return PlacedObjects.FindAll(x => x.Data.Type == ObjectType.Building); } }
    public List<PlaceableObject> PlacedDecorations { get { return PlacedObjects.FindAll(x => x.Data.Type == ObjectType.Decoration); } }
    public Tile SelectedTile { get; private set; } = null;
    public Placeable SelectedObject { get; private set; } = null;
    public GameObject PreviewObject { get; private set; } = null;
    public List<Tile> LastSelectedTiles { get; private set; } = new List<Tile>();
    public PlaceableObject EditObject { get; private set; } = null;
    public PlaceableObject DeleteObject { get; private set; } = null;
    #endregion

    #region Data
    [SerializeField] private float customerSpawnChance = 0.2f;
    [SerializeField] private float customerSpawnChanceBuildingAddition = 0.02f;
    public float CustomerSpawnChance
    {
        get
        {
            return customerSpawnChance + (PlacedBuildings.Count * customerSpawnChanceBuildingAddition);
        }
    }

    [field: SerializeField]
    private float CustomerSpawnTime { get; set; } = 1f;
    [field: SerializeField] public int MaxCustomers { get; private set; } = 50;
    [SerializeField] private float moneyDecorationAddition = 1f;
    public float MoneyAddition
    {
        get
        {
            return Mathf.Clamp(PlacedDecorations.Count * moneyDecorationAddition, 0, MaxMoneyAddition);
        }
    }
    [field: SerializeField] public float MaxMoneyAddition { get; private set; } = 100f;
    public bool GameStarted { get; private set; } = false;
    #endregion

    #region Private
    private float customerSpawnTimer = 0f;
    public List<Person> customers = new List<Person>();
    #endregion

    private void Start()
    {
        Grid.DeleteGrid();
        Grid.CreateGrid();

        PlaceStartObject();
    }

    public void StartGame()
    {
        GameStarted = true;
        SpawnCustomer(true);
    }



    public void RegisterCustomer(Person customer)
    {
        customers.Add(customer);
    }

    public void UnregisterCustomer(Person customer)
    {
        customers.Remove(customer);
    }

    public void PauseGame()
    {
        WorldTimeScale = 0f;
    }

    public void UnpauseGame()
    {
        WorldTimeScale = 1f;
    }

    private void PlaceStartObject()
    {
        foreach (StartObjectData startObject in StartObjects)
        {

            Grid.GetGridElement(startObject.x, startObject.y, out Tile tile);

            List<Tile> tiles = Grid.GetGridElements(tile.transform.position, startObject.placeable.Size, false);

            if (tiles.Count == (startObject.placeable.Size.x * startObject.placeable.Size.y))
            {
                if (tiles.TrueForAll(t => t != null && t.currentObject == null))
                {

                    GameObject newObject = Instantiate(startObject.placeable.Prefab, tile.transform.position, Quaternion.identity);

                    PlaceableObject placeableObject = newObject.GetComponent<PlaceableObject>();
                    if (placeableObject)
                    {
                        PlacedObjects.Add(placeableObject);
                        placeableObject.placedOnTiles = tiles;
                    }

                    LayerOrderer layerOrderer = newObject.GetComponent<LayerOrderer>();
                    if (layerOrderer)
                    {
                        layerOrderer.SetGameManager(this);
                        layerOrderer.UpdateOrder(tile);
                    }


                    tiles.ForEach(t => t.currentObject = newObject);

                    foreach (Tile t in tiles)
                    {
                        PathNode tileData = Grid.GridArray.Find(n => n.x == t.x && n.y == t.y);
                        Grid.GridArray.Remove(tileData);

                        tileData.SetIsWalkable(false);
                        Grid.GridArray.Add(tileData);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (!GameStarted)
        {
            return;
        }

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

        UpdateSlider();
    }

    private void UpdateSlider()
    {
        CustomerSlider.value = customers.Count / (float)MaxCustomers;
        LoveSlider.value = MoneyAddition / MaxMoneyAddition;
    }

    public void PlayingState()
    {

    }

    public void SpawnCustomer(bool force = false)
    {
        if (PlacedBuildings.Count == 0 || customers.Count >= MaxCustomers) return;

        customerSpawnTimer += Time.deltaTime;
        if (customerSpawnTimer >= CustomerSpawnTime || force)
        {
            customerSpawnTimer = 0f;
            if (UnityEngine.Random.Range(0f, 1f) <= CustomerSpawnChance || force)
            {
                Person customer = Instantiate(CustomerPrefab, CustomerStart.position, Quaternion.identity);
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
                if (EditObject != null)
                {
                    Tile currentTile = Grid.GetGridElement(MouseWorldPosition);
                    if (currentTile)
                        EditObject.transform.position = currentTile.transform.position;

                    LastSelectedTiles.ForEach(t => t?.Deselect());

                    List<Tile> tiles = Grid.GetGridElements(MouseWorldPosition, EditObject.Data.Size);
                    tiles.ForEach(t => t?.Select());

                    LastSelectedTiles = tiles;
                }
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

    public void ChangeToEditMode()
    {
        if (gameState == GameState.Building && buildState == BuildState.Edit)
        {
            gameState = GameState.Playing;
            buildState = BuildState.New;

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    Grid.GetGridElement(x, y, out Tile tile);
                    tile.Deselect();
                }
            }
        }
        else
        {
            gameState = GameState.Building;
            buildState = BuildState.Edit;

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    Grid.GetGridElement(x, y, out Tile tile);
                    tile.Select();
                }
            }
        }
    }

    public void ChangeToDeleteMode()
    {
        if (gameState == GameState.Building && buildState == BuildState.Delete)
        {
            gameState = GameState.Playing;
            buildState = BuildState.New;

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    Grid.GetGridElement(x, y, out Tile tile);
                    tile.Deselect();
                }
            }
        }
        else
        {
            gameState = GameState.Building;
            buildState = BuildState.Delete;

            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    Grid.GetGridElement(x, y, out Tile tile);
                    tile.Select();
                }
            }
        }
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
        RaycastHit2D hit = Physics2D.Raycast(MouseWorldPosition, Vector2.zero, 10f);

        if (hit)
        {
            ChristmasPresent present = hit.transform.GetComponent<ChristmasPresent>();
            if (present)
            {
                present.Open();
            }
        }
    }

    public void ConfirmDeletion()
    {
        ConfirmDeletionPanel.SetActive(false);

        Tile tile = Grid.GetGridElement(DeleteObject.transform.position);
        if (tile.currentObject != null)
        {
            DeleteObject = tile.currentObject.GetComponent<PlaceableObject>();
            foreach (Tile t in DeleteObject.placedOnTiles)
            {
                PathNode tileData = Grid.GridArray.Find(n => n.x == t.x && n.y == t.y);
                Grid.GridArray.Remove(tileData);

                tileData.SetIsWalkable(true);
                Grid.GridArray.Add(tileData);

                t.currentObject = null;
            }

            PlacedObjects.Remove(DeleteObject);
            Destroy(DeleteObject.gameObject);
        }

        gameState = GameState.Playing;
        buildState = BuildState.New;

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                Grid.GetGridElement(x, y, out Tile currentTile);
                currentTile.Deselect();
            }
        }
    }

    public void DenyDeletion()
    {
        ConfirmDeletionPanel.SetActive(false);
        DeleteObject = null;

        gameState = GameState.Playing;
        buildState = BuildState.New;

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                Grid.GetGridElement(x, y, out Tile tile);
                tile.Deselect();
            }
        }
    }

    public void CheckClickPlayingBuilding()
    {
        switch (buildState)
        {
            case BuildState.New:
                if (SelectedObject != null)
                {
                    List<Tile> tiles = Grid.GetGridElements(MouseWorldPosition, SelectedObject.Size, false);
                    if (tiles.Count == (SelectedObject.Size.x * SelectedObject.Size.y))
                    {
                        if (tiles.TrueForAll(t => t != null && t.currentObject == null))
                        {
                            Money -= SelectedObject.Cost;

                            Tile tile = Grid.GetGridElement(MouseWorldPosition);
                            GameObject newObject = Instantiate(SelectedObject.Prefab, tile.transform.position, Quaternion.identity);

                            PlaceableObject placeableObject = newObject.GetComponent<PlaceableObject>();
                            if (placeableObject)
                            {
                                PlacedObjects.Add(placeableObject);
                                placeableObject.placedOnTiles = tiles;
                            }

                            LayerOrderer layerOrderer = newObject.GetComponent<LayerOrderer>();
                            if (layerOrderer)
                            {
                                layerOrderer.SetGameManager(this);
                                layerOrderer.UpdateOrder(tile);
                            }


                            tiles.ForEach(t => t.currentObject = newObject);

                            foreach (Tile t in tiles)
                            {
                                PathNode tileData = Grid.GridArray.Find(n => n.x == t.x && n.y == t.y);
                                Grid.GridArray.Remove(tileData);

                                tileData.SetIsWalkable(false);
                                Grid.GridArray.Add(tileData);
                            }

                            Destroy(PreviewObject);

                            PreviewObject = null;
                            SelectedObject = null;

                            gameState = GameState.Playing;
                            buildState = BuildState.New;

                            LastSelectedTiles.ForEach(t => t?.Deselect());
                            LastSelectedTiles.Clear();

                            AudioManager.Instance.Play("Place");
                        }
                    }
                }
                break;
            case BuildState.Edit:
                if (EditObject)
                {
                    List<Tile> tiles = Grid.GetGridElements(MouseWorldPosition, EditObject.Data.Size, false);
                    if (tiles.Count == (EditObject.Data.Size.x * EditObject.Data.Size.y))
                    {
                        if (tiles.TrueForAll(t => t != null && t.currentObject == null))
                        {
                            Tile tile = Grid.GetGridElement(MouseWorldPosition);
                            EditObject.transform.position = tile.transform.position;

                            PlaceableObject placeableObject = EditObject.GetComponent<PlaceableObject>();
                            if (placeableObject)
                            {
                                PlacedObjects.Add(placeableObject);
                                placeableObject.placedOnTiles = tiles;
                            }

                            LayerOrderer layerOrderer = EditObject.GetComponent<LayerOrderer>();
                            if (layerOrderer)
                            {
                                layerOrderer.SetGameManager(this);
                                layerOrderer.UpdateOrder(tile);
                            }

                            tiles.ForEach(t =>
                            {
                                t?.Deselect();
                                t.currentObject = EditObject.gameObject;
                            });

                            foreach (Tile t in tiles)
                            {
                                PathNode tileData = Grid.GridArray.Find(n => n.x == t.x && n.y == t.y);
                                Grid.GridArray.Remove(tileData);

                                tileData.SetIsWalkable(false);
                                Grid.GridArray.Add(tileData);
                            }

                            EditObject = null;
                            gameState = GameState.Playing;
                            buildState = BuildState.New;

                            AudioManager.Instance.Play("Place");
                        }
                    }
                }
                else
                {
                    Tile tile = Grid.GetGridElement(MouseWorldPosition);
                    if (tile.currentObject != null)
                    {
                        EditObject = tile.currentObject.GetComponent<PlaceableObject>();
                        foreach (Tile t in EditObject.placedOnTiles)
                        {
                            PathNode tileData = Grid.GridArray.Find(n => n.x == t.x && n.y == t.y);
                            Grid.GridArray.Remove(tileData);

                            tileData.SetIsWalkable(true);
                            Grid.GridArray.Add(tileData);

                            t.currentObject = null;
                        }

                        for (int x = 0; x < Grid.Width; x++)
                        {
                            for (int y = 0; y < Grid.Height; y++)
                            {
                                Grid.GetGridElement(x, y, out Tile currentTile);
                                currentTile.Deselect();
                            }
                        }

                        AudioManager.Instance.Play("Click");
                    }
                }
                break;
            case BuildState.Delete:
                if (ConfirmDeletionPanel.activeSelf == true) return;

                Tile deleteTile = Grid.GetGridElement(MouseWorldPosition);
                if (deleteTile.currentObject != null)
                {
                    DeleteObject = deleteTile.currentObject.GetComponent<PlaceableObject>();
                    ConfirmDeletionPanel.SetActive(true);
                }

                AudioManager.Instance.Play("Click");
                break;
        }
    }
}