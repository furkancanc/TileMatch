using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private BallFactory ballFactory;
    [SerializeField] private Board board;
    [SerializeField] private Transform shootPoint;

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
            nextShootBall = ballFactory.CreateRandomBallAt(shootPoint.position);
            nextShootBall.transform.parent = shootPoint;
        }

        if (Input.GetMouseButtonDown(0) && !board.isDestroyingMatchingBalls && !board.isReverse)
        {
            ShootNextBall();
        }
    }

    private void ShootNextBall()
    {
        Vector3 shootDirection = (GetMousePosition() - transform.position).normalized;
        nextShootBall.Shoot(shootDirection);
        nextShootBall.transform.parent = null;
        nextShootBall = null;
    }

    private void FaceMouse()
    {
        Vector3 mousePosition = GetMousePosition();
        Vector3 direction = mousePosition - transform.position;

        transform.up = new Vector2(direction.x, direction.y);
    }

    private Vector3 GetMousePosition()
    {
        var mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height)
            return new Vector3();

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        mousePosition.z = 0;

        return mousePosition;
    }
}
