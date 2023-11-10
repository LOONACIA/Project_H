using LOONACIA.Unity.Inputs;
using LOONACIA.Unity.Managers;
using UnityEngine.InputSystem;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity.Console
{
	partial class DebugController
	{
		private DebugInputActions _inputActions;

		private DebugInputContext _inputContext;

		private void EnableInput()
		{
			_inputActions ??= ManagerRoot.Input.GetInputActions<DebugInputActions>();
			_inputContext ??= new(this);
			_inputActions.Debug.SetCallbacks(_inputContext);
			ManagerRoot.Input.RegisterInputActions(_inputActions, "DEBUG");
			_inputActions.Enable();
		}

		private class DebugInputContext : DebugInputActions.IDebugActions
		{
			private readonly DebugController _controller;

			public DebugInputContext(DebugController controller)
			{
				_controller = controller;
			}

			public void OnReturn(InputAction.CallbackContext context)
			{
				if (context.phase != InputActionPhase.Performed)
				{
					return;
				}
			
				if (_controller._isToggled)
				{
					_controller.ExecuteCommand();
				}
			}

			public void OnToggle(InputAction.CallbackContext context)
			{
				if (context.phase != InputActionPhase.Performed)
				{
					return;
				}
			
				_controller._isToggled = !_controller._isToggled;
			}
		}
	}
}