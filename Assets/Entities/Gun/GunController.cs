using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [Header("Gun Data")]
    [SerializeField] private SO_Gun gunData;
    [Header("Spawn Point")]
    [SerializeField] private Transform bulletSpawnPoint;

    private bool canShoot;

    private void Start()
    {
        canShoot = true;

        if (gunData == null)
        {
            Debug.LogError($"{name} has no Gun Data assigned!", this);
        }
    }

    private void HandleShoot()
    {
        if (gunData == null || gunData.bulletPrefab == null)
        {
            Debug.LogWarning("Gun Data or Bullet Prefab is missing!", this);
            return;
        }

        GameObject bullet = Instantiate(gunData.bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

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

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && canShoot)
        {
            canShoot = false;
            StartCoroutine(ShootAction());
        }
    }

    private IEnumerator ShootAction()
    {
        HandleShoot();
        yield return new WaitForSeconds(gunData.shootRate);
        canShoot = true;
    }
}
