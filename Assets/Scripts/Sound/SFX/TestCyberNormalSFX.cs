using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCyberNormalSFX : MonoBehaviour
{
    #region PublicVariables
    [SerializeField] private AudioSource m_combo1;
    [SerializeField] private AudioSource m_combo2;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    public void PlayCombo1SFX()
    {
        PlaySFX(m_combo1);
    }

    public void PlayCombo2SFX()
    {
        PlaySFX(m_combo2);
    }
    #endregion

    #region PrivateMethod
    private void PlaySFX(AudioSource _sfx)
    {
        _sfx.Play();
    }

    #endregion
}
