using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasPresent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private MoneyVizualizer moneyPrefab = null;
    [SerializeField] private Vector2Int moneyRange = new Vector2Int(1, 5);

    private void Start()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
    }

    public void Open()
    {
        int money = Random.Range(moneyRange.x, moneyRange.y);
        MoneyVizualizer moneyVizualizer = Instantiate(moneyPrefab, transform.position, Quaternion.identity);
        moneyVizualizer.SetMoney(money);
        GameManager.Instance.Money += money;
        AudioManager.Instance.Play("Money");
        AudioManager.Instance.Play("Click");
        Destroy(gameObject);
    }
}
