using UnityEngine;

public class BallFallOutPane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Ball>())
        {
            Destroy(other.gameObject);
        }
    }
}
