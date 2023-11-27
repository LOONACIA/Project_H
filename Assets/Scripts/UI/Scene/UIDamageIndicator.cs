using LOONACIA.Unity.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDamageIndicator : UIScene
{
    #region PublicVariables
    public GameObject indicatorParent;
    public GameObject damageIndicator;
    #endregion

    #region PrivateVariables
    private CharacterController m_characterController;
    #endregion

    #region PublicMethod
    private void Start()
    {
        m_characterController = GameObject.Find("@Character").GetComponent<CharacterController>();
        RegisterEvents();
    }
    #endregion

    #region PrivateMethod
    private void RegisterEvents()
    {
        m_characterController.Damaged += OnCharacterDamaged;
    }

    private void OnCharacterDamaged(object sender, DamageInfo e)
    {
      DamageIndicator indi = Instantiate(damageIndicator, indicatorParent.transform).GetComponent<DamageIndicator>();

        indi.Init(e, m_characterController.Character);
    }
    #endregion
}
