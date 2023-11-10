using System;
using System.Collections.Generic;
using System.Linq;
using LOONACIA.Unity.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LOONACIA.Unity.Console
{
    public partial class DebugController : MonoBehaviour
    {
        private List<DebugCommandBase> _commands;

        private bool _isToggled;

        private bool _showHelp;

        private string _commandString;

        private Vector2 _scrollView;

        private Action _enablePlayerInput;

        private Action _disablePlayerInput;

        private void Awake()
        {
            Init();
            EnableInput();
            }

        private void OnGUI()
        {
            if (!_isToggled)
            {
                return;
            }

            int fontSize = Mathf.Max(15, (int)(Screen.height / 54f));
            GUI.skin.textField.fontSize = fontSize;
            GUI.skin.label.fontSize = fontSize;

            float y = 0f;

            if (_showHelp)
            {
                float helpBoxHeight = fontSize * 5f;
                GUI.Box(new(0, y, Screen.width, helpBoxHeight), string.Empty);
                Rect viewport = new(0, 0, Screen.width - 30, fontSize * _commands.Count);

                _scrollView = GUI.BeginScrollView(new(0, y + 5f, Screen.width, helpBoxHeight - 10f), _scrollView, viewport);
                foreach ((DebugCommandBase command, int index) in _commands.Select((command, index) => (command, index)))
                {
                    Rect labelRect = new(5, (fontSize * 1.5f) * index, viewport.width - 100, fontSize * 1.5f);
                    GUI.Label(labelRect, $"{command.Format} - {command.Description}");
                }

                GUI.EndScrollView();

                y += helpBoxHeight;
            }

            GUI.Box(new(0, y, Screen.width, fontSize * 2f), string.Empty);
            GUI.backgroundColor = new();
            GUI.SetNextControlName("DebugConsole");
            _commandString = GUI.TextField(new(10f, y + (fontSize / 2f), Screen.width - 20f, fontSize * 1.5f), _commandString);
            GUI.FocusControl("DebugConsole");
        }

        private void ExecuteCommand()
        {
            _showHelp = false;

            (string id, string parameter) = ParseCommandString();
            foreach (var command in _commands.Where(command => command.Id.Equals(id, StringComparison.OrdinalIgnoreCase)))
            {
                command.Execute(parameter);
            }

            _commandString = string.Empty;
        }

        private (string Id, string Parameter) ParseCommandString()
        {
            int index;
            return (index = _commandString.IndexOf(' ')) == -1
                ? (_commandString, string.Empty)
                : (_commandString[..index], _commandString[index..].TrimStart());
        }

        private void Init()
        {
            _commands = new()
            {
                new TransformObjectCommand(
                    id: "move",
                    description: "Move the position of specified GameObject",
                    format: "move <name> <position>",
                    type: TransformObjectCommand.TransformType.Position),

                new TransformObjectCommand(
                    id: "rotate",
                    description: "Rotate the specified GameObject",
                    format: "rotate <name> <rotation>",
                    type: TransformObjectCommand.TransformType.Rotation),

                new DebugCommand<float>(
                    id: "time_scale",
                    description: "Change the time scale",
                    format: "time_scale <scale>",
                    execute: scale => Time.timeScale = scale,
                    parser: float.TryParse),

                new DebugCommand(
                    id: "reload",
                    description: "Reload the current scene",
                    format: "reload",
                    execute: () => SceneManagerEx.LoadScene(SceneManager.GetActiveScene().name)),

                new DebugCommand(
                    id: "exit",
                    description: "Exit the game",
                    format: "exit",
                    execute: () =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
					Application.Quit();
#endif
                    }),

                new DebugCommand("help", "Shows a list of commands", "help", () => _showHelp = true),
            };
        }
    }
}