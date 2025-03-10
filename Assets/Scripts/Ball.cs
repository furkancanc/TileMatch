using PathCreation;
using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float upscaleCounter;
    private float downscaleCounter = 1;
    public BallState state;
    public BallType type;
    private Vector3 shootDirection;

    [Header("Elements")]
    private Board board;
    private PathCreator pathCreator;
    public BallSlot slot;

    private float distanceTraveled;

    [Header("Data")]
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;


    [Header("Actions")]
    public static Action<BallSlot, Ball> onBallCollided;
    private void Start()
    {
        board = FindFirstObjectByType<Board>();
        pathCreator = FindFirstObjectByType<PathCreator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.enabled = false;
    }

    private void Update()
    {

        switch (state)
        {
            case BallState.SpawningOnTrack:
                upscaleCounter += GameProperties.ballUpscaleSpeed * Time.deltaTime;
                if (upscaleCounter >= 1)
                {
                    state = BallState.InSlot;
                    return;
                }

                transform.localScale = Vector3.one * upscaleCounter;
                transform.up = slot.transform.up;
                break;
            case BallState.SpawningToShoot:
                upscaleCounter += GameProperties.GetSlotSpeedMultiplier(1f) * GameProperties.ballUpscaleSpeed * Time.deltaTime;
                if (upscaleCounter >= 1)
                {
                    state = BallState.ReadyToShoot;
                    return;
                }

                transform.localScale = Vector3.one * upscaleCounter;
                break;
            case BallState.InSlot:
                transform.up = slot.transform.up;
                break;
            case BallState.Destroying:
                float multiplier = downscaleCounter > .9f ? .3f : 1;
                downscaleCounter -= multiplier * GameProperties.GetSlotSpeedMultiplier(1f) 
                    * GameProperties.ballDownslaceSpeed * Time.deltaTime;

                if (downscaleCounter < 0 || board.isReverse && IsNearPathStart())
                {
                    Destroy(gameObject);
                    return;
                }
                transform.localScale = Vector3.one * downscaleCounter;
                break;
            case BallState.Shooting:
                transform.position += shootDirection * (GameProperties.GetSlotSpeedMultiplier(1f) * GameProperties.ballShootingSpeed * Time.deltaTime);
                break;
            case BallState.Landing:
                transform.position =
                    Vector3.MoveTowards(transform.position, slot.transform.position, GameProperties.ballLandingSpeed * GameProperties.GetSlotSpeedMultiplier(1f) * Time.deltaTime);

                if (Vector3.Distance(transform.position, slot.transform.position) < .1f)
                {
                    state = BallState.InSlot;
                    transform.rotation = Quaternion.identity;
                    PlaceInSlotTransform();
                }
                break;
            case BallState.SwitchingSlots:
                int direction = distanceTraveled > slot.GetDistanceTraveled() ? -1 : 1;
                distanceTraveled += direction * GameProperties.GetSlotSpeedMultiplier(1f) * GameProperties.ballSlotSwitchingSpeed * Time.deltaTime;

                transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);

                if (Mathf.Abs(distanceTraveled - slot.GetDistanceTraveled()) < .1f)
                {
                    state = BallState.InSlot;
                    PlaceInSlotTransform();
                }
                break;
            case BallState.ReadyToShoot:
                break;
        }
    }

    private bool IsNearPathStart()
    {
        return pathCreator.path.GetClosestDistanceAlongPath(transform.position) < .2f;
    }
    private void PlaceInSlotTransform()
    {
        transform.position = slot.transform.position;
        transform.parent = slot.transform;
    }

    public void Shoot(Vector3 direction)
    {
        shootDirection = direction;
        state = BallState.Shooting;
        circleCollider2D.enabled = true;
    }

    public void Land()
    {
        state = BallState.Landing;
    }

    public void MoveToSlot()
    {
        state = BallState.SwitchingSlots;
        distanceTraveled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

    public void StartDestroying()
    {
        state = BallState.Destroying;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BallSlot ballSlot = collision.GetComponent<BallSlot>();
        if (!ballSlot || !ballSlot.ball || state != BallState.Shooting)
        {
            return;
        }

        onBallCollided?.Invoke(ballSlot, this);
        circleCollider2D.enabled = false;
    }

    public void UpdateSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }
}
