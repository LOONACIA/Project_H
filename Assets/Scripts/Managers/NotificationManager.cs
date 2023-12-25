using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quest와 ModalDialog를 관리하는 매니저 클래스
/// </summary>
public class NotificationManager
{
    private readonly Dictionary<int, Notification> m_registeredNotifications = new();

    private readonly HashSet<Quest> m_activatedQuests = new();

    /// <summary>
    /// Notification이 활성화되었을 때 발생하는 이벤트
    /// </summary>
    public event EventHandler<Notification> Activated;

    /// <summary>
    /// Quest가 완료되었을 때 발생하는 이벤트
    /// </summary>
    public event EventHandler<Quest> QuestCompleted;

    public void Init()
    {
        m_registeredNotifications.Clear();
        var notifications = GameManager.Settings.QuestData.Notifications;
        var span = CollectionsMarshal.AsSpan(notifications);
        foreach (var notification in span)
        {
            m_registeredNotifications.Add(notification.Id, notification);
            notification.IsNotified = false;
            if (notification is Quest quest)
            {
                quest.IsCompleted = false;
            }
        }
    }

    public void Clear()
    {
        foreach (var quest in m_activatedQuests)
        {
            quest.IsCompleted = false;
            quest.IsNotified = false;
        }
        
        m_activatedQuests.Clear();
    }

    public void Activate(int id)
    {
        if (!m_registeredNotifications.TryGetValue(id, out var notification))
        {
            Debug.LogError($"[NotificationManager] {id} is not registered");
            return;
        }

        if (notification.IsNotified)
        {
            return;
        }

        if (notification is Quest quest)
        {
            // 이미 완료되거나 활성화된 퀘스트는 무시
            if (quest.IsCompleted || !m_activatedQuests.Add(quest))
            {
                return;
            }
        }

        Activated?.Invoke(this, notification);
        notification.IsNotified = true;
    }

    public void Complete(int id)
    {
        if (!m_registeredNotifications.TryGetValue(id, out var notification))
        {
            return;
        }

        if (notification is not Quest quest)
        {
            return;
        }

        quest.IsCompleted = true;
        if (!m_activatedQuests.Remove(quest))
        {
            return;
        }

        QuestCompleted?.Invoke(this, quest);
    }
}