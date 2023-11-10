using UnityEditor;
using UnityEngine;

namespace LOONACIA.Unity.Editor
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyAttributeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (attribute is not ReadOnlyAttribute ro)
			{
				return;
			}
			
			GUI.enabled = ro.Mode is ReadOnlyAttribute.ReadOnlyMode.RuntimeOnly && !Application.isPlaying;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}