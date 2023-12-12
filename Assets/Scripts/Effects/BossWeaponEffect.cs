using UnityEngine;

public class BossWeaponEffect : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer m_trailRenderer;

    private void Awake()
    {
        m_trailRenderer.emitting = false;
    }

    public void OnAttackHit()
    {
        m_trailRenderer.emitting = true;
    }

    public void OnAttackFollowThrough()
    {
        m_trailRenderer.emitting = false;
    }
}
