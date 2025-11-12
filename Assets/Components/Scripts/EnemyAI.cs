using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private VisionComponent vision;
    [SerializeField] private HealthComponent health;

    private void Awake()
    {
        vision.OnTargetSeen.AddListener(OnPlayerSeen);
        vision.OnTargetLost.AddListener(OnPlayerLost);
        health.OnDeath.AddListener(OnDeath);
        health.OnHealthChanged.AddListener(OnDamaged);
    }



    private void OnPlayerSeen(GameObject player)
        => stateMachine.ChangeState<ChaseState>();

    private void OnPlayerLost(GameObject player)
        => stateMachine.ChangeState<PatrolState>();

    private void OnDamaged(float newHealth)
        => stateMachine.ChangeState<DamagedState>();

    private void OnDeath()
        => stateMachine.ChangeState<DeadState>();



}
