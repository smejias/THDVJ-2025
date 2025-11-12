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
            registeredEnemies.Add(enemy);
    }

    public void Unregister(StateMachine enemy)
    {
        registeredEnemies.Remove(enemy);
    }

    public void BroadcastAlert()
    {
        foreach (var enemy in registeredEnemies)
        {
            enemy.ChangeState<AlertState>();
        }

        Debug.Log($"[AlertSystem] ALERT broadcast to {registeredEnemies.Count} enemies!");
    }
}
