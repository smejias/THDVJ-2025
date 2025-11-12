using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<Type, State> states = new();
    private State currentState;

    [SerializeField] private State defaultState;

    protected virtual void Awake()
    {
        foreach (var state in GetComponentsInChildren<State>(true))
        {
            state.Initialize(this);
            states[state.GetType()] = state;
        }
    }
    protected virtual void Start()
    {
        if (defaultState != null)
        {
            ChangeState(defaultState.GetType());   // enters default state on play
        }
        else
        {
            Debug.LogWarning($"{name}: No default state assigned!");
        }

        EnemyAlertSystem.Instance?.Register(this);

        if (defaultState != null)
            ChangeState(defaultState.GetType());
    }


    private void Update()
    {
        currentState?.Tick();
    }

    public void ChangeState<T>() where T : State
    {
        ChangeState(typeof(T)); // this calls the non-generic version
    }

    public void ChangeState(Type stateType)
    {
        if (currentState != null && currentState.GetType() == stateType)
            return;

        currentState?.OnExit();

        if (states.TryGetValue(stateType, out var nextState))
        {
            currentState = nextState;
            currentState.OnEnter();
            Debug.Log($"{name}: {currentState.GetType().Name}");

            var ui = GetComponentInChildren<EnemyStateUI>();
            if (ui != null)
                ui.UpdateStateLabel(currentState.GetType().Name);
        }
        else
        {
            Debug.LogWarning($"{name}: Tried to change to state {stateType.Name} but it's not registered.");
        }
    }

    public State CurrentState => currentState;

    private void OnDestroy()
    {
        EnemyAlertSystem.Instance?.Unregister(this);
    }
}
