using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float cameraMaxSpeed = 10f;
    [SerializeField] private float cameraAcceleration = 10f;
    [SerializeField] private float borderThickness = 10f;
    [SerializeField] private float borderDeadZone = 2f;

    private Vector2 dir = Vector2.zero;
    private Vector2 targetDir = Vector2.zero;
    private float currentSpeed = 0f;
    private float border = 10f;
    private float borderDead = 2f;

    private void Start()
    {
        border = (Screen.height + Screen.width) / 2 * borderThickness / 100;
        borderDead = (Screen.height + Screen.width) / 2 * borderDeadZone / 100;
    }

    private void FixedUpdate()
    {
        dir = Vector2.Lerp(dir, targetDir, Time.deltaTime * cameraAcceleration);

        mainCamera.transform.Translate(dir * cameraMaxSpeed * Time.deltaTime);

        targetDir = Vector2.zero;
    }

    public void MoveCamera(Vector2 _mousePosition)
    {
        if (_mousePosition.x >= Screen.width - border && _mousePosition.x <= Screen.width - borderDead)
            targetDir += Vector2.right;

        if (_mousePosition.x <= border && _mousePosition.x >= borderDead)
            targetDir += Vector2.left;

        if (_mousePosition.y >= Screen.height - border && _mousePosition.y <= Screen.height - borderDead)
            targetDir += Vector2.up;

        if (_mousePosition.y <= border && _mousePosition.y >= borderDead)
            targetDir += Vector2.down;

        if (dir != Vector2.zero && dir.magnitude > 1)
            targetDir.Normalize();
    }
}
