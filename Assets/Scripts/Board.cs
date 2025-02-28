using PathCreation;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private BallSlot ballSlotPrefab; 
    [SerializeField] private PathCreator pathCreator;
    private void Start()
    {
        InitializeBallSlots();
    }

    private void InitializeBallSlots()
    {
        float pathLength = pathCreator.path.length;
        float slotsCount = (int)pathLength;
        float step = pathLength / slotsCount;

        for (int i = 0; i < slotsCount; ++i)
        {
            float distanceTraveled = i * step;

            Vector3 slotPosition = pathCreator.path.GetPointAtDistance(i);
            BallSlot ballSlot = Instantiate(ballSlotPrefab, slotPosition, Quaternion.identity, transform);
            ballSlot.SetDistanceTraveled(distanceTraveled);
        }
    }
}
