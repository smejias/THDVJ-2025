using UnityEngine;

public class AimAtScreenCenter : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxAimDistance = 100f;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(maxAimDistance);
        }

        transform.LookAt(targetPoint);
    }

    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = playerCamera.ScreenPointToRay(screenCenter);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, ray.GetPoint(maxAimDistance));
        }
    }
}