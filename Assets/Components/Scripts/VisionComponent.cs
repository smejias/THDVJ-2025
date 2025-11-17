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

    private List<GameObject> currentTargets = new List<GameObject>();

    public List<GameObject> GetCurrentTargets() => currentTargets;

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

                if (!currentTargets.Contains(candidate))
                {
                    OnTargetSeen?.Invoke(candidate);
                }
            }
        }

        //foreach (GameObject oldTarget in currentTargets)
        //{
        //    if (!newTargets.Contains(oldTarget))
        //    {
        //        OnTargetLost?.Invoke(oldTarget);
        //    }
        //}

        currentTargets = newTargets;
    }

    private bool CanSee(GameObject target)
    {
        Vector3 toTarget = target.transform.position - transform.position;
        float distance = toTarget.magnitude;

        if (distance > visionRange)
            return false;

        Vector3 toTargetNormalized = toTarget.normalized;
        Vector3 forward = transform.forward;
        float dot = Vector3.Dot(forward, toTargetNormalized);
        float minDot = Mathf.Cos(visionAngle * 0.5f * Mathf.Deg2Rad);

        return dot >= minDot;
    }

    private void OnDrawGizmos()
    {
        if (currentTargets.Count > 0)
            Gizmos.color = new Color(1, 0, 0, 0.3f);
        else
            Gizmos.color = new Color(0, 1, 0, 0.3f);

        Vector3 forward = transform.forward * visionRange;
        Gizmos.DrawRay(transform.position, forward);

        Quaternion leftRot = Quaternion.AngleAxis(-visionAngle * 0.5f, Vector3.up);
        Vector3 leftBoundary = leftRot * transform.forward * visionRange;
        Gizmos.DrawRay(transform.position, leftBoundary);

        Quaternion rightRot = Quaternion.AngleAxis(visionAngle * 0.5f, Vector3.up);
        Vector3 rightBoundary = rightRot * transform.forward * visionRange;
        Gizmos.DrawRay(transform.position, rightBoundary);

        int segments = 20;
        Vector3 prevPoint = transform.position + leftBoundary;

        for (int i = 1; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-visionAngle * 0.5f, visionAngle * 0.5f, t);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 point = transform.position + rot * transform.forward * visionRange;
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }

        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.1f);
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}