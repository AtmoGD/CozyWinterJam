using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [field: SerializeField] public Placeable Data { get; private set; } = null;
    [field: SerializeField] public Transform CustomerPosition { get; private set; } = null;
}
