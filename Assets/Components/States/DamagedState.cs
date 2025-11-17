using UnityEngine;

public class DamagedState : State
{
    [SerializeField] private float alertTime = 3f;

    private float timer;

    public override void OnEnter()
    {
        timer = 0f;
        Debug.Log($"{name}: Taking Damage - Entering Damaged State");
    }

    public override void Tick()
    {
        timer += Time.deltaTime;

        if (timer >= alertTime)
        {
            Debug.Log($"{name}: Survived {alertTime} seconds! Going to Alert!");
            stateMachine.ChangeState<AlertState>();
        }
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Damaged");
    }
}