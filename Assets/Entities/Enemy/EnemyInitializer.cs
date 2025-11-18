using UnityEngine;

public class EnemyInitializer : MonoBehaviour
{
    [Header("Enemy Configuration")]
    [SerializeField] private SO_EnemyData enemyData;

    private void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError($"{name}: No EnemyData assigned!");
            return;
        }

        InitializeEnemy();
    }

    private void InitializeEnemy()
    {
        Debug.Log($"[EnemyInitializer] Initializing {enemyData.enemyName}");

        InitializeHealth();
        InitializeVision();
        InitializeStates();
        InitializeWeapon();
    }

    private void InitializeHealth()
    {
        HealthComponent health = GetComponentInChildren<HealthComponent>();
        if (health != null)
        {
            health.SetMaxHealth(enemyData.maxHealth);
            Debug.Log($"[EnemyInitializer] Health set to {enemyData.maxHealth}");
        }
    }

    private void InitializeVision()
    {
        VisionComponent vision = GetComponentInChildren<VisionComponent>();
        if (vision != null)
        {
            vision.SetVisionRange(enemyData.visionRange);
            vision.SetVisionAngle(enemyData.visionAngle);
            Debug.Log($"[EnemyInitializer] Vision: {enemyData.visionRange}m range, {enemyData.visionAngle}° angle");
        }
    }

    private void InitializeStates()
    {
        PatrolState patrol = GetComponentInChildren<PatrolState>(true);
        if (patrol != null)
        {
            patrol.SetMoveSpeed(enemyData.patrolSpeed);
            Debug.Log($"[EnemyInitializer] Patrol speed: {enemyData.patrolSpeed}");
        }

        ChaseState chase = GetComponentInChildren<ChaseState>(true);
        if (chase != null)
        {
            chase.SetMoveSpeed(enemyData.chaseSpeed);
            chase.SetShootingRange(enemyData.attackRange);
            Debug.Log($"[EnemyInitializer] Chase speed: {enemyData.chaseSpeed}, Range: {enemyData.attackRange}");
        }

        DamagedState damaged = GetComponentInChildren<DamagedState>(true);
        if (damaged != null)
        {
            damaged.SetAlertTime(enemyData.alertTriggerTime);
            Debug.Log($"[EnemyInitializer] Alert trigger time: {enemyData.alertTriggerTime}s");
        }
    }

    private void InitializeWeapon()
    {
        EnemyWeaponComponent weapon = GetComponentInChildren<EnemyWeaponComponent>();

        if (!enemyData.canAttack)
        {
            if (weapon != null)
            {
                weapon.enabled = false;
                Debug.Log($"[EnemyInitializer] Weapon disabled (camera or non-combat enemy)");
            }
            return;
        }

        if (weapon != null && enemyData.weaponData != null)
        {
            weapon.SetGunData(enemyData.weaponData);
            Debug.Log($"[EnemyInitializer] Weapon configured");
        }
    }

    public SO_EnemyData GetEnemyData() => enemyData;
}