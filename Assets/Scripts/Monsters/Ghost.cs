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

        StartCoroutine(CoPossess());
        //float speed = ConstVariables.HACKING_SUCCESS_EFFECT_DURATION;
        //Utility.Lerp(transform.position, m_target.transform.position, speed, position => transform.position = position, PossessEnd, true);
    }

    private IEnumerator CoPossess()
    {
        Vector3 from = transform.position;
        Vector3 to = m_target.transform.position;
        float speed = ConstVariables.HACKING_SUCCESS_EFFECT_DURATION;
        float time = 0f;
        while (time < speed)
        {
            if (!GameManager.Instance.IsPaused)
            {
                transform.position = Vector3.Lerp(from, to, time / speed);
                time += Time.unscaledDeltaTime;
            }

            yield return null;
        }
        
        transform.position = to;
        PossessEnd();
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

    public override void Ability(bool isToggled)
    {
    }

    public override void Dash(Vector3 direction)
    {
    }

    #endregion
    
    private IEnumerator CoDestroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ManagerRoot.Resource.Release(gameObject);
    }
}
