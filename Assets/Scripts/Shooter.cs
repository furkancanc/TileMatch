using UnityEngine;

public class Shooter : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        FaceMouse();
    }

    private void FaceMouse()
    {
        Vector3 mousePosition = GetMousePosition();
        Vector3 direction = mousePosition - transform.position;

        transform.up = new Vector2(direction.x, direction.y);
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        return mousePosition;
    }
}
