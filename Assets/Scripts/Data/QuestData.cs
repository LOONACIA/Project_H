using LOONACIA.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = nameof(QuestData), menuName = "Data/" + nameof(QuestData))]
public class QuestData : ScriptableObject, ISerializationCallbackReceiver
{
    private static readonly Regex s_csvRegex = new(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

    [SerializeField]
    [NotifyFieldChanged(nameof(OnTextAssetChanged))]
    private TextAsset m_questData;

    [field: SerializeReference]
    public List<Notification> Notifications { get; private set; }

    public void Parse(string text)
    {
        const string marker = "%%";

        Dictionary<int, Notification> notifications = new();

        // Skip first line
        var span = text.Replace(@"\,", marker).AsSpan();
        bool isHeater = true;
        foreach (var line in span.Split('\n'))
        {
            if (isHeater)
            {
                isHeater = false;
                continue;
            }

            // Skip empty line
            if (line.Chars.IsWhiteSpace())
            {
                continue;
            }

            Notification notification = null;
            bool isPanel = false;
            // 각 셀 안에 콤마가 없다고 가정. 콤마가 들어가야 되면 상단 정규식을 사용해야 함.
            foreach (var field in line.Chars.Split(','))
            {
                switch (field.Index)
                {
                    // Type
                    case 0:
                        notification = field.Chars switch
                        {
                            var type when type.SequenceEqual("Text".AsSpan()) => new Quest(),
                            var type when type.SequenceEqual("Panel".AsSpan()) => new ModalDialog(),
                            _ => throw new ArgumentOutOfRangeException(
                                $"{field.Chars.ToString()} is not supported type.")
                        };
                        isPanel = notification is ModalDialog;
                        break;

                    // Id
                    case 1:
                        if (int.TryParse(field.Chars, out int id))
                        {
                            notification!.Id = id;
                        }

                        break;

                    // Main Quest
                    case 2:
                        if (!isPanel)
                        {
                            ((Quest)notification)!.IsMainQuest = !int.TryParse(field.Chars, out _);
                        }

                        break;

                    // Content
                    case 3:
                        // csv 파일 특성상 콤마가 들어간 셀은 "로 감싸져 있음. 이를 trim한 후 마커를 콤마로 바꿔줌.
                        notification!.Content = field.Chars.Contains(marker, StringComparison.OrdinalIgnoreCase)
                            ? field.Chars.Trim("\"").ToString().Replace(marker, @",")
                            : field.Chars.ToString();
                        break;

                    // Related Id
                    case 4:
                        if (isPanel && int.TryParse(field.Chars, out int relatedId))
                        {
                            ModalDialog parent = (ModalDialog)notifications[relatedId];
                            if (parent.RelatedDialog != null)
                            {
                                throw new ArgumentException(
                                    $"Parent dialog {relatedId} already has related dialog {parent.RelatedDialog.Id}.");
                            }

                            parent.RelatedDialog = (ModalDialog)notification;
                        }

                        break;

                    // Resource Path
                    case 5:
                        if (isPanel && field.Chars.Length > 0)
                        {
                            VideoClip videoClip = Resources.Load<VideoClip>(field.Chars.ToString());
                            ((ModalDialog)notification).Video = videoClip;
                        }

                        break;
                }
            }

            notifications.Add(notification!.Id, notification);
        }

        Notifications = new(notifications.Values);
    }
    
    public void OnBeforeSerialize()
    {
        Parse(m_questData.text);
    }

    public void OnAfterDeserialize()
    {
    }

    private void OnTextAssetChanged()
    {
        Parse(m_questData.text);
    }
}