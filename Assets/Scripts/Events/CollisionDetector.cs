using System;
using UnityEngine;

// TODO: Migrate to EventProxy
public class CollisionDetector : MonoBehaviour
{
    public event EventHandler<CollisionEvent> CollisionDetected; 

    private void OnCollisionEnter(Collision other)
    {
        CollisionDetected?.Invoke(this, new CollisionEvent(gameObject, other, CollisionEvent.Type.Enter));
    }
    
    private void OnCollisionStay(Collision other)
    {
        CollisionDetected?.Invoke(this, new CollisionEvent(gameObject, other, CollisionEvent.Type.Stay));
    }
    
    private void OnCollisionExit(Collision other)
    {
        CollisionDetected?.Invoke(this, new CollisionEvent(gameObject, other, CollisionEvent.Type.Exit));
    }
}

public struct CollisionEvent
{
    public enum Type
    {
        Enter,
        Stay,
        Exit
    }
    
    public CollisionEvent(GameObject sender, Collision collision, Type eventType)
    {
        Sender = sender;
        Collision = collision;
        EventType = eventType;
    }
    
    public GameObject Sender { get; }
    
    public Collision Collision { get; }
    
    public Type EventType { get; }
}