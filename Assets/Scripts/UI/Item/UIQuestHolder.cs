using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using LOONACIA.Unity.UI;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestHolder : UIBase
{
    private readonly Dictionary<int, UIQuestPresenter> m_presenters = new();
    
    private void OnEnable()
    {
        GameManager.Quest.QuestActivated += OnQuestActivated;
        GameManager.Quest.QuestCompleted += OnQuestCompleted;
    }

    private void OnDisable()
    {
        GameManager.Quest.QuestActivated -= OnQuestActivated;
        GameManager.Quest.QuestCompleted -= OnQuestCompleted;
    }

    protected override void Init()
    {
    }
    
    private void OnQuestActivated(object sender, Quest e)
    {
        int id = e.Id;
        if (m_presenters.TryGetValue(id, out var presenter))
        {
            return;
        }
        
        var go = ManagerRoot.Resource.Instantiate($"UI/Item/{nameof(UIQuestPresenter)}");
        go.transform.SetParent(transform);
        presenter = go.GetOrAddComponent<UIQuestPresenter>();
        presenter.SetQuest(e);
        m_presenters.Add(id, presenter);
    }

    private void OnQuestCompleted(object sender, Quest e)
    {
        int id = e.Id;
        if (!m_presenters.TryGetValue(id, out var presenter))
        {
            Debug.LogError($"[QuestHolder] {id} is not activated");
            return;
        }
        
        presenter.Complete();
        m_presenters.Remove(id);
    }
}
