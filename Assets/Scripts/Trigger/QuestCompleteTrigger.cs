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
        Trigger,
        Horde
    }

    [SerializeField]
    private int m_questId;

    [SerializeField]
    private int m_nextQuestId;

    [SerializeField]
    private CompleteType m_type;

    [SerializeField]
    private bool m_isPanel;

    [SerializeField]
    private InteractableObject m_associatedObject;

    [SerializeField]
    private WaveTrigger m_waveTrigger;

    [SerializeField]
    private UnityEvent m_onComplete;

    private bool m_isTriggered;



    private void Start()
    {
        if (m_type == CompleteType.Interaction && m_associatedObject != null)
        {
            m_associatedObject.Interacted += OnInteracted;
        }

        if(m_type == CompleteType.Horde && m_waveTrigger != null)
        {
            m_waveTrigger.WaveEnd += OnWaveEnd;
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
        if (m_isPanel)
        {
            GameManager.Notification.Activate(m_nextQuestId);
            m_isTriggered = true;
            return;
        }

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

    private void OnWaveEnd(object sender, EventArgs e)
    {
        if(m_waveTrigger == null)
        {
            return;
        }

        Complete();
        if (m_nextQuestId != 0)
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
