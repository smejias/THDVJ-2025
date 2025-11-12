using TMPro;
using UnityEngine;

public class EnemyStateUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private StateMachine stateMachine;

    private void Awake()
    {
        
        if (cameraTransform == null)
            cameraTransform = Camera.main?.transform;
    }
        
    private void Update()
    {
        text.text = stateMachine.CurrentState.ToString();
    }

    private void LateUpdate()
    {
        if (text == null || cameraTransform == null)
            return;

     
        text.transform.rotation = Quaternion.LookRotation(text.transform.position - cameraTransform.position);

        
    }

    public void UpdateStateLabel(string stateName)
    {
        if (text != null)
            text.text = stateName;
    }
}
