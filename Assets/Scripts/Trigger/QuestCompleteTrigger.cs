using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    private int m_nextQuestId;

    [SerializeField]
    private CompleteType m_type;

    [SerializeField]
    private InteractableObject m_associatedObject;
    
    [SerializeField]
    private UnityEvent m_onComplete;

    private bool m_isTriggered;
    

    private void Start()
    {
        if (m_type == CompleteType.Interaction && m_associatedObject != null)
        {
            m_associatedObject.Interacted += OnInteracted;
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
        if (m_isTriggered)
        {
            return;
        }
        Debug.Log("Interacted");
        m_isTriggered = true;
        Complete();
        if(m_nextQuestId != 0)
        {
            GameManager.Notification.Activate(m_nextQuestId);
        }
        else
        {
            GameManager.Notification.Activate(m_questId + 1);
        }

    }

    private void Complete()
    {
        GameManager.Notification.Complete(m_questId);
        m_onComplete?.Invoke();
    }
}
