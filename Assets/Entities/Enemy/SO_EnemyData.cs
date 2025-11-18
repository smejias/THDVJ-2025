using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Game/Enemy Data")]
public class SO_EnemyData : ScriptableObject
{
    [Header("Enemy Type")]
    public string enemyName = "Soldier";

    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Vision Settings")]
    public float visionRange = 10f;
    public float visionAngle = 60f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Combat (if applicable)")]
    public bool canAttack = true;
    public float attackRange = 10f;
    public float attackDamage = 10f;
    public SO_Gun weaponData;

    [Header("State Timers")]
    public float damageStateTime = 1f;
    public float alertTriggerTime = 3f;
}