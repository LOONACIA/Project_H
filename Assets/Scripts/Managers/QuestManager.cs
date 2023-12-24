using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class QuestManager
{
    private readonly Dictionary<int, Quest> m_registeredQuests = new();

    private readonly Dictionary<int, QuestPresenter> m_presenters = new();
    
    private Transform m_questRoot;

    public void Init()
    {
        m_registeredQuests.Clear();
        m_registeredQuests.Add(0, new() { Id = 0, Content = "Hack and Heist Body" });
        m_registeredQuests.Add(1, new() { Id = 1, Content = "Hack and Heist Body" });
    }

    public void Activate(int id)
    {
        if (m_questRoot == null)
        {
            GameObject root = ManagerRoot.Resource.Instantiate("UI/Item/QuestHolder");
            m_questRoot = root.FindChild<Transform>("Panel");
        }
        
        if (m_presenters.TryGetValue(id, out var presenter))
        {
            return;
        }
        
        if (!m_registeredQuests.TryGetValue(id, out var quest))
        {
            Debug.LogError($"[QuestManager] {id} is not registered");
            return;
        }

        var go = ManagerRoot.Resource.Instantiate("UI/Item/QuestPresenter");
        go.transform.SetParent(m_questRoot);
        presenter = go.GetOrAddComponent<QuestPresenter>();
        presenter.SetQuest(quest);
        m_presenters.Add(id, presenter);
    }

    public void Complete(int id)
    {
        if (!m_registeredQuests.TryGetValue(id, out var quest))
        {
            Debug.LogError($"[QuestManager] {id} is not registered");
            return;
        }

        quest.IsCompleted = true;
        
        if (!m_presenters.TryGetValue(id, out var presenter))
        {
            Debug.LogError($"[QuestManager] {id} is not activated");
            return;
        }
        
        presenter.Complete();
    }
}