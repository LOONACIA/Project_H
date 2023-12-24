using LOONACIA.Unity.Collections;
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
        if (m_eventTable.TryGetValue(eventName, out ListenerList value))
        {
            value.RemoveHandler(action);
        }
    }

    public void DispatchEvent(string eventName)
    {
        if (m_eventTable.TryGetValue(eventName, out ListenerList value))
        {
            value.RaiseEvent();
        }
    }

    private readonly struct StrongEventListener : IEventListener
    {
        public StrongEventListener(Delegate handler)
        {
            Handler = handler;
        }

        public Delegate Handler { get; }

        public bool IsSame(Delegate handler)
        {
            return Equals(handler, Handler);
        }
    }

    private readonly struct WeakEventListener : IEventListener
    {
        private readonly WeakReference<Delegate> m_handler;

        public WeakEventListener(Delegate handler)
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
        private readonly List<IEventListener> m_listeners = new();

        public void AddListener(IEventListener listener)
        {
            m_listeners.Add(listener);
        }

        public void RemoveListener(IEventListener listener)
        {
            m_listeners.Remove(listener);
        }

        public void AddHandler(Delegate handler)
        {
            IEventListener listener = new StrongEventListener(handler);

            m_listeners.Add(listener);
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
            using PooledList<IEventListener> list = new(m_listeners);
            foreach (var listener in list)
            {
                RaiseEvent(in listener);
            }
        }

        private void RaiseEvent(in IEventListener listener)
        {
            if (listener.Handler is Action action)
            {
                action.Invoke();
            }
        }
    }
}

public interface IEventListener
{
    Delegate Handler { get; }

    bool IsSame(Delegate handler);
}