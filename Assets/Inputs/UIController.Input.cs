using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class UIController
{
	private CharacterInputActions m_inputActions;

    private UIInputActionContext m_context;
    
    private void InitInput()
    {
        m_inputActions ??= ManagerRoot.Input.GetInputActions<CharacterInputActions>();
        m_context ??= new(this);
        
        m_inputActions.UI.SetCallbacks(m_context);
        EnableInput();
    }

    private void EnableInput()
    {
        m_inputActions.UI.Enable();
    }

    private void DisableInput()
    {
        m_inputActions.UI.Disable();
    }

    private class UIInputActionContext : CharacterInputActions.IUIActions
    {
        private readonly UIController m_controller;
        
        public UIInputActionContext(UIController controller)
        {
            m_controller = controller;
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            // Handled by EventSystem in Scene
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Pause();
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            // Handled by EventSystem in Scene
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            // Handled by EventSystem in Scene
        }
    }
}