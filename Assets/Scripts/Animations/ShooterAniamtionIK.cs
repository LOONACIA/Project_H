using RenownedGames.AITree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAniamtionIK : MonoBehaviour
{
    #region PublicVariables
    public Animator animator;

    public Transform targetObj;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //OnAnimatorIK();
    }

    private void OnAnimatorIK()
    {
        if (animator)
        {
            if(targetObj != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, targetObj.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, targetObj.rotation);
            }
        }
    }
    #endregion

    #region PrivateMethod
    #endregion
}
