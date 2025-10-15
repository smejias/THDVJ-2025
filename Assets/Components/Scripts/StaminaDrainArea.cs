using UnityEngine;
using System.Collections.Generic;

public class StaminaDrainArea : MonoBehaviour
{
    [Header("Drain Settings")]
    [SerializeField] private float drainInterval = 1f;
    [SerializeField] private float drainAmount = 1f;
    [SerializeField] private bool blockStaminaRegen = true;

    private List<StaminaComponent> staminaTargets = new List<StaminaComponent>();
    private float drainTimer;

    public void OnTargetInArea(GameObject target)
    {
        StaminaComponent stamina = target.GetComponentInChildren<StaminaComponent>();
        if (stamina != null && !staminaTargets.Contains(stamina))
        {
            staminaTargets.Add(stamina);
            Debug.Log($"Target with stamina entered drain area: {target.name}");

            if (blockStaminaRegen)
            {
                stamina.SetRegeneration(false);
            }
        }
    }

    public void OnTargetLeaveArea(GameObject target)
    {
        StaminaComponent stamina = target.GetComponentInChildren<StaminaComponent>();
        if (stamina != null && staminaTargets.Contains(stamina))
        {
            Debug.Log($"Target with stamina left drain area: {target.name}");

            if (blockStaminaRegen)
            {
                stamina.SetRegeneration(true);
            }

            staminaTargets.Remove(stamina);
        }
    }

    private void Update()
    {
        if (staminaTargets.Count == 0) return;

        drainTimer += Time.deltaTime;
        if (drainTimer >= drainInterval)
        {
            drainTimer = 0f;

            for (int i = staminaTargets.Count - 1; i >= 0; i--)
            {
                if (staminaTargets[i] != null)
                {
                    staminaTargets[i].UseStamina(drainAmount);
                }
                else
                {
                    staminaTargets.RemoveAt(i);
                }
            }
        }
    }
}