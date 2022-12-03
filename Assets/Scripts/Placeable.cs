using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Building,
    Decoration
}



[CreateAssetMenu(fileName = "New Placeable", menuName = "Placeable")]
public class Placeable : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; } = "New Placeable";
    [field: SerializeField] public Sprite ShopSprite { get; private set; } = null;
    [field: SerializeField] public GameObject Prefab { get; private set; } = null;
    [field: SerializeField] public GameObject PreviewPrefab { get; private set; } = null;
    [field: SerializeField] public int Cost { get; private set; } = 0;
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public ObjectType Type { get; private set; } = ObjectType.Building;
    [field: SerializeField] public float WaitTime { get; private set; } = 1f;
    [field: SerializeField] public Vector2Int MoneyRange { get; private set; } = Vector2Int.one;
}
