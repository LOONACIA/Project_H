using UnityEngine;

public class DetectionCone : MonoBehaviour
{
    private float m_coneAngle;

    private Light m_light;
    
    private void Awake()
    {
        m_light = GetComponent<Light>();
    }

    private void Start()
    {
        float angle = m_light.spotAngle / 2f;
        m_coneAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
    }

    public bool IsInCone(Vector3 targetPosition)
    {
        float angle = m_light.spotAngle / 2f;
        m_coneAngle = Mathf.Cos(angle * Mathf.Deg2Rad);
        var direction = (targetPosition - transform.position).normalized;
        return Vector3.Dot(transform.forward, direction) >= m_coneAngle;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
        {
            Debug.Log(IsInCone(actor.transform.position));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_light == null)
        {
            m_light = GetComponent<Light>();
        }

        float angle = m_light.spotAngle / 2f;
        int length = 10;
        // Draw Cone Gizmos
        Vector3 direction = transform.forward;
        Vector3 right = Quaternion.Euler(0, angle, 0) * direction;
        Vector3 left = Quaternion.Euler(0, -angle, 0) * direction;
        Vector3 up = Quaternion.Euler(-angle, 0, 0) * direction;
        Vector3 down = Quaternion.Euler(angle, 0, 0) * direction;
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, right * length);
        Gizmos.DrawRay(transform.position, left * length);
        Gizmos.DrawRay(transform.position, up * length);
        Gizmos.DrawRay(transform.position, down * length);
        
        // Draw circle in front of the cone
        Gizmos.color = Color.green;
        float radius = Mathf.Tan(angle * Mathf.Deg2Rad) * length;
        Gizmos.DrawWireSphere(transform.position + direction * length, radius);
    }
}
