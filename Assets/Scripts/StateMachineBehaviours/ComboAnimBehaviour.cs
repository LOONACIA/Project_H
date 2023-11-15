using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ComboAnimBehaviour : StateMachineBehaviour
{
    public static readonly int s_attackCountAnimationHash = Animator.StringToHash("AttackCount");
    public int myCount = 0;

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        animator.SetInteger(s_attackCountAnimationHash,myCount);
    }
}
