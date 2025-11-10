using UnityEngine;

public class IdleState : State
{
    public override void OnEnter()
    {
        Debug.Log($"{name}: Entering Idle");
    }

    public override void Tick()
    {
        // Example: if patrol exists, switch to it
        if (GetComponentInChildren<PatrolState>() != null)
            stateMachine.ChangeState<PatrolState>();
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Idle");
    }
}
