using UnityEngine;

public class PatrolState : State
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float waypointReachDistance = 1.5f;
    [SerializeField] private float waitTimeAtWaypoint = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Transform enemyTransform;
    private CharacterController controller; // NEW

    public override void OnEnter()
    {
        Debug.Log($"{name}: Entering Patrol");

        EnemyAI enemyAI = GetComponentInParent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyTransform = enemyAI.transform;
            controller = enemyAI.GetComponent<CharacterController>();
        }
        else
        {
            enemyTransform = transform;
            controller = GetComponent<CharacterController>();
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"{name}: No waypoints assigned!");
            return;
        }

        currentWaypointIndex = 0;
        isWaiting = false;
        waitTimer = 0f;
    }

    public override void Tick()
    {
        if (waypoints == null || waypoints.Length == 0 || enemyTransform == null)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null)
        {
            Debug.LogWarning($"{name}: Waypoint at index {currentWaypointIndex} is null!");
            return;
        }

        Vector3 currentPosition = enemyTransform.position;
        Vector3 targetPosition = targetWaypoint.position;

        Vector3 directionToWaypoint = targetPosition - currentPosition;
        directionToWaypoint.y = 0f;

        float distanceToWaypoint = directionToWaypoint.magnitude;

        if (distanceToWaypoint <= waypointReachDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
            }

            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }

        if (!isWaiting)
        {
            Vector3 directionNormalized = directionToWaypoint.normalized;

            if (directionToWaypoint.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionNormalized);

                enemyTransform.rotation = Quaternion.Slerp(
                    enemyTransform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            Vector3 movement = directionNormalized * patrolSpeed * Time.deltaTime;

            if (controller != null)
            {
                controller.Move(movement);
            }
            else
            {
                enemyTransform.position += movement;
            }
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Patrol");
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);

            int nextIndex = (i + 1) % waypoints.Length;
            if (waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0 || !Application.isPlaying)
            return;

        if (currentWaypointIndex >= 0 && currentWaypointIndex < waypoints.Length)
        {
            Transform currentWaypoint = waypoints[currentWaypointIndex];
            if (currentWaypoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(currentWaypoint.position, 0.5f);

                if (enemyTransform != null)
                    Gizmos.DrawLine(enemyTransform.position, currentWaypoint.position);
            }
        }
    }
}