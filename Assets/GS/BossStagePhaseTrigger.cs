using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossStagePhaseTrigger : MonoBehaviour, IActivate
{
    [SerializeField, Tooltip("BossPhase 시작되면 활성화 될 지면 오브젝트 목록")]
    private Collider[] m_groundObjectList;

    [SerializeField, Tooltip("플레이어가 착지할 곳을 보여줄 오브젝트의 목록")]
    private Renderer[] m_landingPointList;

    //private List<Collider> m_groundColliderList = new();

    private Collider m_collider;

    private bool m_isActive;
   
    private void Start()
    {
        TryGetComponent<Collider>(out m_collider);
        m_collider.enabled = false;

        //foreach (var groundObject in m_groundObjectList)
        //{
        //    m_groundColliderList.AddRange(groundObject.GetComponentsInChildren<Collider>(true));
        //}

        //foreach (var groundCollier in m_groundColliderList)
        //{
        //    groundCollier.enabled = false;
        //}

        foreach (var landingPoint in m_landingPointList) 
        {
            landingPoint.enabled = false;
            landingPoint.material = new(landingPoint.material);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_isActive) return;

        if (other.gameObject.TryGetComponent<Monster>(out var montser) && montser.IsPossessed)
        {
            Deactivate();

            StartCoroutine(Dissolve());
            
            foreach (var groundCollier in m_groundObjectList) 
            {
                groundCollier.gameObject.SetActive(true);
                groundCollier.enabled = true;
            }
        }
    }

    public void Activate()
    {
        if (m_isActive) return;

        m_isActive = true;

        m_collider.enabled = true;

        foreach (var landingPoint in m_landingPointList)
        {
            landingPoint.enabled = true;
        }
    }

    public void Deactivate()
    {
        if (!m_isActive) return;

        m_isActive = false;
    }

    public bool IsInArea(Vector3 target)
    {
        return m_collider.bounds.Contains(target);
    }

    private IEnumerator Dissolve()
    {
        float progressTime = 0.5f;
        float time = 0;

        while (time < progressTime) 
        {
            time += Time.deltaTime;

            foreach (var landingPoint in m_landingPointList)
            {
                landingPoint.material.SetFloat("_DissolveHide", Mathf.Lerp(-1, 1, time / progressTime));
            }

            yield return null;
        }
    }
}
