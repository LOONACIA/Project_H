using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossStagePhaseTrigger : MonoBehaviour, IActivate
{
    [SerializeField, Tooltip("BossPhase 시작되면 활성화 될 지면 오브젝트 목록")]
    private GameObject[] m_groundObjectList;

    private List<Collider> m_groundColliderList = new();

    private Collider m_collider;

    private bool m_isActive;
   
    private void Start()
    {
        TryGetComponent<Collider>(out m_collider);
        m_collider.enabled = false;

        foreach (var groundObject in m_groundObjectList)
        {
            m_groundColliderList.AddRange(groundObject.GetComponentsInChildren<Collider>(true));
        }

        foreach (var groundCollier in m_groundColliderList)
        {
            groundCollier.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_isActive) return;

        if (other.gameObject.TryGetComponent<Monster>(out var montser) && montser.IsPossessed)
        {
            // TODO : 메테리얼 변경되는 효과

            foreach (var groundCollier in m_groundColliderList) 
            {
                groundCollier.enabled = true;
            }
        }
    }

    public void Activate()
    {
        if (m_isActive) return;

        m_isActive = true;

        m_collider.enabled = true;
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

}
