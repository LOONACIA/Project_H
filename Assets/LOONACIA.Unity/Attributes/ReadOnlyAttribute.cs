using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace LOONACIA.Unity
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public enum ReadOnlyMode
        {
            Always,
            RuntimeOnly
        }
        
        public ReadOnlyAttribute(ReadOnlyMode mode = ReadOnlyMode.Always)
        {
            Mode = mode;
        }
        
        public ReadOnlyMode Mode { get; }
    }
}