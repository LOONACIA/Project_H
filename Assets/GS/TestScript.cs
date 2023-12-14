using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// temp
public class TestScript : MonoBehaviour
{
    [SerializeField]
    private Monster m_character;

    private ActorStatus m_status;

    private void Start()
    {
        m_status = m_character.GetComponent<ActorStatus>();
    }


    private void Update()
    {
        Debug.Log(m_status.IsKnockedDown);
    }
}
