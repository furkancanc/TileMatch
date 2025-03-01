using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private BallFactory ballFactory;

    [Header("Data")]
    private Camera mainCamera;
    public Ball nextShootBall;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        FaceMouse();

        if (!nextShootBall)
        {
            nextShootBall = ballFactory.CreateBallAt(transform.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 shootDirection = (GetMousePosition() - transform.position).normalized;
            nextShootBall.Shoot(shootDirection);
            nextShootBall = null;
        }
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
