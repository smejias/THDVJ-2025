using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertSystem : MonoBehaviour
{
    public static EnemyAlertSystem Instance { get; private set; }

    private readonly List<StateMachine> registeredEnemies = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(StateMachine enemy)
    {
        if (!registeredEnemies.Contains(enemy))
        {
            registeredEnemies.Add(enemy);
            Debug.Log($"[AlertSystem] Registered: {enemy.name}");
        }
    }

    public void Unregister(StateMachine enemy)
    {
        registeredEnemies.Remove(enemy);
    }

    public void BroadcastAlert()
    {
        Debug.Log($"[AlertSystem] ALERT broadcast to {registeredEnemies.Count} enemies!");

        foreach (var enemy in registeredEnemies)
        {
            if (enemy == null) continue;

            ChaseState chaseState = enemy.GetComponentInChildren<ChaseState>(true);

            if (chaseState != null)
            {
                enemy.ChangeState<ChaseState>();
                Debug.Log($"[AlertSystem] {enemy.name} → ChaseState");
            }
            else
            {
                Debug.Log($"[AlertSystem] {enemy.name} has no ChaseState (camera or stationary enemy)");
            }
        }
    }
}