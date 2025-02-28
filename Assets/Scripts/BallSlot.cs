using PathCreation;
using UnityEngine;

public class BallSlot : MonoBehaviour
{
    [Header("Elements")]
    private PathCreator pathCreator;

    [Header("Data")]
    private float distanceTraveled;

    private void Start()
    {
        pathCreator = Object.FindFirstObjectByType<PathCreator>();
    }

    public void SetDistanceTraveled(float distanceTraveled)
    {
        this.distanceTraveled = distanceTraveled;
    }

    private void Update()
    {
        if (pathCreator)
        {
            distanceTraveled += Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTraveled);
        }
    }
}
