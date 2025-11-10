using UnityEngine;

public class PatrolState : State
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTimeAtPoint = 2f;

    private MovementComponent movement;
    private int currentPoint;
    private float waitTimer;

    public override void OnEnter()
    {
        Debug.Log($"{name}: Entering Patrol");
        if (movement == null)
            movement = GetComponentInParent<MovementComponent>();

        currentPoint = 0;
        waitTimer = 0f;

        if (movement != null && patrolPoints.Length > 0)
            movement.SetTarget(patrolPoints[currentPoint]);
    }

    public override void Tick()
    {
        if (movement == null || patrolPoints.Length == 0)
            return;

        float distance = Vector3.Distance(movement.transform.position, patrolPoints[currentPoint].position);

        if (distance < 1.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                movement.SetTarget(patrolPoints[currentPoint]);
                waitTimer = 0f;
            }
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Patrol");
        if (movement != null)
            movement.ClearTarget();
    }
}
