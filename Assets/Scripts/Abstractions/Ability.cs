using System;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    private AbilityState m_state;
    
    private IEventProxy m_eventProxy;
    
    public Monster Owner { get; set; }

    public virtual bool CanActivate { get; set; } = true;
    
    public AbilityState State
    {
        get => m_state;
        private set
        {
            if (m_state == value)
            {
                return;
            }

            m_state = value;
            OnStateChanged(m_state);
        }
    }
    
    public event EventHandler<AbilityState> StateChanged;
    
    protected virtual void Awake()
    {
        if (Owner == null)
        {
            Owner = GetComponentInParent<Monster>();
        }

        m_eventProxy = GetComponent<IEventProxy>();
    }

    protected virtual void OnEnable()
    {
        RegisterEvents(m_eventProxy);
    }
    
    protected virtual void OnDisable()
    {
        UnregisterEvents(m_eventProxy);
    }

    protected virtual void OnStateChanged(AbilityState state)
    {
        StateChanged?.Invoke(this, state);
    }
    
    protected virtual void RegisterEvents(IEventProxy eventProxy)
    {
        eventProxy.AddHandler($"On{nameof(AbilityState.Idle)}", OnIdle);
        eventProxy.AddHandler($"On{nameof(AbilityState.PreActivate)}", OnPreActivate);
        eventProxy.AddHandler($"On{nameof(AbilityState.Activate)}", OnActivate);
        eventProxy.AddHandler($"On{nameof(AbilityState.Deactivating)}", OnDeactivating);
    }
    
    protected virtual void UnregisterEvents(IEventProxy eventProxy)
    {
        eventProxy.RemoveHandler($"On{nameof(AbilityState.Idle)}", OnIdle);
        eventProxy.RemoveHandler($"On{nameof(AbilityState.PreActivate)}", OnPreActivate);
        eventProxy.RemoveHandler($"On{nameof(AbilityState.Activate)}", OnActivate);
        eventProxy.RemoveHandler($"On{nameof(AbilityState.Deactivating)}", OnDeactivating);
    }

    protected virtual void OnIdleState()
    {
    }

    protected virtual void OnPreActivateState()
    {
    }

    protected virtual void OnActivateState()
    {
    }

    protected virtual void OnDeactivatingState()
    {
    }

    private void OnIdle()
    {
        if (State == AbilityState.Idle)
        {
            return;
        }
        
        State = AbilityState.Idle;
        OnIdleState();
    }

    private void OnPreActivate()
    {
        if (State == AbilityState.PreActivate)
        {
            return;
        }
        
        State = AbilityState.PreActivate;
        OnPreActivateState();
    }
    
    private void OnActivate()
    {
        if (State == AbilityState.Activate)
        {
            return;
        }
        
        State = AbilityState.Activate;
        OnActivateState();
    }
    
    private void OnDeactivating()
    {
        if (State == AbilityState.Deactivating)
        {
            return;
        }
        
        State = AbilityState.Deactivating;
        OnDeactivatingState();
    }
}