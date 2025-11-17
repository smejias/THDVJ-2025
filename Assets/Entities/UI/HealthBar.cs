using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthSlider;

    [Header("Referencia al Player")]
    [SerializeField] private GameObject player;
    private HealthComponent healthComponent;

    private void Awake()
    {
        healthSlider = GetComponent<Slider>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            healthComponent = player.GetComponentInChildren<HealthComponent>(true);
        }
    }

    private void Start()
    {
        if (healthComponent != null)
        {
            healthSlider.maxValue = healthComponent.MaxHealth;

            healthSlider.value = healthComponent.CurrentHealth;

            healthComponent.OnHealthChanged.AddListener(UpdateHealthBar);
        }
        else
        {
            Debug.LogError("HealthComponent not found on the player! Check hierarchy and assignment.");
        }
    }

    private void UpdateHealthBar(float newHealth)
    {
        healthSlider.value = newHealth;
    }

    private void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }
}