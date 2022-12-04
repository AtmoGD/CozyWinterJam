using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float cameraMaxSpeed = 10f;
    [SerializeField] private float cameraAcceleration = 10f;
    [SerializeField] private float borderThickness = 10f;
    [SerializeField] private bool hasDeadZone = false;
    [SerializeField] private float borderDeadZone = 2f;
    [SerializeField] private Vector2 cameraBoundsMin = new Vector2(0f, 0f);
    [SerializeField] private Vector2 cameraBoundsMax = new Vector2(100f, 100f);

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
        MoveCamera();

        ClampCameraBetweenBounds();
    }

    private void MoveCamera()
    {
        dir = Vector2.Lerp(dir, targetDir, Time.deltaTime * cameraAcceleration);
        mainCamera.transform.Translate(dir * cameraMaxSpeed * Time.deltaTime);
        targetDir = Vector2.zero;
    }

    private void CalculateCameraSize(out Vector2 cameraSize)
    {
        Vector2 cornerVector = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        Vector2 centerVector = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        cameraSize = (cornerVector - centerVector) / 2;
    }

    private void ClampCameraBetweenBounds()
    {
        CalculateCameraSize(out Vector2 cameraSize);

        Vector3 pos = mainCamera.transform.position;
        pos.x = Mathf.Clamp(pos.x, cameraBoundsMin.x + cameraSize.x, cameraBoundsMax.x - cameraSize.x);
        pos.y = Mathf.Clamp(pos.y, cameraBoundsMin.y + cameraSize.y, cameraBoundsMax.y - cameraSize.y);
        mainCamera.transform.position = pos;
    }

    public void MoveCamera(Vector2 _mousePosition)
    {
        if (_mousePosition.x >= Screen.width - border && (hasDeadZone ? _mousePosition.x <= Screen.width - borderDead : true))
            targetDir += Vector2.right;

        if (_mousePosition.x <= border && (hasDeadZone ? _mousePosition.x >= borderDead : true))
            targetDir += Vector2.left;

        if (_mousePosition.y >= Screen.height - border && (hasDeadZone ? _mousePosition.y <= Screen.height - borderDead : true))
            targetDir += Vector2.up;

        if (_mousePosition.y <= border && (hasDeadZone ? _mousePosition.y >= borderDead : true))
            targetDir += Vector2.down;

        if (dir != Vector2.zero && dir.magnitude > 1)
            targetDir.Normalize();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(cameraBoundsMin.x + (cameraBoundsMax.x - cameraBoundsMin.x) / 2, cameraBoundsMin.y + (cameraBoundsMax.y - cameraBoundsMin.y) / 2, 0f), new Vector3(cameraBoundsMax.x - cameraBoundsMin.x, cameraBoundsMax.y - cameraBoundsMin.y, 0f));
    }
}
