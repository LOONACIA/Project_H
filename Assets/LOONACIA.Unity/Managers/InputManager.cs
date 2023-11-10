using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace LOONACIA.Unity.Managers
{
	public class InputManager
	{
		private readonly Dictionary<string, IInputActionCollection2> _inputActions = new();

		public string CurrentControlScheme { get; private set; }

		public void RegisterInputActions<T>(T inputActions)
			where T : class, IInputActionCollection2
		{
			RegisterInputActions(inputActions, typeof(T).Name);
		}

		public void RegisterInputActions<T>(T inputActions, string key)
			where T : class, IInputActionCollection2
		{
			_inputActions[key] = inputActions;
		}

		public T GetInputActions<T>(string key = null)
			where T : class, IInputActionCollection2, new()
		{
			key ??= typeof(T).Name;
			if (!_inputActions.TryGetValue(key, out var inputActions))
			{
				inputActions = new T();
				_inputActions.Add(key, inputActions);
			}

			return inputActions as T;
		}

		public void Enable<T>(string key = null)
			where T : class, IInputActionCollection2
		{
			key ??= typeof(T).Name;
			if (_inputActions.TryGetValue(key, out var inputActions) && inputActions is T)
			{
				inputActions.Enable();
			}
		}

		public void Enable(string key)
		{
			if (_inputActions.TryGetValue(key, out var inputActions))
			{
				inputActions.Enable();
			}
		}

		public void Disable<T>(string key = null)
			where T : class, IInputActionCollection2
		{
			key ??= typeof(T).Name;
			if (_inputActions.TryGetValue(key, out var inputActions) && inputActions is T)
			{
				inputActions.Disable();
			}
		}

		public void Disable(string key)
		{
			if (_inputActions.TryGetValue(key, out var inputActions))
			{
				inputActions.Disable();
			}
		}

		internal void Init()
		{
			InputSystem.onActionChange -= OnActionChange;
			InputSystem.onActionChange += OnActionChange;
		}

		private void OnActionChange(object arg, InputActionChange phase)
		{
			if (phase != InputActionChange.ActionPerformed)
			{
				return;
			}

			if (arg is not InputAction inputAction)
			{
				return;
			}

			if (inputAction.GetBindingForControl(inputAction.activeControl) is { } binding)
			{
				CurrentControlScheme = binding.groups;
			}
		}
	}
}