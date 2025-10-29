using UnityEngine;

[CreateAssetMenu(fileName = "NewGunData", menuName = "Guns/New Gun Data", order = 0)]
public class SO_Gun : ScriptableObject
{
    [Header("Gun Stats")]
    public float bulletSpeed = 50f;
    public float shootRate = 0.5f;
    public float gunRange = 100f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
}

