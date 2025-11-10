using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected StateMachine stateMachine;

    public virtual void Initialize(StateMachine machine)
    {
        stateMachine = machine;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Tick() { }
}
