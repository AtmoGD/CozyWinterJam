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
    [field: SerializeField] public Color BaseColor { get; private set; } = Color.white;
    [field: SerializeField] public Color NotEnoughMoneyColor { get; private set; } = Color.red;

    private void Start()
    {
        Image.sprite = Placeable.ShopSprite;
        NameText.text = Placeable.Name;
        CostText.text = Placeable.Cost.ToString();
    }

    private void OnEnable()
    {
        if (UIController.Manager.Money < Placeable.Cost)
        {
            CostText.color = NotEnoughMoneyColor;
        }
        else
        {
            CostText.color = BaseColor;
        }
    }

    public void PlaceObject()
    {
        UIController.PlaceObject(Placeable);
    }
}
