using UnityEngine;

[CreateAssetMenu(fileName = "MediKit", menuName = "Game/Items/MediKit")]
public class SO_MediKit : SO_PickupItem
{
    [Header("MediKit Settings")]
    public float healthAmount = 50f;

    public override bool Use(GameObject player)
    {
        HealthComponent health = player.GetComponentInChildren<HealthComponent>();
        if (health != null)
        {
            if (health.IsAtFullHealth)
            {
                Debug.Log($"[MediKit] Already at full health, cannot use");
                return false;
            }

            health.RestoreHealth(healthAmount);
            Debug.Log($"[MediKit] Restored {healthAmount} health");
            return true;
        }
        return false;
    }
}