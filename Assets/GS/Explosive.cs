using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Explosive : MonoBehaviour
{
    private Collider m_collider;

    private Rigidbody m_rigidBody;

    public void Explode(float force, Vector3 worldPosition, float radius)
    {
        if (TryGetComponent<Collider>(out m_collider)
            && TryGetComponent<Rigidbody>(out m_rigidBody))
        { 
            m_collider.isTrigger = true;
            m_rigidBody.isKinematic = false;

            m_rigidBody.AddExplosionForce(force, worldPosition, radius, 50, ForceMode.VelocityChange);
        }
    }
}
