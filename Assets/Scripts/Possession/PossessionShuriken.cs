using System;
using System.Collections;
using UnityEngine;

public class PossessionShuriken : MonoBehaviour
{
    #region PublicVariables

    // 맞은 객체
    public Actor targetActor;

    // 던진 객체
    public Actor throwActor;

    #endregion

    #region PrivateVariables

    // 몬스터를 추적하는지, 아니면 그냥 날아가는지
    private bool m_isTrace = false;

    private bool m_isStop;

    private Rigidbody m_rb;

    // 움직여야될 목표 위치
    private Vector3 m_targetDir;

    private int m_targetLayer;

    [SerializeField]
    private float m_speed;

    private Action<Actor> m_onTargetHit;

    private int m_surikenStopLayer;
    #endregion

    #region PublicMethod

    private void Awake()
    {
        TryGetComponent<Rigidbody>(out m_rb);
        m_targetLayer = LayerMask.GetMask("Monster");
        m_surikenStopLayer = LayerMask.GetMask("Wall", "Ground", "Monster", "Shield");

        StartCoroutine(nameof(IE_Destory));
    }

    public void InitSetting(Actor actor, Actor sender, Action<Actor> onTargetHit)
    {
        targetActor = actor;
        m_isTrace = true;
        throwActor = sender;
        m_onTargetHit = onTargetHit;
    }

    public void InitSetting(Vector3 dir, Actor sender, Action<Actor> onTargetHit)
    {
        m_targetDir = dir;
        throwActor = sender;
        m_onTargetHit = onTargetHit;
    }

    private void FixedUpdate()
    {
        //Target에 꽂혀있을 때
        if (m_isStop)
        {
            return;
        }

        if (m_isTrace)
        {
            m_targetDir = (targetActor.transform.position + Vector3.up - transform.position).normalized;
        }
        CheckBack();
        Move();
        
    }

    #endregion

    #region PrivateMethod

    private void CheckBack()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position - (m_targetDir.normalized * 0.3f), 0.3f, m_targetDir.normalized, out hit, 0.5f, m_surikenStopLayer) == false)
            return;

        Transform other = hit.transform;

        if (1 << other.gameObject.layer == m_targetLayer)
        {
            if (other.gameObject == throwActor.gameObject)
            {
                return;
            }

            targetActor = other.gameObject.GetComponent<Actor>();
            HitTarget();
        }
        else if ((m_surikenStopLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (m_isTrace)
                return;

            m_isStop = true;
            m_rb.isKinematic = true;

            GetComponent<Collider>().enabled = false;
            Invoke(nameof(DestroySelf), 5f);
        }
    }

    private void Move()
    {
        if (m_isStop)
        {
            return;
        }

        m_rb.MovePosition(transform.position + m_targetDir * (m_speed * Time.fixedDeltaTime));
    }

    private void HitTarget()
    {
        StopCoroutine(nameof(IE_Destory));
        m_isStop = true;
        m_rb.isKinematic = true;

        GetComponent<Collider>().enabled = false;

        transform.SetParent(targetActor.transform);
        OnTargetHit(targetActor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == m_targetLayer)
        {
            if (other.gameObject == throwActor.gameObject)
            {
                return;
            }

            targetActor = other.gameObject.GetComponent<Actor>();
            HitTarget();
        }
        else if ((m_surikenStopLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (m_isTrace)
                return;

            m_isStop = true;
            m_rb.isKinematic = true;

            GetComponent<Collider>().enabled = false;
            Invoke(nameof(DestroySelf), 5f);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTargetHit(Actor actor)
    {
        m_onTargetHit?.Invoke(actor);
    }

    private IEnumerator IE_Destory()
    {
        yield return new WaitForSeconds(ConstVariables.SHURIKEN_COOLTIME);

        DestroySelf();
    }
    #endregion
}