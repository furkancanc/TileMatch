using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private BallFactory ballFactory;
    [SerializeField] private Board board;

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
            nextShootBall = ballFactory.CreateRandomBallAt(transform.position);
        }

        if (Input.GetMouseButtonDown(0) && !board.isDestroyingMatchingBalls && !board.isReverse)
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
        var mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height)
            return new Vector3();

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        mousePosition.z = 0;

        return mousePosition;
    }
}
