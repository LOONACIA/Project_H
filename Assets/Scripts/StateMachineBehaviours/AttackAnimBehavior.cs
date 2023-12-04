using System.Collections.Generic;
using UnityEngine;

public class AttackAnimBehavior : StateMachineBehaviour
{
    [SerializeField]
    private List<float> m_animProbability;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(ConstVariables.ANIMATOR_PARAMETER_ATTACK_INDEX, GetAnimIndex());
    }

    private float TotalProbability()
    {
        float probabilityTotal = 0;

        foreach (var anim in m_animProbability)
        {
            probabilityTotal += anim;
        }

        return probabilityTotal;
    }

    private int GetAnimIndex()
    {
        float randomNum = Random.Range(0, TotalProbability());

        for (int i = 0; i < m_animProbability.Count; i++)
        { 
            randomNum -= m_animProbability[i];

            if (randomNum <= 0)
                return i;
        }

        return 0;
    }
}
