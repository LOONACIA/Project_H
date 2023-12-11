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

    public Material[] OriginMaterials
    {
        get
        {
            return m_originMaterials.ToArray();
        }
        set
        {
            m_originMaterials = value;
        }
    }

    public Material[] SpawnMaterials 
    {
        get
        { 
            return m_spawnMaterials.ToArray();
        }
        private set
        { 
            m_spawnMaterials = value;
        }
    }

    public void Init()
    {
        if (OriginMaterials == null) 
            OriginMaterials = Renderer.sharedMaterials;  
        
        if (SpawnMaterials == null)
            SpawnMaterials = OriginMaterials;
    }
}
