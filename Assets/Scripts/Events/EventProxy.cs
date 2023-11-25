using System;
using System.Collections.Generic;
using UnityEngine;

public class EventProxy : MonoBehaviour, IEventProxy
{
    private readonly Dictionary<string, ListenerList> m_eventTable = new();
    
    public void AddHandler(string eventName, Action action)
    {
        m_eventTable.TryAdd(eventName, new());

        m_eventTable[eventName].AddHandler(action);
    }
    
    public void RemoveHandler(string eventName, Action action)
    {
        if (m_eventTable.TryGetValue(eventName, value: out ListenerList value))
        {
            value.RemoveHandler(action);
        }
    }
    
    private void DispatchEvent(string eventName)
    {
        if (m_eventTable.TryGetValue(eventName, out ListenerList value))
        {
            value.RaiseEvent();
        }
    }

    private readonly struct EventListener
    {
        private readonly WeakReference<Delegate> m_handler;

        public EventListener(Delegate handler)
        {
            m_handler = new(handler);
        }

        public Delegate Handler => m_handler.TryGetTarget(out var handler) ? handler : null;
        
        public bool IsAlive => m_handler?.TryGetTarget(out _) is true;
        
        public bool IsSame(Delegate handler)
        {
            return Equals(handler, Handler);
        }
    }
    
    private class ListenerList
    {
        private readonly List<EventListener> m_listeners = new();
        
        public void AddListener(EventListener listener)
        {
            m_listeners.Add(listener);
        }
        
        public void RemoveListener(EventListener listener)
        {
            m_listeners.Remove(listener);
        }

        public void AddHandler(Delegate handler)
        {
            m_listeners.Add(new(handler));
        }

        public void RemoveHandler(Delegate handler)
        {
            for (int i = 0; i < m_listeners.Count; i++)
            {
                if (m_listeners[i].IsSame(handler))
                {
                    m_listeners.RemoveAt(i);
                    break;
                }
            }
        }

        public void RaiseEvent()
        {
            foreach (var listener in m_listeners)
            {
                RaiseEvent(in listener);
            }
        }

        private void RaiseEvent(in EventListener listener)
        {
            if (listener.Handler is Action action)
            {
                action.Invoke();
            }
        }
    }
}
