using System;
using UnityEngine;

namespace LOONACIA.Unity.Console
{
	public class TransformObjectCommand : DebugCommandBase
	{
		public enum TransformType
		{
			Scale,
			Rotation,
			Position
		}

		private readonly TransformType _type;

		public TransformObjectCommand(string id, string description, string format, TransformType type) : base(id, description, format)
		{
			_type = type;
		}

		public override void Execute(object parameter = null)
		{
			if (parameter is not string paramString)
			{
				return;
			}

			var span = paramString.AsSpan();

			if (!ArgumentParserBag.TryGetGameObject(span, out var go))
			{
				return;
			}

			int index = span.IndexOf(go.name) + go.name.Length;
			if (!ArgumentParserBag.TryGetVector3(span.Slice(index + 1).Trim(), out Vector3 vector))
			{
				return;
			}

			switch (_type)
			{
				case TransformType.Scale:
					go.transform.localScale = vector;
					break;
				case TransformType.Rotation:
					go.transform.rotation = Quaternion.Euler(vector);
					break;
				case TransformType.Position:
					go.transform.position = vector;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}