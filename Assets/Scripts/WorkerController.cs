using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    private void Start()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
    }
}
