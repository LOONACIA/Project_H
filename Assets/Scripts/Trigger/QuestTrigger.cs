using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    private WaitForSeconds m_waitForSecondsCache;

    [SerializeField]
    private int[] m_Id;

    [SerializeField]
    private float m_delay;

    private bool m_IsActive = true;

    private void Start()
    {
        m_waitForSecondsCache = new(m_delay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Actor>(out var actor) || !actor.IsPossessed)
        {
            return;
        }

        if (!m_IsActive)
        {
            return;
        }
        m_IsActive = false;

        StartCoroutine(CoSendNotification());
    }

    private IEnumerator CoSendNotification()
    {
        for (int index = 0; index < m_Id.Length; index++)
        {
            int id = m_Id[index];
            GameManager.Notification.Activate(id);
            if (index != m_Id.Length - 1)
            {
                yield return m_waitForSecondsCache;
            }
        }
    }
}
