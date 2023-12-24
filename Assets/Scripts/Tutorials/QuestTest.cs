using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTest : MonoBehaviour
{
    [SerializeField]
	private int m_questId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            GameManager.Quest.Activate(m_questId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            GameManager.Quest.Complete(m_questId);
        }
    }
}
