using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class GunController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed;

    [Header("Gun Settings")]
    [SerializeField] private float shootRate;
    [SerializeField] private float gunRange;



    private bool canShoot;

    private void Start()
    {
        canShoot = true;

    }

    private void HandleShoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
        }

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetRange(gunRange);
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && canShoot)
        {
            canShoot = false;
            Debug.Log("Shoot Pressed");

            StartCoroutine(ShootAction());

        }

    }

    IEnumerator ShootAction()
    {
        HandleShoot();
        yield return new WaitForSeconds(shootRate);
        canShoot = true;

    }
}