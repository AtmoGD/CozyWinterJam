using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private Animator emoticonsAnimator = null;
    [SerializeField] private MoneyVizualizer moneyPrefab = null;
    [SerializeField] private AudioSource moneySound = null;
    [SerializeField] private List<RuntimeAnimatorController> animatorControllers = new List<RuntimeAnimatorController>();
    public GameManager Manager { get; private set; } = null;
    public Animator Animator { get; private set; } = null;
    public LayerOrderer LayerOrderer { get; private set; } = null;

    private List<PlaceableObject> shopsToVisit = new List<PlaceableObject>();
    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int currentTargetTile = Vector2Int.zero;
    [SerializeField] private float speed = 1f;
    [SerializeField]
    private float Speed
    {
        get
        {
            return speed * Manager.WorldTimeScale;
        }
    }
    [SerializeField] private float timeToWaitAtShop = 1f;
    [SerializeField] private float visitShopChance = 0.4f;

    bool goToEndTile = false;
    bool leaving = false;
    bool moneySpend = false;

    private float currentWaitTime = 0f;

    private void Start()
    {
        Manager = GameManager.Instance;
        Manager.RegisterCustomer(this);

        Animator = GetComponent<Animator>();
        Animator.runtimeAnimatorController = animatorControllers[Random.Range(0, animatorControllers.Count)];
        LayerOrderer = GetComponent<LayerOrderer>();
        if (LayerOrderer)
        {
            LayerOrderer.SetGameManager(Manager);
        }

        ChooseRandomShopList();

        GetNewPath();

        if (path.Count <= 0)
        {
            print("No path found on stat shopping list. Gonna destroy myself");
            Die();
        }
    }

    public void Die()
    {
        Manager.UnregisterCustomer(this);
        Destroy(gameObject);
    }

    private void ChooseRandomShopList()
    {
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
                money = Mathf.RoundToInt(Manager.MoneyAddition + money);

                moneySound.Play();

                MoneyVizualizer moneyVizualizer = Instantiate(moneyPrefab, transform.position, Quaternion.identity);
                moneyVizualizer.SetMoney(money);

                Manager.Money += money;
                moneySpend = true;

                if (emoticonsAnimator)
                {
                    emoticonsAnimator.SetTrigger("Hearth");
                }
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
                    else
                        print("ERROR01: No path found");
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
                else
                    print("ERROR02: No path found");
            }
        }
    }

    private void MoveAlongPath()
    {
        Vector3 startPosition = transform.position;

        if (leaving)
        {
            transform.position = Vector3.MoveTowards(transform.position, Manager.CustomerEnd.position, Speed * Time.deltaTime);

            if (transform.position == Manager.CustomerEnd.position)
            {
                Die();
                return;
            }
        }
        else
        {
            Manager.Grid.GetGridElement(currentTargetTile.x, currentTargetTile.y, out Gridsystem.Tile targetTile);

            if (transform.position != targetTile.transform.position)
            {
                Vector3 targetPosition = targetTile.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
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
                            else
                                print("ERROR03: No path found");

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
        Animator.SetFloat("Speed", Mathf.Clamp01(Speed));

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

