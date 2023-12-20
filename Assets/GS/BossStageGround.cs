using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BossStageGround : MonoBehaviour
{
    private Collider m_collider;

    private void Start()
    {
        TryGetComponent<Collider>(out m_collider);
        m_collider.enabled = false;
    }

    public void Active()
    { 
        m_collider.enabled = true;
    }
}
