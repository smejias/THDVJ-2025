using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private VisionComponent vision;
    [SerializeField] private HealthComponent health;

    private void Awake()
    {
        vision.OnTargetSeen.AddListener(OnPlayerSeen);
        health.OnDeath.AddListener(OnDeath);
        health.OnTookDamage.AddListener(OnDamaged);

        if (EnemyAlertSystem.Instance != null)
        {
            EnemyAlertSystem.Instance.Register(stateMachine);
        }
    }

    private void OnDestroy()
    {
        if (EnemyAlertSystem.Instance != null)
        {
            EnemyAlertSystem.Instance.Unregister(stateMachine);
        }
    }

    private void OnPlayerSeen(GameObject player)
    {
        stateMachine.ChangeState<AlertState>();
    }

    private void OnDamaged() 
    {
        stateMachine.ChangeState<DamagedState>();
    }

    private void OnDeath()
    {
        stateMachine.ChangeState<DeadState>();
    }
}