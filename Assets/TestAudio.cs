using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AudioSource ss;
            ss = transform.GetComponent<AudioSource>();

            ss.Play();
        }
    }
    #endregion

    #region PrivateMethod
    #endregion
}
