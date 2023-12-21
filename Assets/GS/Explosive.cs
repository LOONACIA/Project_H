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

    public void Explode(float force, Vector3 centerPosition, float radius)
    {
        if (TryGetComponent<Collider>(out m_collider)
            && TryGetComponent<Rigidbody>(out m_rigidBody))
        { 
            m_collider.isTrigger = true;
            m_rigidBody.isKinematic = false;

            var direction = (transform.position - centerPosition).normalized;
            var randomVec = Random.insideUnitSphere * 0.1f;
            direction += randomVec;
            m_rigidBody.AddForce(direction * force, ForceMode.VelocityChange);

            // temp
            m_rigidBody.AddExplosionForce(100, centerPosition, 100, 100);
        }
    }
}
