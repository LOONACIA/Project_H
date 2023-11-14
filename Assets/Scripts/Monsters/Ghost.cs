using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using LOONACIA.Unity;
using UnityEngine;

public class Ghost : Actor
{
    private GameObject m_target;

    private Action m_onPossessed;
    
    public void PossessToTarget(Actor target, Action onPossessed = null)
    {
        m_onPossessed = onPossessed;
        m_target = target.FirstPersonCameraPivot;
        GameManager.Camera.CurrentCamera = gameObject.FindChild<CinemachineVirtualCamera>();
        GameManager.Effect.ShowPossessionStartEffect();
        
        // TODO: speed 계산
        float speed = 1f;
        Utility.Lerp(transform.position, m_target.transform.position, speed, position => transform.position = position, PossessEnd, true);
    }

    private void PossessEnd()
    {
        GameManager.Effect.ShowPossessionSuccessEffect();
        GameManager.Camera.CurrentCamera = m_target.gameObject.FindChild<CinemachineVirtualCamera>();

        // 대상 몬스터로 스위칭
        m_onPossessed?.Invoke();
    }

    #region Actor Implementations

    public override void Move(Vector3 direction)
    {
    }

    public override void TryJump()
    {
    }

    public override void TryAttack()
    {
    }

    public override void Skill()
    {
    }

    public override void Dash()
    {
    }

    public override void Block(bool value)
    {
    }

    #endregion
}
