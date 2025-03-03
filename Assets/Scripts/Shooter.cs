using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private BallFactory ballFactory;
    [SerializeField] private Transform shootPoint;

    [Header("Data")]
    private Camera mainCamera;
    public Ball nextShootBall;

    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    private SpriteRenderer spriteRenderer;

    public bool isShooterDisabledFromOutside;

    private void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        FaceMouse();
        UpdateSprite();

        if (!nextShootBall)
        {
            nextShootBall = ballFactory.CreateRandomBallAt(shootPoint.position);
            nextShootBall.state = BallState.SpawningToShoot;
            nextShootBall.transform.parent = shootPoint;
        }

        if (Input.GetMouseButtonDown(0) && !isShooterDisabledFromOutside)
        {
            ShootNextBall();
        }
    }

    public void UpdateSprite()
    {
        spriteRenderer.sprite = !isShooterDisabledFromOutside && IsNextBallReady ? activeSprite : inactiveSprite;
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

    private bool IsNextBallReady => nextShootBall && nextShootBall.state == BallState.ReadyToShoot;
}
