using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private float rotationSpeed = 10f;

    private Transform target;
    private float movementInput = 0f;

    private void Awake()
    {
        if (!rigidBody) rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    public void SetTarget(Transform newTarget) => target = newTarget;
    public void ClearTarget() => target = null;

    public void SetMovementInput(float input) => movementInput = Mathf.Clamp(input, -1f, 1f);


    private void FixedUpdate()
    {
        if (movementInput != 0f)
        {
            MoveAndRotate(transform.forward, moveSpeed * movementInput);
            return;
        }

        if (target == null)
        {
            rigidBody.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;

        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > stoppingDistance)
        {
            MoveAndRotate(directionToTarget.normalized, moveSpeed);
        }
        else
        {
            rigidBody.linearVelocity = Vector3.zero;
        }
    }

    private void MoveAndRotate(Vector3 direction, float currentSpeed)
    {
        Vector3 directionNormalized = direction.normalized;

        if (directionNormalized.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionNormalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }

        Vector3 movementVector = directionNormalized * currentSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = rigidBody.position + movementVector;
        rigidBody.MovePosition(newPosition);
    }
}