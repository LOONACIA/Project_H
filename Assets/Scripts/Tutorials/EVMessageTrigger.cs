using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class EVMessageTrigger : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    [SerializeField]
    private InputActionReference m_inputDashAction;
    [SerializeField] 
    private InputActionReference m_inputSkillAction;

    private bool m_isTriggered;
    #endregion

    #region PublicMethod
    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    public void ShowSkillDashObject()
    {
        m_isTriggered = true;
        UpdateText();
    }

    public void UpdateText()
    {
        // If not triggered
        if (!m_isTriggered)
        {
            // Ignore
            return;
        }

        string path1 = m_inputDashAction.action.bindings
            .SingleOrDefault(binding => binding.groups.Equals(ManagerRoot.Input.CurrentControlScheme))
            .ToDisplayString();
        string path2 = m_inputSkillAction.action.bindings
            .SingleOrDefault(binding => binding.groups.Equals(ManagerRoot.Input.CurrentControlScheme))
            .ToDisplayString();

        GameManager.UI.UpdateObject($"{path1}는 대쉬, {path2}는 스킬을 \n 사용할 수 있습니다.");

        m_isTriggered = false;
    }


    #endregion

    #region PrivateMethod
    private void OnActionChange(object arg1, InputActionChange arg2)
    {
        UpdateText();
    }
    #endregion
}
