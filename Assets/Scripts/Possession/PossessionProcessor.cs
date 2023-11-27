using System;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionProcessor : MonoBehaviour
{
    private static readonly int s_possess = Animator.StringToHash("Possess");

    [SerializeField]
    private GameObject m_ghostPrefab;

    [SerializeField]
    private LayerMask m_targetLayers;

    private Actor m_sender;

    // 빙의가 가능한지 여부 체크, 표창을 던질 지, 빙의를 할지를 판단함.
    public bool m_isAblePossession = false;
    
    public bool m_isHitTarget = false;

    public PossessionShuriken m_shuriken;

    /// <summary>
    /// 빙의 타겟 선정에 성공할 경우, 빙의 시작 시 발생하는 이벤트.
    /// </summary>
    public event EventHandler Possessing;

    /// <summary>
    /// 빙의 시 발생하는 이벤트. 성공하면 대상 몬스터가 전달되고, 실패하면 null이 전달됨.
    /// </summary>
    public event EventHandler<Actor> Possessed;

    /// <summary>
    /// 빙의 타겟에게 표창이 적중할 경우 발생하는 이벤트.
    /// </summary>
    public event EventHandler<float> TargetHit;

    /// <summary>
    /// 빙의 가능한 상태일 경우 발생하는 이벤트.
    /// </summary>
    public event EventHandler Possessable;

    public void TryPossess(Actor sender)
    {
        m_sender = sender;
        IAnimationEventReceiver receiver = m_sender.GetComponentInChildren<IAnimationEventReceiver>();
        if (receiver != null)
        {
            receiver.SetPossession(this);
        }

        //표창이 박혀있는지 체크
        // TODO: 풀링 시 shuriken null check 로직 수정 필요
        if (!m_isHitTarget || m_shuriken == null)
        {
            m_sender.Animator.SetTrigger(s_possess);
            return;
        }

        //표창이 박혀있는데 빙의가 아직 불가능하면 return
        if (!m_isAblePossession)
            return;

        //표창이 박혔을 시, 빙의 시작
        m_isAblePossession = false;
        m_isHitTarget = false;
        
        PossessTarget();

        ManagerRoot.Resource.Release(m_shuriken.gameObject);
        //m_shuriken.DestroyShuriken();
    }

    //표창이 박힌 타겟에게 빙의
    public void PossessTarget()
    {
        var position = m_sender.FirstPersonCameraPivot.transform.position;
        var ghostObj = ManagerRoot.Resource.Instantiate(m_ghostPrefab, position, m_sender.transform.rotation);
        if (!ghostObj.TryGetComponent<Ghost>(out var ghost))
        {
            throw new InvalidOperationException($"{ghostObj.name} does not have {nameof(Ghost)} component.");
        }

        Possessing?.Invoke(this, EventArgs.Empty);

        ghost.PossessToTarget(m_shuriken.targetActor, () => OnPossessed(m_shuriken.targetActor));
    }

    public void OnPossessAnimStart()
    {
        //StopTime();
        //GameManager.Effect.ShowPreparePossessionEffect();

        ThrowShuriken();
    }

    public void OnPossessAnimEnd()
    {
        //OnPossessed(null);

        // 그 전 코드

        //GameManager.Effect.ShowBeginPossessionEffect();
        //GameObject target = RayToTarget();
        //if (target == null || !target.TryGetComponent<Actor>(out var actor))
        //{
        //    GameManager.Effect.ShowPossessionFailEffect();
        //    OnPossessed(null);
        //    return;
        //}

        //var position = m_sender.FirstPersonCameraPivot.transform.position;
        //var ghostObj = ManagerRoot.Resource.Instantiate(m_ghostPrefab, position, m_sender.transform.rotation);
        //if (!ghostObj.TryGetComponent<Ghost>(out var ghost))
        //{
        //    throw new InvalidOperationException($"{ghostObj.name} does not have {nameof(Ghost)} component.");
        //}

        //Possessing?.Invoke(this, EventArgs.Empty);

        //ghost.PossessToTarget(actor, () => OnPossessed(actor));
    }

    /// <summary>
    /// 빙의를 할 타겟을 선정하는 함수
    /// </summary>
    private GameObject RayToTarget()
    {
        var cameraPivot = m_sender.FirstPersonCameraPivot;

        Debug.DrawRay(cameraPivot.transform.position, cameraPivot.transform.forward * 300f, Color.red, 3f);
        return Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out var hit, 300f, m_targetLayers)
            ? hit.transform.gameObject
            : null;
    }

    private void StartTime()
    {
        Time.timeScale = 1f;
    }

    private void StopTime()
    {
        Time.timeScale = 0f;
    }

    private void OnPossessed(Actor actor)
    {
        Possessed?.Invoke(this, actor);
        StartTime();
    }

    #region 표창 날리기
    public void ThrowShuriken()
    {
        var cameraPivot = GameManager.Camera.CurrentCamera;

        bool isHit = Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out var hit, 300f);

        m_shuriken = Instantiate(m_sender.Data.ShurikenObj, cameraPivot.transform.position + cameraPivot.transform.forward, Quaternion.identity).GetComponent<PossessionShuriken>();

        // Ray를 쏜 곳에 몬스터가 있을 시,
        if (isHit && 1 << hit.transform.gameObject.layer == m_targetLayers)
        {
            m_shuriken.InitSetting(hit.transform.GetComponent<Actor>(), m_sender, OnTargetHit);
        }
        else
        {
            m_shuriken.InitSetting(cameraPivot.transform.forward, m_sender, OnTargetHit);
        }
    }

    private void OnTargetHit(Actor actor)
    {
        m_isHitTarget = true;
        TargetHit?.Invoke(this, actor.Data.PossessionRequiredTime);
        TryHacking(actor);
    }

    private void TryHacking(Actor actor)
    {
        StartCoroutine(CoWaitForPossession(actor.Data.PossessionRequiredTime));
    }
    
    private IEnumerator CoWaitForPossession(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_isAblePossession = true;
        Possessable?.Invoke(this, EventArgs.Empty);
    }
    
    #endregion
}