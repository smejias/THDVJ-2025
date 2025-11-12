using UnityEngine;

public class AlertState : State
{
    [SerializeField] private bool isCamera = false;
    [SerializeField] private float alertDuration = 5f;

    private float timer;
    private MovementComponent movement;
    private Transform player;

    public override void OnEnter()
    {
        timer = 0f;
        movement = GetComponentInParent<MovementComponent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        Debug.Log($"{name}: Entering Alert");

        if (!isCamera && movement != null && player != null)
            movement.SetTarget(player);
    }

    public override void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= alertDuration)
            stateMachine.ChangeState<PatrolState>();
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Alert");
        if (movement != null)
            movement.ClearTarget();
    }
}
