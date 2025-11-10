using UnityEngine;

public class DeadState : State
{
    [SerializeField] private float deathDuration = 2f;
    private float timer;

    public override void OnEnter()
    {
        timer = 0f;
        Debug.Log($"{name}: Entering DeadState");
     
        var move = GetComponentInParent<MovementComponent>();
        if (move != null) move.enabled = false;

        var vision = GetComponentInParent<VisionComponent>();
        if (vision != null) vision.enabled = false;

        var rigid = GetComponentInParent<Rigidbody>();
        if (rigid != null)
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    public override void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= deathDuration)
        {
            var root = transform.root.gameObject;
            Debug.Log($"{root.name}: Destroyed after death");

            Object.Destroy(root);
        }
    }

    public override void OnExit()
    {
      Debug.Log($"{name}: Exiting DeadState");
    }
}
