using TMPro;
using UnityEngine;

public class EnemyStateUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Transform cameraTransform;
    private StateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponentInParent<StateMachine>();
        if (cameraTransform == null)
            cameraTransform = Camera.main?.transform;
    }

    private void LateUpdate()
    {
        if (text == null || cameraTransform == null)
            return;

        // Billboard: face camera
        text.transform.rotation = Quaternion.LookRotation(text.transform.position - cameraTransform.position);
    }

    public void UpdateStateLabel(string stateName)
    {
        if (text != null)
            text.text = stateName;
    }
}
