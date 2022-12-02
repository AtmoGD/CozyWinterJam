using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float cameraSpeed = 10f;
    [SerializeField] private float borderThickness = 10f;

    private Vector2 dir = Vector2.zero;
    private float border = 10f;

    private void Start()
    {
        border = (Screen.height + Screen.width) / 2 * borderThickness / 100;
    }

    private void FixedUpdate()
    {
        if (dir != Vector2.zero)
            mainCamera.transform.Translate(dir * cameraSpeed * Time.deltaTime);
    }

    public void MoveCamera(Vector2 _mousePosition)
    {
        dir = Vector2.zero;

        if (_mousePosition.x >= Screen.width - border)
            dir += Vector2.right;

        if (_mousePosition.x <= border)
            dir += Vector2.left;

        if (_mousePosition.y >= Screen.height - border)
            dir += Vector2.up;

        if (_mousePosition.y <= border)
            dir += Vector2.down;

        if (dir != Vector2.zero)
            dir.Normalize();
    }
}
