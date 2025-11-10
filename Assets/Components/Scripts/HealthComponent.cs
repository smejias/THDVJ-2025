using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float regenerationRate = 5f;
    [SerializeField] private bool canRegenerate = false;
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] private bool canDie = true;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 2f;

    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsAtFullHealth => currentHealth >= maxHealth;
    public bool IsDead => currentHealth <= 0 && canDie;
    public bool CanTakeDamage => canTakeDamage;
    public bool CanDie => canDie;

    private bool hasDied = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (canRegenerate && currentHealth < maxHealth && !IsDead)
        {
            currentHealth += regenerationRate * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!canTakeDamage || damage <= 0 || IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log(transform.parent.name + " attacked. Health at: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0 && canDie && !hasDied)
        {
            hasDied = true;
            OnDeath?.Invoke();
            Debug.Log($"{this.transform.parent.name} has died!");

            if (destroyOnDeath)
            {
                if (destroyDelay > 0)
                {
                    Destroy(this.transform.parent, destroyDelay);
                }
                else
                {
                    Destroy(this.transform.parent);
                }
            }
        }
    }

    public void RestoreHealth(float amount)
    {
        if (amount <= 0 || IsDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void ResetToFullHealth()
    {
        currentHealth = maxHealth;
        hasDied = false;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetCanTakeDamage(bool enabled)
    {
        canTakeDamage = enabled;
    }

    public void SetCanDie(bool enabled)
    {
        canDie = enabled;
    }

    public void SetRegeneration(bool enabled)
    {
        canRegenerate = enabled;
    }
}