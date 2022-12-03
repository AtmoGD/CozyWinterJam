using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyVizualizer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI moneyText = null;
    private void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
    public void SetMoney(int money)
    {
        moneyText.text = money.ToString();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
