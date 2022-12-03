using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public GameManager Manager { get; private set; } = null;
    private List<PlaceableObject> shopsToVisit = new List<PlaceableObject>();
    private PlaceableObject currentTarget = null;

    private List<Vector2Int> path = new List<Vector2Int>();
    private Vector2Int currentTargetTile = Vector2Int.zero;

    [SerializeField] private float speed = 3f;

    private void Start()
    {
        Manager = GameManager.Instance;
    }

    private void Update()
    {
        if (path.Count > 0)
        {
            MoveAlongPath();
        }
        else
        {
            GetNewPath();
        }
    }

    private void GetNewPath()
    {
        if (Manager.PlacedObjects.Count > 0)
        {
            Gridsystem.Tile startTile = Manager.Grid.GetGridElement(transform.position);
            int randomIndex = Random.Range(0, Manager.PlacedObjects.Count);
            PlaceableObject target = Manager.PlacedObjects[randomIndex];
            Gridsystem.Tile endTile = Manager.Grid.GetGridElement(target.CustomerPosition.position);

            Vector2Int start = new Vector2Int(startTile.x, startTile.y);
            Vector2Int end = new Vector2Int(endTile.x, endTile.y);

            path = Manager.Pathfinder.FindPath(Manager.Grid, start, end);

            if (path.Count > 0)
                currentTargetTile = path[path.Count - 1];
        }
    }

    private void MoveAlongPath()
    {
        Manager.Grid.GetGridElement(currentTargetTile.x, currentTargetTile.y, out Gridsystem.Tile targetTile);

        if (transform.position != targetTile.transform.position)
        {
            Vector3 targetPosition = targetTile.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            path.RemoveAt(path.Count - 1);
            if (path.Count > 0)
            {
                currentTargetTile = path[path.Count - 1];
            }
        }
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

