using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLightEffect : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void OnEnable()
    {
        StartCoroutine(IE_ReleaseSelf());
    }
    #endregion

    #region PrivateMethod
    private IEnumerator IE_ReleaseSelf()
    {   
        yield return new WaitForSeconds(0.1f);
        ManagerRoot.Resource.Release(gameObject);
    }
    #endregion
}
