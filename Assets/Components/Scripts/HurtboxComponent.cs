using UnityEngine;

public class HurtBoxComponent : MonoBehaviour
{
    [SerializeField] private HealthComponent targetHealth;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"HurtBoxComponent on {gameObject.name} needs a Collider!");
        }

        // Auto-find health component if not assigned
        if (targetHealth == null)
        {
            // First try same GameObject
            targetHealth = GetComponent<HealthComponent>();

            // If not found, try parent
            if (targetHealth == null)
            {
                targetHealth = GetComponentInParent<HealthComponent>();
            }

            // If still not found, try children
            if (targetHealth == null)
            {
                targetHealth = GetComponentInChildren<HealthComponent>();
            }

            if (targetHealth == null)
            {
                Debug.LogWarning($"HurtBoxComponent on {gameObject.name} couldn't find a HealthComponent!");
            }
        }
    }

    /// <summary>
    /// Called by HitBox to deal damage to this object
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
    }
}