using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LOONACIA.Unity.Console
{
	public static class ArgumentParserBag
	{
		public static bool TryGetVector3(ReadOnlySpan<char> parameter, out Vector3 vector)
		{
			bool isHandled = false;
			vector = Vector3.zero;

			var enumerator = parameter.Split(',');
			if (!enumerator.MoveNext() || enumerator.Current.Chars.SequenceEqual(parameter))
			{
				enumerator = parameter.Split(' ');
			}
			enumerator.Reset();

			foreach (var split in enumerator)
			{
				if (!float.TryParse(split.Chars, out var value))
				{
					continue;
				}

				switch (split.Index)
				{
					case 0:
						vector.x = value;
						isHandled = true;
						break;
					case 1:
						vector.y = value;
						isHandled = true;
						break;
					case 2:
						vector.z = value;
						isHandled = true;
						break;
				}
			}

			return isHandled;
		}

		public static bool TryGetGameObject(ReadOnlySpan<char> parameter, out GameObject gameObject)
		{
			string name = Regex.Match(parameter.ToString(), @"\w+|""[\w\s]*""").Value.Trim('\"');
			gameObject = GameObject.Find(name);
			return gameObject != null;
		}
	}
}