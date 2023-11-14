using System;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
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
    [SerializeField]
    private bool m_isAblePossession = false;

    /// <summary>
    /// 빙의 타겟 선정에 성공할 경우, 빙의 시작 시 발생하는 이벤트.
    /// </summary>
    public event EventHandler Possessing; 

    /// <summary>
    /// 빙의 시 발생하는 이벤트. 성공하면 대상 몬스터가 전달되고, 실패하면 null이 전달됨.
    /// </summary>
    public event EventHandler<Actor> Possessed;

    public void TryPossess(Actor sender)
    {   
        //표창이 박혀있지 않을 시, 해킹을 하지 않고 표창을 날림.
        if(m_isAblePossession == false)
        {
            ThrowShuriken();
            return;
        }

        m_sender = sender;
        IAnimationEventReceiver receiver = m_sender.GetComponentInChildren<IAnimationEventReceiver>();
        if (receiver != null)
        {
            receiver.SetPossession(this);
        }

        m_sender.Animator.SetTrigger(s_possess);
    }

    public void OnPossessAnimStart()
    {
        StopTime();
        GameManager.Effect.ShowPreparePossessionEffect();
    }

    public void OnPossessAnimEnd()
    {
        GameManager.Effect.ShowBeginPossessionEffect();
        GameObject target = RayToTarget();
        if (target == null || !target.TryGetComponent<Actor>(out var actor))
        {
            GameManager.Effect.ShowPossessionFailEffect();
            OnPossessed(null);
            return;
        }

        var position = m_sender.FirstPersonCameraPivot.transform.position;
        var ghostObj = ManagerRoot.Resource.Instantiate(m_ghostPrefab, position, m_sender.transform.rotation);
        if (!ghostObj.TryGetComponent<Ghost>(out var ghost))
        {
            throw new InvalidOperationException($"{ghostObj.name} does not have {nameof(Ghost)} component.");
        }
        
        Possessing?.Invoke(this, EventArgs.Empty);
        
        ghost.PossessToTarget(actor, () => OnPossessed(actor));
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
        
    }
    #endregion
}