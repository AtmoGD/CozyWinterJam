using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [field: SerializeField] public Placeable Data { get; private set; } = null;
    [field: SerializeField] public List<Transform> CustomerPositions { get; private set; } = new List<Transform>();
    public List<Gridsystem.Tile> placedOnTiles { get; set; } = new List<Gridsystem.Tile>();

    public Transform GetCustomerPosition()
    {
        return CustomerPositions[Random.Range(0, CustomerPositions.Count)];
    }
}
