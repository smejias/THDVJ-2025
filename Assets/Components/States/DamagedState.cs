using UnityEngine;

public class DamagedState : State
{
    [SerializeField] private float recoveryTime = 3f;
    private float timer;

    public override void OnEnter()
    {
        timer = 0f;
        Debug.Log($"{name}: Entering Damaged");
    }

    public override void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= recoveryTime)
            stateMachine.ChangeState<AlertState>();
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Damaged");
    }
}