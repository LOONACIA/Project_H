using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCharacter : MonoBehaviour
{
    [SerializeField]
    private Transform m_teleportPosition;
    private Vector3 m_position;
    // Start is called before the first frame update
    void Start()
    {
        m_position = m_teleportPosition.position;
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Actor>(out var actor))
        {
            // Ignore
            return;
        }

        if (actor.IsPossessed)
        {
            other.transform.position = m_position;
        }
    }
}
