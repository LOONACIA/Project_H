using System;

public interface IEventProxy
{
    void AddHandler(string eventName, Action action);

    void RemoveHandler(string eventName, Action action);
    
    void DispatchEvent(string eventName);
}