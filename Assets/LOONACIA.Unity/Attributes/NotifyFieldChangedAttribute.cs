using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
	[AttributeUsage(AttributeTargets.Field)]
	public class NotifyFieldChangedAttribute : PropertyAttribute
	{
		public NotifyFieldChangedAttribute(string methodName)
		{
			MethodName = methodName;
		}

		public string MethodName { get; }
	}
}
