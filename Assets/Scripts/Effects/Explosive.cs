using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Explosive : MonoBehaviour
{
    private Collider m_collider;

    private Rigidbody m_rigidBody;

    public void Explode(float force, Vector3 centerPosition, float radius, float upwardModifier = 10)
    {
        if (TryGetComponent<Collider>(out m_collider)
            && TryGetComponent<Rigidbody>(out m_rigidBody))
        { 
            var meshCollider = m_collider as MeshCollider;
            if (meshCollider != null)
            {
                meshCollider.convex = true;
            }

            m_collider.isTrigger = true;
            m_rigidBody.isKinematic = false;

            var direction = (transform.position - centerPosition).normalized;
            var randomVec = Random.insideUnitSphere * 0.1f;
            direction += randomVec;
            m_rigidBody.AddForce(direction * force, ForceMode.VelocityChange);
            m_rigidBody.AddExplosionForce(force, centerPosition, radius, upwardModifier);
        }

        Invoke(nameof(Destroy), Random.Range(5, 8));
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
