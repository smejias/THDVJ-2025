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

        if (targetHealth == null)
        {
            targetHealth = GetComponent<HealthComponent>();

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
    public void TakeDamage(float damage)
    {
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }
    }
}