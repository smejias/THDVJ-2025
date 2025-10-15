using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class VisionComponent : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 60f;

    [Header("Events")]
    public UnityEvent<GameObject> OnTargetSeen;
    public UnityEvent<GameObject> OnTargetLost;

    private List<GameObject> currentTargets = new List<GameObject>();

    private void Update()
    {
        CheckVision();
    }

    private void CheckVision()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(targetTag);
        List<GameObject> newTargets = new List<GameObject>();

        // Check each candidate
        foreach (GameObject candidate in candidates)
        {
            if (CanSee(candidate))
            {
                newTargets.Add(candidate);

                // New target found
                if (!currentTargets.Contains(candidate))
                {
                    OnTargetSeen?.Invoke(candidate);
                }
            }
        }

        // Check for lost targets
        foreach (GameObject oldTarget in currentTargets)
        {
            if (!newTargets.Contains(oldTarget))
            {
                OnTargetLost?.Invoke(oldTarget);
            }
        }

        currentTargets = newTargets;
    }

    private bool CanSee(GameObject target)
    {
        Vector3 directionToTarget = target.transform.position - transform.position;
        float distance = directionToTarget.magnitude;

        // Too far?
        if (distance > visionRange) return false;

        // Outside cone angle?
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle > visionAngle / 2f) return false;

        return true;
    }

    // Show vision cone in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Draw range circle
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Draw cone lines
        Vector3 leftEdge = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward * visionRange;
        Vector3 rightEdge = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward * visionRange;

        Gizmos.DrawLine(transform.position, transform.position + leftEdge);
        Gizmos.DrawLine(transform.position, transform.position + rightEdge);
    }
}