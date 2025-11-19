using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [Header("Gun Data")]
    [SerializeField] private SO_Gun gunData;
    [Header("Spawn Point")]
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Attachment")]
    [SerializeField] private Transform handTransform; // assign "hand" child here

    [Header("Magazine Limits")]
    [SerializeField] private int maxClips = 10;

    private bool canShoot;
    private bool isReloading;

    private int bulletsLeftInClip;
    private int clipsLeft;

    public event System.Action OnAmmoChanged;
    public event System.Action<bool> OnReloadStateChanged;

    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    private void Start()
    {
        bulletsLeftInClip = gunData.clipSize;
        clipsLeft = gunData.clipAmount - 1;
        canShoot = true;

        // store offset relative to hand
        if (handTransform != null)
        {
            positionOffset = transform.position - handTransform.position;
            rotationOffset = Quaternion.Inverse(handTransform.rotation) * transform.rotation;
        }

        OnAmmoChanged?.Invoke();
    }

    private void LateUpdate()
    {
        if (handTransform != null)
        {
            transform.position = handTransform.position + handTransform.rotation * positionOffset;
            transform.rotation = handTransform.rotation * rotationOffset;
        }
    }

    private void HandleShoot()
    {
        if (gunData == null || gunData.bulletPrefab == null) return;

        Vector3 aimDir = GetAimDirection();

        GameObject bullet = Instantiate(
            gunData.bulletPrefab,
            bulletSpawnPoint.position,
            Quaternion.LookRotation(aimDir)
        );

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
            bulletRb.linearVelocity = aimDir * gunData.bulletSpeed;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.SetRange(gunData.gunRange);
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

    public void ResetAmmo()
    {
        bulletsLeftInClip = gunData.clipSize;
        clipsLeft = maxClips - 1; 
        canShoot = true;
        isReloading = false;

        OnAmmoChanged?.Invoke();

        Debug.Log($"[GunController] Ammo reset: {bulletsLeftInClip}/{gunData.clipSize} bullets, {clipsLeft} clips left");
    }

    public bool AddMagazines(int amount)
    {
        if (clipsLeft >= maxClips - 1)
        {
            Debug.Log($"[GunController] Already at max magazines ({maxClips})");
            return false;
        }

        clipsLeft += amount;
        clipsLeft = Mathf.Min(clipsLeft, maxClips - 1);

        OnAmmoChanged?.Invoke();

        Debug.Log($"[GunController] Added {amount} magazine(s). Total: {clipsLeft + 1}/{maxClips}");
        return true;
    }

    private Vector3 GetAimDirection()
    {
        Camera cam = Camera.main;
        Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Ray ray = cam.ScreenPointToRay(center);

        if (Physics.Raycast(ray, out RaycastHit hit, gunData.gunRange))
        {
            return (hit.point - bulletSpawnPoint.position).normalized;
        }

        return ray.direction;
    }

}
