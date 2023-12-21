using UnityEngine;

public class ObjectUITest : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GameManager.Effect.ShowDashEffect();
        }
    }
    #endregion

    #region PrivateMethod

    #endregion
}
