using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private float stoppingDistance = 0.5f;

    private Transform target;

    private void Awake()
    {
        if (!rigidBody) rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    public void SetTarget(Transform newTarget) => target = newTarget;
    public void ClearTarget() => target = null;

    private void FixedUpdate()
    {
        if (target == null) return;

        var distance = Vector3.Distance(transform.position, target.position);

        if (distance > stoppingDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
            }

            Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;
            rigidBody.MovePosition(rigidBody.position + move);
        }
    }
}
    