using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace LOONACIA.Unity.Managers
{
	public class InputManager : IDisposable
	{
		private readonly Dictionary<string, IInputActionCollection2> _inputActions = new();
        
        private readonly Dictionary<string, InputControlScheme> _controlSchemes = new();

        private InputDevice _currentDevice;
        
        private IDisposable _handler;
        
        ~InputManager()
        {
            Dispose();
        }

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
        
        public string GetCurrentControlScheme<T>()
            where T : class, IInputActionCollection2
        {
            return GetCurrentControlScheme(typeof(T).Name);
        }
        
        public string GetCurrentControlScheme(string key)
        {
            if (_controlSchemes.TryGetValue(key, out var controlScheme) && controlScheme.SupportsDevice(_currentDevice))
            {
                return controlScheme.name;
            }
            
            controlScheme = _inputActions[key].controlSchemes.FirstOrDefault(x => x.SupportsDevice(_currentDevice));
            if (controlScheme == default)
            {
                controlScheme = _inputActions[key].controlSchemes.FirstOrDefault();
            }
            
            _controlSchemes[key] = controlScheme;
            return controlScheme.name;
        }
        
        public void Dispose()
        {
            _handler?.Dispose();
            GC.SuppressFinalize(this);
        }

		internal void Init()
        {
            _handler = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
		}
        
        private void OnAnyButtonPress(InputControl inputControl)
        {
            _currentDevice = inputControl.device;
        }
    }
}