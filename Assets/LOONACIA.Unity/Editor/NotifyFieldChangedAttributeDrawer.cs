using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LOONACIA.Unity.Editor
{
	[CustomPropertyDrawer(typeof(NotifyFieldChangedAttribute))]
	public class NotifyFieldChangedAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (attribute is not NotifyFieldChangedAttribute nfc)
			{
				return;
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(position, property, label);
			if (!EditorGUI.EndChangeCheck())
			{
				return;
			}

			MethodInfo method = property.serializedObject.targetObject
				.GetType()
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.FirstOrDefault(m => m.Name == nfc.MethodName);
			
			property.serializedObject.ApplyModifiedProperties();

			if (method != null && !method.GetParameters().Any())
			{
				method.Invoke(property.serializedObject.targetObject, null);
			}
		}
	}
}
