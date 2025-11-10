using UnityEngine;

public class AlertState : State
{
    [SerializeField] private float alertDuration = 5f;
    private float timer;

    public override void OnEnter()
    {
        timer = 0f;
        Debug.Log($"{name}: Entering Alert");
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
    }
}
