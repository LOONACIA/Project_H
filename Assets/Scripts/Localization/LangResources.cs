using LOONACIA.Unity;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(LangResources), menuName = "Localization/" + nameof(LangResources))]
public class LangResources : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private TextAsset m_textAsset;
    
    [SerializeField]
    [ReadOnly]
    private SerializableDictionary<string, string> m_resources = new();

    public SerializableDictionary<string, string> Resources => m_resources;

    public void OnBeforeSerialize()
    {
        if (m_textAsset == null)
        {
            return;
        }
        
        Parse();
    }

    public void OnAfterDeserialize()
    {
        
    }

    private void Parse()
    {
        const string marker = "%%";
        Resources.Clear();
        var span = m_textAsset.text.Replace("\r", string.Empty).Replace(@"\,", marker).AsSpan().Trim();
        foreach (var line in span.Split('\n'))
        {
            if (line.Chars.IsWhiteSpace())
            {
                continue;
            }

            var fields = line.Chars.Split(',');
            string key = string.Empty, value = string.Empty;
            foreach (var field in fields)
            {
                switch (field.Index)
                {
                    case 0:
                        key = field.Chars.ToString();
                        break;
                    case 1:
                        value = field.Chars.Contains(marker, StringComparison.OrdinalIgnoreCase)
                            ? field.Chars.Trim("\"").ToString().Replace(marker, @",")
                            : field.Chars.ToString();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            $"{field.Chars.ToString()} is not supported type.");
                }
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                Debug.LogWarning("Key is empty.");
                continue;
            }
            
            Resources.Add(key, value);
        }
    }
}
