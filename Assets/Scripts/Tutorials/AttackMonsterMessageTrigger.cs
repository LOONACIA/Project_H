using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMonsterMessageTrigger : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public void ShowKillMonsterObject()
    {
        GameManager.UI.UpdateObject("경비로봇에 빙의해서 적을 처치하세요");
    }

    public void ShowNextRoomOjbect()
    {
        GameManager.UI.UpdateObject("문을 열고 이동하세요");
    }
    #endregion

    #region PrivateMethod
    #endregion
}
