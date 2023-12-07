using System;
using Cinemachine;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System.Collections;
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
        
        transform.LookAt(target.FirstPersonCameraPivot.transform.position);
        
        // TODO: speed 계산
        float speed = ConstVariables.HACKING_SUCCESS_EFFECT_DURATION;
        Utility.Lerp(transform.position, m_target.transform.position, speed, position => transform.position = position, PossessEnd, true);
    }

    private void PossessEnd()
    {
        GameManager.Effect.ShowPossessionSuccessEffect();
        GameManager.Camera.CurrentCamera = m_target.gameObject.FindChild<CinemachineVirtualCamera>();

        // 대상 몬스터로 스위칭
        m_onPossessed?.Invoke();
        
        // Release gameObject
        StartCoroutine(CoDestroy(ConstVariables.MONSTER_DESTROY_WAIT_TIME));
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

    public override void Dash(Vector3 direction)
    {
    }

    public override void Block(bool value)
    {
    }
    
    private IEnumerator CoDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ManagerRoot.Resource.Release(gameObject);
    }

    #endregion
}
