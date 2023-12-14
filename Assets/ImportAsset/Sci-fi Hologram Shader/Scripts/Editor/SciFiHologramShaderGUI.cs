using UnityEditor;
using UnityEngine;
using System;

class SciFiHologramShaderGUI : ShaderGUI {
	MaterialEditor editor;
	MaterialProperty[] properties;
	Material material;
	
    override public void OnGUI (MaterialEditor editor, MaterialProperty[] properties) {
		Color tempGUIColor = GUI.color;
		this.editor = editor;
		this.properties = properties;
		this.material = editor.target as Material;
		//
		// general settings
		GUILayout.Label("General settings", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 2;	
		editor.RangeProperty(GetProp("_Brightness"), "Brightness");	
		editor.RangeProperty(GetProp("_Fade"), "Fade");
		EditorGUI.indentLevel -= 2;	
		// rim light settings
		GUILayout.Label("Rim light settings", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 2;	
		editor.ColorProperty(GetProp("_RimColor"), "Tint");	
		editor.RangeProperty(GetProp("_RimStrenght"), "Strength");
		editor.RangeProperty(GetProp("_RimFalloff"), "Falloff");
		EditorGUI.indentLevel -= 2;
		// main settings
		GUILayout.Label("\nMain texture", EditorStyles.boldLabel);
		EditorGUI.indentLevel += 2;	
		editor.ColorProperty(GetProp("_Color"), "Tint");	 
		EditorGUI.indentLevel -= 2;	
		editor.TexturePropertySingleLine(new GUIContent("Texture",""), GetProp("_MainTex"));
		TextureUVTransformProperty(GetProp("_MainTex"));
		// scanlines
		GUILayout.Label("\nEffects", EditorStyles.boldLabel);
		editor.TexturePropertySingleLine(new GUIContent("Scanlines",""), GetProp("_Scanlines"));
		editor.RangeProperty(GetProp("_ScanStr"), "Strength");	
		TextureUVTransformProperty(GetProp("_Scanlines"), "Scale", "Speed"); 		
		// scanlines #2
		ToggleVariant("Enable more scanlines", "SCAN2_ON", "SCAN2_OFF");
		if (IsKeyword("SCAN2_OFF")) GUI.color = new Color(tempGUIColor.r,tempGUIColor.g,tempGUIColor.b,0.25f);
		editor.TexturePropertySingleLine(new GUIContent("Scanlines",""), GetProp("_Scan2"));
		editor.RangeProperty(GetProp("_ScanStr2"), "Strength");	
		TextureUVTransformProperty(GetProp("_Scan2"), "Scale", "Speed"); 
		GUI.color = tempGUIColor;
	}

	bool IsKeyword(String s) {
		String[] keywords = material.shaderKeywords;
		return System.Array.IndexOf(keywords,s) != -1;
	}
	
	void ToggleVariant(String d, String on, String off) {
		bool toggle = IsKeyword(on);
		EditorGUI.BeginChangeCheck ( );
		GUILayout.Space(10);
		toggle = EditorGUILayout.Toggle (d, toggle);
		if (EditorGUI.EndChangeCheck ( )) {
			String[] newKeywords = new String[1];
			newKeywords[0] = toggle ? on : off;
			material.shaderKeywords = newKeywords;
			EditorUtility.SetDirty (material);
		}
	}
	
	MaterialProperty GetProp (String propName) {
		return FindProperty(propName, properties);
	}	

	Vector4 TextureUVTransformProperty(MaterialProperty uvTransformProperty) {
		return TextureUVTransformProperty(uvTransformProperty, "Tiling", "Offset");
	}
	Vector4 TextureUVTransformProperty(MaterialProperty uvTransformProperty, String firstText, String secondText) {
		Rect position = EditorGUILayout.GetControlRect(true, 32.0f, EditorStyles.layerMaskField, new GUILayoutOption[0]);
		Vector4 uvTransform = uvTransformProperty.textureScaleAndOffset;
		Vector2 value = new Vector2(uvTransform.x, uvTransform.y);
		Vector2 value2 = new Vector2(uvTransform.z, uvTransform.w);
		float num = EditorGUIUtility.labelWidth;
		float x = position.x + num;
		float x2 = position.x + 30f;
		Rect totalPosition = new Rect(x2, position.y, EditorGUIUtility.labelWidth, 16.0f);
		Rect position2 = new Rect(x, position.y, position.width - EditorGUIUtility.labelWidth, 16.0f);
		EditorGUI.PrefixLabel(totalPosition, new GUIContent(firstText));
		value = EditorGUI.Vector2Field(position2, GUIContent.none, value);
		totalPosition.y += 16.0f;
		position2.y += 16.0f;
		EditorGUI.PrefixLabel(totalPosition, new GUIContent(secondText));
		value2 = EditorGUI.Vector2Field(position2, GUIContent.none, value2);
		Vector4 newUVTransform = new Vector4(value.x, value.y, value2.x, value2.y);
		uvTransformProperty.textureScaleAndOffset = newUVTransform;
		return newUVTransform;
	}	
}