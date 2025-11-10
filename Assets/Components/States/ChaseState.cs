using UnityEngine;

public class ChaseState : State
{
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private Transform player;
    [SerializeField] private float loseTargetTime = 2f;

    private MovementComponent movement;
    private VisionComponent vision;
    private float timeWithoutVision = 0f;

    public override void OnEnter()
    {
        Debug.Log($"{name}: Entering Chase");

        if (movement == null)
            movement = GetComponentInParent<MovementComponent>();

        if (vision == null)
            vision = GetComponentInParent<VisionComponent>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (movement != null && player != null)
            movement.SetTarget(player);

        timeWithoutVision = 0f;
    }

    public override void Tick()
    {
        if (player == null || vision == null)
            return;

        bool canSeePlayer = false;
        foreach (var target in vision.GetCurrentTargets())
        {
            if (target == player.gameObject)
            {
                canSeePlayer = true;
                break;
            }
        }

        if (!canSeePlayer)
        {
            timeWithoutVision += Time.deltaTime;

            if (timeWithoutVision >= loseTargetTime)
            {
                stateMachine.ChangeState<PatrolState>();
            }
        }
        else
        {
            timeWithoutVision = 0f;
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Chase");
        if (movement != null)
            movement.ClearTarget();
    }
}