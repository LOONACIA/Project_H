using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    private readonly Dictionary<int, Quest> m_registeredQuests = new();
    
    private readonly HashSet<Quest> m_activatedQuests = new();
    
    public event EventHandler<Quest> QuestActivated;
    
    public event EventHandler<Quest> QuestCompleted;

    public void Init()
    {
        m_registeredQuests.Clear();
        var notifications = GameManager.Settings.QuestData.Notifications;
        var span = CollectionsMarshal.AsSpan(notifications);
        foreach (var notification in span)
        {
            if (notification is Quest quest)
            {
                quest.IsCompleted = false;
                m_registeredQuests.Add(quest.Id, quest);
            }
        }
    }

    public void Activate(int id)
    {
        if (!m_registeredQuests.TryGetValue(id, out var quest))
        {
            Debug.LogError($"[QuestManager] {id} is not registered");
            return;
        }
        
        if (!m_activatedQuests.Add(quest))
        {
            Debug.LogError($"[QuestManager] {id} is already activated");
            return;
        }
        
        QuestActivated?.Invoke(this, quest);
    }

    public void Complete(int id)
    {
        if (!m_registeredQuests.TryGetValue(id, out var quest))
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