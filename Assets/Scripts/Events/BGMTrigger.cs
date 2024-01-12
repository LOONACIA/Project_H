using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    #region PublicVariables
    [Header("BGM 사운드")]
    public SFXInfo bgm;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Actor>(out var actor1) || !actor1.IsPossessed)
        {
            return;
        }

        if (bgm.audio == GameManager.Sound.GetCureentBGM())
            return;
    
        GameManager.Sound.Play(bgm);
    }
    #endregion

    #region PrivateMethod
    #endregion
}
