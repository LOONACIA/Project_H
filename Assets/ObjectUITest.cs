using UnityEngine;

public class ObjectUITest : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    #endregion

    #region PrivateMethod
    private void OnCollisionEnter(Collision collision)
    {
        GameManager.UI.UpdateObject("오브젝트 업데이트!");
    }
    #endregion
}
