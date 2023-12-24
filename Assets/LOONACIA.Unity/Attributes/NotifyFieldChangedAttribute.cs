using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
    /// <summary>
    /// Invoke method when field changed in inspector<br/>
    /// <br/>
    /// Method signature must be one of these:<br/>
    /// 1. MethodName()<br/>
    /// 2. MethodName(object oldValue) - newValue is target field value<br/>
    /// 3. MethodName(object oldValue, object newValue)<br/>
    /// <br/>
    /// return type is doesn't matter<br/>
    /// </summary>
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
