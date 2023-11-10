using System.Reflection;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
    public static class SerializedPropertyExtension
    {
        public static object GetBoxedValue(this SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Integer => property.intValue,
                SerializedPropertyType.Boolean => property.boolValue,
                SerializedPropertyType.Float => property.floatValue,
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Color => property.colorValue,
                SerializedPropertyType.ObjectReference => property.objectReferenceValue,
                SerializedPropertyType.LayerMask => property.intValue,
                SerializedPropertyType.Enum => property.enumValueIndex,
                SerializedPropertyType.Vector2 => property.vector2Value,
                SerializedPropertyType.Vector3 => property.vector3Value,
                SerializedPropertyType.Vector4 => property.vector4Value,
                SerializedPropertyType.Rect => property.rectValue,
                SerializedPropertyType.ArraySize => property.arraySize,
                SerializedPropertyType.Character => property.intValue,
                SerializedPropertyType.AnimationCurve => property.animationCurveValue,
                SerializedPropertyType.Bounds => property.boundsValue,
                SerializedPropertyType.Gradient => GetGradient(property),
                SerializedPropertyType.Quaternion => property.quaternionValue,
                SerializedPropertyType.ExposedReference => property.exposedReferenceValue,
                SerializedPropertyType.FixedBufferSize => property.fixedBufferSize,
                SerializedPropertyType.Vector2Int => property.vector2IntValue,
                SerializedPropertyType.Vector3Int => property.vector3IntValue,
                SerializedPropertyType.RectInt => property.rectIntValue,
                SerializedPropertyType.BoundsInt => property.boundsIntValue,
                SerializedPropertyType.ManagedReference => property.managedReferenceValue,
                SerializedPropertyType.Hash128 => property.hash128Value,
                _ => null
            };
        }

        private static Gradient GetGradient(SerializedProperty gradientProperty)
        {
            PropertyInfo propertyInfo =
                typeof(SerializedProperty).GetProperty("gradientValue", BindingFlags.NonPublic | BindingFlags.Instance);

            if (propertyInfo == null)
            {
                return null;
            }

            return propertyInfo.GetValue(gradientProperty, null) as Gradient;
        }
    }
}