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
    private bool isReloading;

    private int bulletsLeftInClip;
    private int clipsLeft;

    public event System.Action OnAmmoChanged;
    public event System.Action<bool> OnReloadStateChanged;

    private void Start()
    {
        bulletsLeftInClip = gunData.clipSize;
        clipsLeft = gunData.clipAmount - 1;
        canShoot = true;
        OnAmmoChanged?.Invoke();
    }

    private void HandleShoot()
    {
        if (gunData == null || gunData.bulletPrefab == null) return;

        GameObject bullet = Instantiate(gunData.bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null) bulletRb.linearVelocity = bulletSpawnPoint.forward * gunData.bulletSpeed;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null) bulletScript.SetRange(gunData.gunRange);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && canShoot && !isReloading && bulletsLeftInClip > 0)
        {
            canShoot = false;
            StartCoroutine(ShootAction());
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed && !isReloading && bulletsLeftInClip < gunData.clipSize && clipsLeft > 0)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ShootAction()
    {
        HandleShoot();
        bulletsLeftInClip -= 1;
        OnAmmoChanged?.Invoke();

        yield return new WaitForSeconds(gunData.shootRate);

        if (bulletsLeftInClip > 0) canShoot = true;
        else if (clipsLeft <= 0) canShoot = false;
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        canShoot = false;
        OnReloadStateChanged?.Invoke(true);
        OnAmmoChanged?.Invoke();

        yield return new WaitForSeconds(gunData.reloadTime);

        clipsLeft -= 1;
        bulletsLeftInClip = gunData.clipSize;

        isReloading = false;
        canShoot = true;
        OnReloadStateChanged?.Invoke(false);
        OnAmmoChanged?.Invoke();
    }

    public int BulletsLeftInClip => bulletsLeftInClip;
    public int ClipsLeft => clipsLeft;
    public int ClipSize => gunData.clipSize;
}
