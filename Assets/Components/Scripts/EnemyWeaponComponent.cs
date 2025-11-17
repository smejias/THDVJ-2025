using UnityEngine;

public class EnemyWeaponComponent : MonoBehaviour
{
    [Header("Gun Data")]
    [SerializeField] private SO_Gun gunData;

    [Header("Spawn Point")]
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Line of Sight Settings")]
    [SerializeField] private LayerMask obstacleMask;

    private float nextFireTime = 0f;
    private bool canShoot = true;

    private void Start()
    {
        if (bulletSpawnPoint == null)
        {
            bulletSpawnPoint = transform;
            Debug.LogWarning($"{name}: No bullet spawn point assigned, using self transform");
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    public bool Fire()
    {
        if (!canShoot || gunData == null || Time.time < nextFireTime)
            return false;

        if (target != null)
        {
            if (!HasClearLineOfSight())
                return false; 

            AimAtTarget();
        }

        HandleShoot();
        nextFireTime = Time.time + gunData.shootRate;
        return true;
    }

    private bool HasClearLineOfSight()
    {
        if (target == null) return false;

        Vector3 origin = bulletSpawnPoint.position;
        Vector3 targetCenter = target.position + Vector3.up * 0.5f;
        Vector3 direction = targetCenter - origin;
        float distance = direction.magnitude;

        RaycastHit hit;

        // Fire a raycast
        if (Physics.Raycast(origin, direction.normalized, out hit, distance, obstacleMask))
        {
   
            return false;
        }

        return true;
    }

    private void AimAtTarget()
    {
        if (target == null) return;

        Vector3 direction = target.position - bulletSpawnPoint.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            bulletSpawnPoint.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void HandleShoot()
    {
        if (gunData == null || gunData.bulletPrefab == null)
        {
            Debug.LogWarning($"{name}: Missing gun data or bullet prefab!");
            return;
        }

        GameObject bullet = Instantiate(
            gunData.bulletPrefab,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = bulletSpawnPoint.forward * gunData.bulletSpeed;
        }

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetRange(gunData.gunRange);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetCanShoot(bool enabled)
    {
        canShoot = enabled;
    }

    public SO_Gun GunData => gunData;
    public Transform Target => target;
}