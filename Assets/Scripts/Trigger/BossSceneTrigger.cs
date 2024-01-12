using LOONACIA.Unity.Managers;
using UnityEngine;

public class BossSceneTrigger : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    #endregion

    #region PrivateMethod
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Character.Controller.Character.gameObject)
        {
            ManagerRoot.Input.Disable<CharacterInputActions>();
            _ = SceneHelper.LazyLoadAsync(nameof(SceneName.Stage3));
        }
    }
    #endregion
}
