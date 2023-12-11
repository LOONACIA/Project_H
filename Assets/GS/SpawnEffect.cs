using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    private readonly Dictionary<Renderer, Material[]> m_originMaterialDic = new();
    
    [Header("Renderer Info")]
    [SerializeField]
    private Material m_spawnMaterial;

    [SerializeField]
    private Renderer[] m_renderers;

    [Header("Solidify Data")]
    [SerializeField]
    private float m_solidifyProgressTime;

    [SerializeField]
    private float m_solidifyRate;

    [Header("Material Lerp Data")]
    [SerializeField]
    private float m_lerpProgressTime;

    private Actor m_actor;

    private bool m_isSpawned;

    private Coroutine m_spawnEffectCoroutine;

    private Coroutine m_solidifyCoroutine;

    private Coroutine m_lerpCoroutine;

    private void Awake()
    {
        m_actor = GetComponent<Actor>();
    }

    private void Start()
    {
        foreach (Renderer renderer in m_renderers)
        {
            m_originMaterialDic.Add(renderer, renderer.materials.ToArray());
        }
    }

    private void OnEnable()
    {
        m_actor.Spawned += OnSpawned;
    }

    private void OnDisable()
    {
        m_actor.Spawned -= OnSpawned;
    }

    // test
    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.Escape)) 
    //    {
    //        if (m_spawnEffectCoroutine != null)
    //            StopCoroutine(m_spawnEffectCoroutine); 
    //        m_spawnEffectCoroutine = StartCoroutine(SwitchMaterials());
    //    }
    //}

    public void Play()
    {
        if (m_isSpawned) return;
        if (m_renderers.Length <= 0) return;

        if (m_spawnEffectCoroutine != null)
            StopCoroutine(m_spawnEffectCoroutine);
        m_spawnEffectCoroutine = StartCoroutine(SwitchMaterials());
    }

    private IEnumerator SwitchMaterials()
    {
        m_isSpawned = true;

        // 원본 메테리얼에서 소환용 메테리얼로 변경
        foreach (Renderer renderer in m_renderers)
        {
            Material[] materials = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                materials[i] = Instantiate(m_spawnMaterial);
            }

            renderer.materials = materials;
        }

        // 소환용 메테리얼의 수치 변경 => 허공에서 몬스터가 생기는 연출
        if (m_solidifyCoroutine != null)
            StopCoroutine(m_solidifyCoroutine);
        yield return m_solidifyCoroutine = StartCoroutine(SolidifyMaterials());

        // 메테리얼 변경이 자연스럽게 
        //if (m_lerpCoroutine != null)
        //    StopCoroutine(m_lerpCoroutine);
        //yield return m_lerpCoroutine = StartCoroutine(LerpMaterials());

        // 소환용 메테리얼에서 원본 메테리얼로 변경
        foreach (Renderer renderer in m_renderers)
        {
            renderer.materials = m_originMaterialDic[renderer];
        }
    }

    /// <summary>
    /// 투명한 상태에서 모습이 나타나는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SolidifyMaterials()
    {
        float time = 0;

        while (time < m_solidifyProgressTime)
        {
            time += Time.deltaTime;

            float ratio = Mathf.Lerp(1, m_solidifyRate, time / m_solidifyProgressTime);

            foreach (Renderer renderer in m_renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetFloat("_DissolveAmount", ratio);
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// 소환용 메테리얼에서 원본 메테리얼로 자연스럽게 전환하기 위한 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpMaterials()
    {
        float time = 0;

        while (time < m_lerpProgressTime)
        {
            time += Time.deltaTime;

            float ratio = Mathf.Lerp(0, 1, time / m_lerpProgressTime);

            foreach (Renderer renderer in m_renderers)
            {
                Material[] currentMaterials = renderer.materials;
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    currentMaterials[i].Lerp(m_spawnMaterial, m_originMaterialDic[renderer][i], ratio);
                }

                renderer.materials = currentMaterials;
            }

            yield return null;
        }
    }

    private void OnSpawned(object sender, EventArgs e)
    {
        Play();
    }
}