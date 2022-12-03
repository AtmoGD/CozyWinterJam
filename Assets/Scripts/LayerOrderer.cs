using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOrderer : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    private GameManager gameManager;

    public void UpdateOrder(Gridsystem.Tile tile)
    {
        int order = gameManager.Grid.Height * 10;
        order -= tile.y * 10;

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].sortingOrder = order + i;
        }
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }
}
