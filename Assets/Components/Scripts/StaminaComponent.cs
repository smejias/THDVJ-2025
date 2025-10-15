using UnityEngine;
using UnityEngine.Events;

public class StaminaComponent : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float currentStamina = 10f;
    [SerializeField] private float regenerationRate = 1f; // Stamina per second
    [SerializeField] private bool canRegenerate = true;

    [Header("Events")]
    public UnityEvent<float> OnStaminaChanged;
    public UnityEvent OnStaminaEmpty;

    // Properties
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool IsEmpty => currentStamina <= 0;
    public bool CanRegenerate => canRegenerate;

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (canRegenerate && currentStamina < maxStamina)
        {
            currentStamina += regenerationRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina);
        }
    }

    public bool UseStamina(float amount)
    {
        if (amount <= 0 || IsEmpty) return false;

        currentStamina -= amount;
        currentStamina = Mathf.Max(0, currentStamina);

        OnStaminaChanged?.Invoke(currentStamina);

        if (IsEmpty)
        {
            OnStaminaEmpty?.Invoke();
        }

        return true;
    }

    public void SetRegeneration(bool enabled)
    {
        canRegenerate = enabled;
    }
}