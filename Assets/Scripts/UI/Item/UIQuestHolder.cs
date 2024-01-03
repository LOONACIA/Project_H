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
        foreach (var activatedQuest in GameManager.Notification.GetActivatedQuests())
        {
            OnNotificationActivated(this, activatedQuest);
        }
        GameManager.Notification.Activated += OnNotificationActivated;
        GameManager.Notification.QuestCompleted += OnNotificationCompleted;
    }

    private void OnDisable()
    {
        GameManager.Notification.Activated -= OnNotificationActivated;
        GameManager.Notification.QuestCompleted -= OnNotificationCompleted;
    }

    protected override void Init()
    {
    }

    private void OnNotificationActivated(object sender, Notification e)
    {
        int id = e.Id;
        if (m_presenters.TryGetValue(id, out var presenter))
        {
            return;
        }

        if (e is not Quest quest)
        {
            return;
        }

        var go = ManagerRoot.Resource.Instantiate($"UI/Item/{nameof(UIQuestPresenter)}");
        go.transform.SetParent(transform);
        go.transform.localScale = Vector3.one;
        presenter = go.GetOrAddComponent<UIQuestPresenter>();
        presenter.SetQuest(quest);
        m_presenters.Add(id, presenter);
    }

    private void OnNotificationCompleted(object sender, Quest e)
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