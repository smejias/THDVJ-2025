using UnityEngine;

public class ChaseState : State
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float optimalDistance = 7f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float shootingRange = 10f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyWeaponComponent directWeaponReference;
    private EnemyWeaponComponent weapon;
    private Transform enemyTransform;
    private CharacterController controller;

    public override void OnEnter()
    {
        Debug.Log($"{name}: Entering Chase");

        if (directWeaponReference != null)
        {
            weapon = directWeaponReference;
        }
        else
        {
            EnemyAI enemyAI = GetComponentInParent<EnemyAI>();
            if (enemyAI != null)
            {
                weapon = enemyAI.GetComponent<EnemyWeaponComponent>();
            }
        }

        EnemyAI currentEnemyAI = GetComponentInParent<EnemyAI>();
        if (currentEnemyAI != null)
        {
            enemyTransform = currentEnemyAI.transform;
            controller = currentEnemyAI.GetComponent<CharacterController>();
        }
        else
        {
            enemyTransform = transform;
            controller = GetComponent<CharacterController>();
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (weapon != null && player != null)
            weapon.SetTarget(player);
    }

    public override void Tick()
    {
        if (player == null || enemyTransform == null)
        {
            return;
        }

        Vector3 directionToPlayer = player.position - enemyTransform.position;
        directionToPlayer.y = 0;
        float distanceToPlayer = directionToPlayer.magnitude;

        Vector3 rotationDirection = directionToPlayer.normalized;

        Vector3 movementDirection = rotationDirection;
        float currentSpeed = 0f;

        if (distanceToPlayer > maxDistance || distanceToPlayer > optimalDistance + 1f)
        {
            currentSpeed = chaseSpeed;
        }
        else if (distanceToPlayer < minDistance || distanceToPlayer < optimalDistance - 1f)
        {
            currentSpeed = chaseSpeed * 0.5f;
            movementDirection = -movementDirection;
        }

        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);

            enemyTransform.rotation = Quaternion.Slerp(
                enemyTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (currentSpeed > 0)
        {
            Vector3 movement = movementDirection * currentSpeed * Time.deltaTime;

            if (controller != null)
            {
                controller.Move(movement);
            }
            else
            {
                enemyTransform.position += movement;
            }
        }

        if (distanceToPlayer <= shootingRange && weapon != null)
        {
            weapon.Fire();
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Chase");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, optimalDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

    public void SetMoveSpeed(float speed)
    {
        chaseSpeed = speed;
    }

    public void SetShootingRange(float range)
    {
        shootingRange = range;
    }
}