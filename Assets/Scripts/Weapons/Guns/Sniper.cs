using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Sniper : Gun
{
    private static readonly int s_emissionColorID = Shader.PropertyToID("_EmissionColor");
    
    [Header("Sniper")]
    [SerializeField]
    private float m_maxDistance = 100f;
    
    [SerializeField]
    private Color m_shootLineColor;

    [SerializeField]
    private Color m_laserColor;
    
    private LineRenderer m_renderer;
    
    private CoroutineEx m_drawLineCoroutine;

    private float m_intensity = 1f;

    private float m_intensityMul = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        
        m_renderer = GetComponent<LineRenderer>();
    }
    
    protected void OnDisable()
    {
        m_drawLineCoroutine?.Abort();
    }

    private void OnDestroy()
    {
        m_drawLineCoroutine?.Abort();
    }

    protected override void Fire()
    {
        Ray ray = GetRay();

        var hits = Physics.RaycastAll(ray, m_maxDistance, m_aimLayers)
            .OrderBy(hit => hit.distance)
            .ToArray();

        RaycastHit end = hits.FirstOrDefault(hit => hit.transform.gameObject.layer != m_damageLayer);

        var targets = hits.TakeWhile(hit => hit.transform.gameObject.layer == m_damageLayer);

        Hit(ProcessHit(targets));

        Vector3 target;
        if (hits.Length > 0)
        {
            target = end.point != default ? end.point : hits[^1].point;
        }
        else
        {
            target = ray.GetPoint(m_maxDistance);
        }
        
        DrawLine(target);
    }

    protected override void OnTargetChanged()
    {
        base.OnTargetChanged();
        UpdateLine();
    }
    
    private IEnumerable<AttackInfo> ProcessHit(IEnumerable<RaycastHit> hits)
    {
        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent(out Actor actor))
            {
                yield return new(Owner.gameObject, actor.Health, Damage, hit.point, -hit.normal);
            }
        }
    }
    
    private void DrawLine(Vector3 target)
    {
        m_renderer.positionCount = 2;
        m_renderer.SetPosition(0, m_spawnPosition.position);
        m_renderer.SetPosition(1, target);

        //Color c = m_renderer.material.GetColor("_EmissionColor");
        
        m_renderer.material.SetColor(s_emissionColorID, m_shootLineColor * 1500f);

        m_drawLineCoroutine = Utility.Lerp(0.2f, 0f, 0.5f, value => m_renderer.startWidth = m_renderer.endWidth = value, () => m_intensity = 1f);
    }
    
    private void UpdateLine()
    {
        if (Target == default)
        {
            m_renderer.positionCount = 0;
            return;
        }
        
        Vector3 direction = (Target - m_virtualCamera.transform.position).normalized;
        Vector3 end = Target;
        if (Physics.Raycast(m_virtualCamera.transform.position, direction, out var hit, m_maxDistance, m_aimLayers))
        {
            end = hit.point;
        }
        
        m_renderer.positionCount = 2;
        m_renderer.SetPosition(0, m_spawnPosition.position);
        m_renderer.SetPosition(1, end);
        m_renderer.startWidth = m_renderer.endWidth = 0.005f;

        m_intensity += m_intensityMul;
        m_renderer.material.SetColor(s_emissionColorID, m_laserColor * m_intensity);
    }
}
