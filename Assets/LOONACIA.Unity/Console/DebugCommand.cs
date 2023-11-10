using System;

namespace LOONACIA.Unity.Console
{
	public class DebugCommand : DebugCommandBase
	{
		private readonly Action _execute;

		public DebugCommand(string id, string description, string format, Action execute) : base(id, description, format)
		{
			_execute = execute;
		}

		public override void Execute(object parameter = null)
		{
			_execute?.Invoke();
		}
	}
}
