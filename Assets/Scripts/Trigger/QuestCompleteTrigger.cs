using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

        if (m_type == CompleteType.Horde && m_waveTrigger != null)
        {
            m_waveTrigger.WaveEnd += OnWaveEnd;
        }
    }

    private void OnEnable()
    {
        if (GameManager.Notification.IsCompleted(m_questId))
        {
            m_isTriggered = true;
            m_onComplete?.Invoke();
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
        if (m_nextQuestId != 0)
        {
            GameManager.Notification.Activate(m_nextQuestId);
        }
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
        if (m_nextQuestId != 0)
        {
            GameManager.Notification.Activate(m_nextQuestId);
        }
    }

    private void OnWaveEnd(object sender, EventArgs e)
    {
        if (m_waveTrigger == null)
        {
            return;
        }

        Complete();
        if (m_nextQuestId != 0)
        {
            GameManager.Notification.Activate(m_nextQuestId);
        }
    }

    private void Complete()
    {
        GameManager.Notification.Complete(m_questId);
        m_onComplete?.Invoke();
    }

    private void OnValidate()
    {
        if (m_questId <= 0)
        {
            Debug.LogError("Quest ID must be greater than 0.", gameObject);
        }
    }
}