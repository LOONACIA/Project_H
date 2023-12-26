using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCompleteTrigger : MonoBehaviour
{
    public enum CompleteType
    {
        Interaction,
        Trigger
    }

    [SerializeField]
    private int m_questId;

    [SerializeField]
    private CompleteType m_type;

    [SerializeField]
    private IInteractableObject m_assoicatedObject;

    private bool m_isTriggered;

    private void Start()
    {
        if (m_type == CompleteType.Interaction && m_assoicatedObject != null)
        {
            m_assoicatedObject.Interacted += OnInteracted;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_isTriggered || m_type != CompleteType.Trigger)
        {
            return;
        }

        if (!other.TryGetComponent<Actor>(out var actor) || !actor.IsPossessed)
        {
            return;
        }

        m_isTriggered = true;
        Complete();
    }

    private void OnInteracted(object sender, Transform e)
    {
        if (!m_isTriggered)
        {
            return;
        }

        m_isTriggered = true;
        Complete();
    }

    private void Complete()
    {
        GameManager.Notification.Complete(m_questId);
    }
}
