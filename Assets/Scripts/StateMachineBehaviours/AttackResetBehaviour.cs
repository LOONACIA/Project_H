using UnityEngine;


/// <summary>
/// 3인칭 넉백, 스턴 등 공격 State를 초기화해주는 이벤트가 필요할 때 사용됩니다.
/// </summary>
public class AttackResetBehaviour : StateMachineBehaviour
{
    private IEventProxy m_eventProxy;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (m_eventProxy == null)
        {
            m_eventProxy = animator.GetComponent<IEventProxy>();
        }

        //공격 무기에 IDLE 시그널 보냄
        m_eventProxy.DispatchEvent($"On{nameof(WeaponState.Idle)}");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_eventProxy == null)
        {
            m_eventProxy = animator.GetComponent<IEventProxy>();
        }

        //공격 무기에 IDLE 시그널 보냄
        m_eventProxy.DispatchEvent($"On{nameof(WeaponState.Idle)}");
    }
}
