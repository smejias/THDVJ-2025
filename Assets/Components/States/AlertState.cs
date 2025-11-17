using UnityEngine;

public class AlertState : State
{
    public override void OnEnter()
    {
        Debug.Log($"{name}: Entering Alert - Broadcasting to all enemies!");

        if (EnemyAlertSystem.Instance != null)
        {
            EnemyAlertSystem.Instance.BroadcastAlert();
        }
        else
        {
            Debug.LogWarning("EnemyAlertSystem not found!");
        }

        ChaseState chaseState = stateMachine.GetComponentInChildren<ChaseState>(true);
        if (chaseState != null)
        {
            stateMachine.ChangeState<ChaseState>();
        }
        else
        {
            Debug.Log($"{name}: No ChaseState found (camera or stationary enemy), staying in Alert");
        }
    }

    public override void Tick()
    {
        // AlertState immediately transitions after broadcasting
    }

    public override void OnExit()
    {
        Debug.Log($"{name}: Exiting Alert");
    }
}