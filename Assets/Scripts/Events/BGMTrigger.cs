using System.Collections;
using System.Collections.Generic;
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
        if (bgm.audio == GameManager.Sound.GetCureentBGM())
            return;

        GameManager.Sound.Play(bgm);
    }
    #endregion

    #region PrivateMethod
    #endregion
}
