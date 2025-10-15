using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private enum EnemyState { Idle, Chase, Damaged, Dead }

    [Header("References")]
    [SerializeField] private MovementComponent movement;
    [SerializeField] private VisionComponent vision;
    [SerializeField] private HealthComponent health;
    [SerializeField] private HurtBoxComponent hurtBox;
    [SerializeField] private StaminaDrainArea staminaDrainArea;

    [Header("Dead State Settings")]
    [SerializeField] private Material onDeadMaterial;

    private EnemyState currentState = EnemyState.Idle;
    private GameObject target;

    private void Start()
    {
        if (!movement) movement = GetComponentInChildren<MovementComponent>();
        if (!vision) vision = GetComponentInChildren<VisionComponent>();
        if (!health) health = GetComponentInChildren<HealthComponent>();
        if (!hurtBox) hurtBox = GetComponentInChildren<HurtBoxComponent>();
        if (!staminaDrainArea) staminaDrainArea = GetComponentInChildren<StaminaDrainArea>();

        if (vision != null)
        {
            vision.OnTargetSeen.AddListener(OnPlayerSeen);
            vision.OnTargetLost.AddListener(OnPlayerLost);
        }

        if (health != null)
        {
            health.OnDeath.AddListener(OnDeath);
            health.OnHealthChanged.AddListener(OnHealthChanged);
        }
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Chase:
                if (target == null)
                {
                    ChangeState(EnemyState.Idle);
                    return;
                }
                movement.SetTarget(target.transform);
                break;

            case EnemyState.Damaged:
                if (movement != null)
                {
                    movement.ClearTarget();
                }

                if (target != null)
                {
                    ChangeState(EnemyState.Chase);
                }
                else
                {
                    ChangeState(EnemyState.Idle);
                }
                break;

            case EnemyState.Dead:
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        if (currentState == EnemyState.Dead) return;

        Debug.Log($"{name}: {currentState} → {newState}");

        switch (currentState)
        {
            case EnemyState.Chase:
                if (movement != null) movement.ClearTarget();
                break;
        }

        currentState = newState;

        switch (newState)
        {
            case EnemyState.Dead:
                if (movement != null) movement.enabled = false;
                if (vision != null) vision.enabled = false;
                if (staminaDrainArea != null) staminaDrainArea.enabled = false;
                if (hurtBox != null) hurtBox.enabled = false;

                Renderer renderer = GetComponentInParent<Renderer>();
                if (renderer != null && onDeadMaterial != null)
                {
                    renderer.material = onDeadMaterial;
                }
                break;
        }
    }

    private void OnPlayerSeen(GameObject player)
    {
        if (currentState == EnemyState.Dead) return;
        target = player;
        ChangeState(EnemyState.Chase);
    }

    private void OnPlayerLost(GameObject player)
    {
        if (currentState == EnemyState.Dead) return;
        if (player == target)
        {
            target = null;
            ChangeState(EnemyState.Idle);
        }
    }

    private void OnHealthChanged(float newHealth)
    {
        if (currentState != EnemyState.Dead && currentState != EnemyState.Damaged)
        {
            ChangeState(EnemyState.Damaged);
        }
    }

    private void OnDeath()
    {
        ChangeState(EnemyState.Dead);
    }

    private void OnDestroy()
    {
        if (vision != null)
        {
            vision.OnTargetSeen.RemoveListener(OnPlayerSeen);
            vision.OnTargetLost.RemoveListener(OnPlayerLost);
        }

        if (health != null)
        {
            health.OnDeath.RemoveListener(OnDeath);
            health.OnHealthChanged.RemoveListener(OnHealthChanged);
        }
    }
}