using PathCreation;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float upscaleCounter;
    private float downscaleCounter = 1;
    public BallState state;
    public BallType type;
    private Vector3 shootDirection;

    public BallSlot slot;

    private Board board;

    private CircleCollider2D CircleCollider2D;

    private void Start()
    {
        board = FindFirstObjectByType<Board>();
        CircleCollider2D = GetComponent<CircleCollider2D>();

        CircleCollider2D.enabled = false;
    }

    private void Update()
    {

        switch (state)
        {
            case BallState.Spawning:
                upscaleCounter += GameProperties.ballUpscaleSpeed * Time.deltaTime;
                if (upscaleCounter >= 1)
                {
                    state = BallState.InSlot;
                    return;
                }

                transform.localScale = Vector3.one * upscaleCounter;
                break;
            case BallState.InSlot:
                break;
            case BallState.Destroying:
                downscaleCounter -= GameProperties.ballDownslaceSpeed * Time.deltaTime;
                if (downscaleCounter < 0)
                {
                    Destroy(gameObject);
                    return;
                }
                transform.localScale = Vector3.one * downscaleCounter;
                break;
            case BallState.Shooting:
                transform.position += shootDirection * (GameProperties.ballShootingSpeed * Time.deltaTime);
                break;
            case BallState.Landing:
                transform.position = 
                    Vector3.MoveTowards(transform.position, slot.transform.position, GameProperties.ballLandingSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, slot.transform.position) < .1f)
                {
                    state = BallState.InSlot;
                    transform.position = slot.transform.position;
                    transform.parent = slot.transform;
                }
                break;
            case BallState.SwitchingSlots:
                transform.position =
                    Vector3.MoveTowards(transform.position, slot.transform.position, GameProperties.ballLandingSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, slot.transform.position) < .1f)
                {
                    state = BallState.InSlot;
                    transform.position = slot.transform.position;
                    transform.parent = slot.transform;
                }
                break;
        }
    }

    public void Shoot(Vector3 direction)
    {
        shootDirection = direction;
        state = BallState.Shooting;
        CircleCollider2D.enabled = true;
    }

    public void Land()
    {
        state = BallState.Landing;
    }

    public void MoveToSlot()
    {
        state = BallState.SwitchingSlots;
    }

    public void StartDestroying()
    {
        state = BallState.Destroying;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<BallSlot>())
        {
            return;
        }

        BallSlot ballSlot = collision.GetComponent<BallSlot>();

        if (!ballSlot.ball || state != BallState.Shooting)
        {
            return;
        }

        Debug.Log("Boo!");
        board.LandBall(ballSlot, this);
        CircleCollider2D.enabled = false;
    }
}
