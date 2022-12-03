using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private MoneyVizualizer moneyPrefab = null;
    public GameManager Manager { get; private set; } = null;
    public Animator Animator { get; private set; } = null;
    public LayerOrderer LayerOrderer { get; private set; } = null;

    private List<PlaceableObject> shopsToVisit = new List<PlaceableObject>();
    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int currentTargetTile = Vector2Int.zero;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float timeToWaitAtShop = 1f;
    [SerializeField] private float visitShopChance = 0.4f;

    bool goToEndTile = false;
    bool leaving = false;
    bool moneySpend = false;

    private float currentWaitTime = 0f;

    private void Start()
    {
        Manager = GameManager.Instance;
        Animator = GetComponent<Animator>();
        LayerOrderer = GetComponent<LayerOrderer>();
        if (LayerOrderer)
        {
            LayerOrderer.SetGameManager(Manager);
        }

        if (Manager)
        {
            foreach (PlaceableObject placeableObject in Manager.PlacedBuildings)
            {
                if (Random.Range(0f, 1f) < visitShopChance)
                {
                    shopsToVisit.Add(placeableObject);
                }
            }

            // Shuffle the list
            for (int i = 0; i < shopsToVisit.Count; i++)
            {
                PlaceableObject temp = shopsToVisit[i];
                int randomIndex = Random.Range(i, shopsToVisit.Count);
                shopsToVisit[i] = shopsToVisit[randomIndex];
                shopsToVisit[randomIndex] = temp;
            }
        }

        GetNewPath();
    }

    private void Update()
    {
        if (currentWaitTime > 0)
        {
            if (!moneySpend)
            {
                int moneyMin = shopsToVisit[0].Data.MoneyRange.x;
                int moneyMax = shopsToVisit[0].Data.MoneyRange.y;
                int money = Random.Range(moneyMin, moneyMax);

                MoneyVizualizer moneyVizualizer = Instantiate(moneyPrefab, transform.position, Quaternion.identity);
                moneyVizualizer.SetMoney(money);

                Manager.Money += money;
                moneySpend = true;
            }

            currentWaitTime -= Time.deltaTime;
            if (currentWaitTime <= 0)
            {
                GetNewPath();
                moneySpend = false;
            }

            return;
        }

        MoveAlongPath();
    }

    private void GetNewPath()
    {
        if (Manager.PlacedBuildings.Count > 0)
        {
            if (shopsToVisit.Count > 0)
            {
                Gridsystem.Tile startTile = Manager.Grid.GetGridElement(transform.position);

                PlaceableObject target = shopsToVisit[0];

                Transform customerPosition = target.GetCustomerPosition();
                if (customerPosition)
                {
                    Gridsystem.Tile endTile = Manager.Grid.GetGridElement(customerPosition.position);

                    Vector2Int start = new Vector2Int(startTile.x, startTile.y);
                    Vector2Int end = new Vector2Int(endTile.x, endTile.y);

                    path = Manager.Pathfinder.FindPath(Manager.Grid, start, end);

                    if (path.Count > 0)
                        currentTargetTile = path[path.Count - 1];
                }

                shopsToVisit.Remove(target);
            }
            else
            {
                Gridsystem.Tile startTile = Manager.Grid.GetGridElement(transform.position);
                Gridsystem.Tile endTile = Manager.Grid.GetGridElement(Manager.CustomerEndTile.position);

                Vector2Int start = new Vector2Int(startTile.x, startTile.y);
                Vector2Int end = new Vector2Int(endTile.x, endTile.y);

                path = Manager.Pathfinder.FindPath(Manager.Grid, start, end);

                if (path.Count > 0)
                    currentTargetTile = path[path.Count - 1];
            }
        }
    }

    private void MoveAlongPath()
    {
        Vector3 startPosition = transform.position;

        if (leaving)
        {
            transform.position = Vector3.MoveTowards(transform.position, Manager.CustomerEnd.position, speed * Time.deltaTime);

            if (transform.position == Manager.CustomerEnd.position)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            Manager.Grid.GetGridElement(currentTargetTile.x, currentTargetTile.y, out Gridsystem.Tile targetTile);

            if (transform.position != targetTile.transform.position)
            {
                Vector3 targetPosition = targetTile.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
            else
            {
                if (path.Count > 0)
                    path.RemoveAt(path.Count - 1);

                if (path.Count > 0)
                {
                    currentTargetTile = path[path.Count - 1];
                }
                else
                {
                    if (shopsToVisit.Count > 0)
                    {
                        currentWaitTime = timeToWaitAtShop;
                    }
                    else
                    {
                        if (goToEndTile)
                        {
                            leaving = true;
                        }
                        else
                        {
                            Gridsystem.Tile endTile = Manager.Grid.GetGridElement(Manager.CustomerEndTile.position);
                            Gridsystem.Tile startTile = Manager.Grid.GetGridElement(transform.position);

                            Vector2Int start = new Vector2Int(startTile.x, startTile.y);
                            Vector2Int end = new Vector2Int(endTile.x, endTile.y);

                            path = Manager.Pathfinder.FindPath(Manager.Grid, start, end);

                            if (path.Count > 0)
                                currentTargetTile = path[path.Count - 1];

                            goToEndTile = true;
                        }
                    }
                }
            }
            LayerOrderer.UpdateOrder(targetTile);
        }

        Vector3 direction = (transform.position - startPosition).normalized;

        Animator.SetFloat("xDir", direction.x);
        Animator.SetFloat("yDir", direction.y);

    }


    private void OnDrawGizmos()
    {
        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;
                }

                Manager.Grid.GetGridElement(path[i].x, path[i].y, out Gridsystem.Tile tile);
                Gizmos.DrawCube(tile.transform.position, Vector3.one * 0.5f);
            }
        }
    }
}

