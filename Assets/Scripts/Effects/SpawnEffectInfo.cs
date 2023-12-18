using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class SpawnEffectInfo
{
    [SerializeField]
    private Renderer m_renderer;

    [SerializeField]
    private Material[] m_originMaterials;

    [SerializeField]
    private Material[] m_spawnMaterials;

    public Renderer Renderer => m_renderer;

    public Material[] GetOriginMaterials() => m_originMaterials.ToArray();
    
    public Material[] GetSpawnMaterials() => m_spawnMaterials.ToArray();

    public void Init()
    {
        if (m_originMaterials == null)
        {
            m_originMaterials = Renderer.sharedMaterials;
        }

        if (m_spawnMaterials == null)
        {
            m_spawnMaterials = m_originMaterials;
        }
    }
}
