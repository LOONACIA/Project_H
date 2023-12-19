using LOONACIA.Unity.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController
{
    private CharacterInputActions m_inputActions;

    private CharacterInputActionContext m_context;
    
    private void InitInput()
    {
        m_inputActions ??= ManagerRoot.Input.GetInputActions<CharacterInputActions>();
        m_context ??= new(this);
        
        m_inputActions.Character.SetCallbacks(m_context);
        EnableInput();
    }

    private void EnableInput()
    {
        m_inputActions.Character.Enable();
    }

    private void DisableInput()
    {
        m_inputActions.Character.Disable();
    }

    private class CharacterInputActionContext : CharacterInputActions.ICharacterActions
    {
        private readonly PlayerController m_controller;
        
        public CharacterInputActionContext(PlayerController controller)
        {
            m_controller = controller;
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            m_controller.m_directionInput = new(input.x, 0, input.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Jump();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Attack();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Dash();
            }
        }
        
        public void OnPossess(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Possess();
            }
        }

        public void OnHack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Hack();
            }
        }

        public void OnAbility(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Ability();
            }
            // switch (context.phase)
            // {
            //     case InputActionPhase.Started:
            //         m_controller.Block(true);
            //         break;
            //     case InputActionPhase.Performed:
            //         break;
            //     case InputActionPhase.Canceled:
            //         m_controller.Block(false);
            //         break;
            // }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    m_controller.Interact();
                    break;
                case InputActionPhase.Canceled:
                    m_controller.AbortInteract();
                    break;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            m_controller.m_lookDelta = context.ReadValue<Vector2>();
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_controller.Skill();
            }
        }
    }
}
