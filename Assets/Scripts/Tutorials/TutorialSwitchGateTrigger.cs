using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSwitchGateTrigger : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public void ShowCheckHPObject()
    {
        GameManager.UI.UpdateObject("좌측 상단에서 HP를 확인하세요");
    }

    public void ShowCheckHackingGauge()
    {
        GameManager.UI.UpdateObject("화면 중간의 해킹 게이지를 \r\n확인하세요");
    }

    public void ShowSwtichGateTrigger()
    {
        GameManager.UI.UpdateObject("스위치를 찾아 문을 열고 나가세요");
    }

    public void ShowCheckSavePoint()
    {
        GameManager.UI.UpdateObject("초록색의 저장 포인트가서 \r\n 위치를 저장해두세요");
    }
    #endregion

    #region PrivateMethod
    #endregion
}
