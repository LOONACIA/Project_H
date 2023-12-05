using UnityEngine;

/// <summary>
/// 이 State로 들어왔을 때는 Animator의 Culling Mode가 Always Animate로 설정됩니다.
/// 공격 등 시야에 들어오지 않았을 때도 애니메이션이 재생되어야 할 때 사용합니다.
/// </summary>
public class CullingOffStateBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //이 State를 여러 Behaviour에서 사용할 수 있으므로 Update에서 계속 바꾸어줍니다.
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
