using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCharacter : MonoBehaviour
{
    [SerializeField]
    private Transform m_teleportPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (m_teleportPosition == null)
            m_teleportPosition = transform;
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
            GameManager.Instance.SetSavePoint(m_teleportPosition.position);
        }
    }
}
