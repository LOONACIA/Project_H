using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using LOONACIA.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionShuriken : MonoBehaviour
{
    #region PublicVariables
    PossessionProcessor processor;
    public Actor targetActor;
    #endregion

    #region PrivateVariables
    // 몬스터를 추적하는지, 아니면 그냥 날아가는지
    private bool isTrace = false;
    private bool isStop = false;

    private Rigidbody m_rb;
    
    // 움직여야될 목표 위치
    private Vector3 m_targetDir;

    private int m_targetLayer;

    [SerializeField] private float m_speed;
    #endregion

    #region PublicMethod
    private void Awake()
    {
        TryGetComponent<Rigidbody>(out m_rb);
        m_targetLayer = LayerMask.GetMask("Monster");
    }

    public void InitSetting(Actor _actor, PossessionProcessor _processor)
    {
        targetActor = _actor;
        isTrace = true;
        processor = _processor;
    }

    public void InitSetting(Vector3 _dir, PossessionProcessor _processor)
    {
        m_targetDir = _dir;
        processor = _processor;
    }

    public void DestroyShuriken()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if(isTrace)
        {
            m_targetDir = ((targetActor.transform.position + new Vector3(0,2,0)) - transform.position).normalized;
        }

        Move();
    }
    #endregion

    #region PrivateMethod
    private void Move()
    {   
        if(isStop)
        {
            return;
        }

        m_rb.MovePosition(transform.position + m_targetDir * m_speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(1 << other.gameObject.layer == m_targetLayer)
        {
            processor.m_isAblePossession = true;
            targetActor = other.gameObject.GetComponent<Actor>();   
            GetComponent<Collider>().enabled = false;
            isStop = true;
        }
        else
        {
            Debug.Log("부서진다.." + other.gameObject.layer);
            Destroy(gameObject);
        }
    }
    #endregion
}
