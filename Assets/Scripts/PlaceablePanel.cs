using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceablePanel : MonoBehaviour
{
    [field: SerializeField] public Placeable Placeable { get; private set; } = null;
    [field: SerializeField] public UIController UIController { get; private set; } = null;
    [field: SerializeField] public Image Image { get; private set; } = null;
    [field: SerializeField] public TMP_Text NameText { get; private set; } = null;
    [field: SerializeField] public TMP_Text CostText { get; private set; } = null;

    private void Start()
    {
        Image.sprite = Placeable.ShopSprite;
        NameText.text = Placeable.Name;
        CostText.text = Placeable.Cost.ToString();
    }

    public void PlaceObject()
    {
        UIController.PlaceObject(Placeable);
    }
}
