using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 startPosition;
    private float maxRange;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void SetRange(float range)
    {
        maxRange = range;
    }

    private void Update()
    {
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
    }
}