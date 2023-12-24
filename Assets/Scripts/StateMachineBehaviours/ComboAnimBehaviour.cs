using UnityEngine;

public class ComboAnimBehaviour : StateMachineBehaviour
{
    public int myCount;
    
    private static readonly int s_attackCountAnimationHash = Animator.StringToHash("AttackCount");
    
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        animator.SetInteger(s_attackCountAnimationHash,myCount);
    }
}
