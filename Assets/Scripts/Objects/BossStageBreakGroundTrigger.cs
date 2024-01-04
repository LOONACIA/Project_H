using LOONACIA.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossStageBreakGroundTrigger : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    [SerializeField]
    private BossStageRoot m_bossStageRoot;
    [SerializeField]
    private GameObject m_breakGround;

    private MeshRenderer m_mr;
    private MeshCollider m_col;
    private BoxCollider m_trigger;
    private Explosive[] m_explosiveList;
    #endregion

    #region PublicMethod
    private void Start()
    {
        m_mr = m_breakGround.GetComponent<MeshRenderer>();
        m_col = m_breakGround.GetComponent<MeshCollider>();
        m_trigger = GetComponent<BoxCollider>();
        
        m_explosiveList = m_breakGround.GetComponentsInChildren<Explosive>(true);
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

            if(TryGetComponent<AudioSource>(out var audioSource))
            {
                audioSource.Play();
            }
        }
    }

    private void BreakGround()
    {
        m_mr.enabled = false;
        m_col.enabled = false;

        foreach (var explosive in m_explosiveList)
        {
            explosive.gameObject.layer = LayerMask.NameToLayer("Default");
            explosive.gameObject.SetActive(true);
            explosive.Explode(200, transform.position + Vector3.down * 50, 100, 10);
        }
    }

    private void SetForce(Rigidbody rb)
    {   
        rb.gameObject.SetActive(true);
        rb.isKinematic = false;
        float x = Random.Range(-0.5f, 0.5f);
        float z = Random.Range(-0.5f, 0.5f);

        Vector3 dir = new Vector3(x, 1f, z).normalized;

        rb.AddForce(dir * 40f , ForceMode.Impulse);
    }
    #endregion
}
