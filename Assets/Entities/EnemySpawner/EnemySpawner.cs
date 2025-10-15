using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;

    private GameObject currentEnemy;
    private Vector3 originalSpawnPosition;
    private Quaternion originalSpawnRotation;

    private void Start()
    {
        if (spawnPoint != null)
        {
            originalSpawnPosition = spawnPoint.position;
            originalSpawnRotation = spawnPoint.rotation;
        }
        else
        {
            originalSpawnPosition = transform.position;
            originalSpawnRotation = transform.rotation;
        }

        SpawnEnemy();
    }

    private void Update()
    {
        if (currentEnemy == null)
        {
            SpawnEnemy();
        }
    }

    public void OnRespawn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RespawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && currentEnemy == null)
        {
            currentEnemy = Instantiate(enemyPrefab, originalSpawnPosition, originalSpawnRotation);

            HealthComponent enemyHealth = currentEnemy.GetComponentInChildren<HealthComponent>();

            if (enemyHealth != null)
            {
                enemyHealth.OnDeath.AddListener(OnEnemyDied);
            }

            Debug.Log("Enemy spawned at original position");
        }
    }

    private void RespawnEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }

        SpawnEnemy();
        Debug.Log("Enemy respawned");
    }

    private void OnEnemyDied()
    {
        Debug.Log("Enemy died, will respawn automatically");

        Invoke("RespawnEnemy", 1f);

    }
    //por si los llego a necesitar  
    public bool HasActiveEnemy()
    {
        return currentEnemy != null;
    }

    public GameObject GetCurrentEnemy()
    {
        return currentEnemy;
    }
}