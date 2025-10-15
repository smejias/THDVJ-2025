using UnityEngine;

public class HitBoxComponent : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] public float damageAmount = 10f;
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private bool continuousDamage = false;
    [SerializeField] private float damageInterval = 1f; // For continuous damage

    private HurtBoxComponent currentTarget;
    private float damageTimer;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"HitBoxComponent on {gameObject.name} needs a Collider!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HurtBoxComponent hurtBox = other.GetComponent<HurtBoxComponent>();
        if (hurtBox != null)
        {
            currentTarget = hurtBox;

            if (!continuousDamage)
            {
         
                DealDamage(hurtBox);
                OnHit();
            }
            else
            {
              
                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HurtBoxComponent hurtBox = other.GetComponent<HurtBoxComponent>();
        if (hurtBox == currentTarget)
        {
            currentTarget = null;
        }
    }

    private void Update()
    {
        if (continuousDamage && currentTarget != null)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                DealDamage(currentTarget);
            }
        }
    }

    private void DealDamage(HurtBoxComponent hurtBox)
    {
        hurtBox.TakeDamage(damageAmount);
    }

    public void OnHit()
    {
        if (destroyOnHit)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}