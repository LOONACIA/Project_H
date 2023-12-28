using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMStopTrigger : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethodw
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Actor>(out var actor1) || !actor1.IsPossessed)
        {
            return;
        }

        GameManager.Sound.BGMOff();
    }
    #endregion

    #region PrivateMethod
    #endregion
}
