using LOONACIA.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossStageBreakGround : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    [SerializeField]
    private BossStageRoot m_bossStageRoot;
    private Transform[] m_childrenList;
    private MeshRenderer m_mr;
    private MeshCollider m_col;
    private BoxCollider m_trigger;
    #endregion

    #region PublicMethod
    private void Start()
    {
        m_mr = GetComponent<MeshRenderer>();
        m_col = GetComponent<MeshCollider>();
        m_trigger = GetComponent<BoxCollider>();

        m_childrenList = GetComponentsInChildren<Transform>(true);
    }
    #endregion

    #region PrivateMethod
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Monster>(out var monster)
            && monster.IsPossessed)
        { 
            m_trigger.enabled = false;
            BreakGround();
            m_bossStageRoot.StageStart();

            foreach (var child in m_childrenList)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    private void BreakGround()
    {
        m_mr.enabled = false;
        m_col.enabled = false;

        var children = GetComponentsInChildren<Rigidbody>(true);

        foreach (var child in children) 
        { 
            SetForce(child);
        }
    }

    private void SetForce(Rigidbody rb)
    {   
        rb.gameObject.SetActive(true);
        float x = Random.Range(-0.5f, 0.5f);
        float z = Random.Range(-0.5f, 0.5f);

        Vector3 dir = new Vector3(x, 1f, z).normalized;

        rb.AddForce(dir * 40f , ForceMode.Impulse);
    }
    #endregion
}
