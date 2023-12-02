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
}
