using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DissolveGateSizeSetting : MonoBehaviour
{
    private Renderer m_renderer;

    private void Start()
    {
        InitSetting();
    }

    private void OnEnable()
    {
        InitSetting();
    }

    public void InitSetting()
    {
        if (m_renderer == null)
            TryGetComponent<Renderer>(out m_renderer);

        if (m_renderer == null) return;

        var dotsTiling = new Vector2(transform.lossyScale.x, transform.lossyScale.y) * 2;

        m_renderer.material = new(m_renderer.material);
        
        m_renderer.material.SetFloat("_DotsTilingX", dotsTiling.x);
        m_renderer.material.SetFloat("_DotsTilingY", dotsTiling.y);
    }
}
