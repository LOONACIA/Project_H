using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour, IHackable
{
    [SerializeField]
    private float m_progressTime = 0.5f;

    [SerializeField]
    private float m_jumpPower = 10;

    private bool m_isHacking;

    private void OnTriggerStay(Collider other)
    {
        if (m_isHacking && other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            Jump(other.gameObject);
        }
    }

    public void Hacking()
    {
        if (m_isHacking) return;

        m_isHacking = true;

        Debug.Log(m_isHacking);
    }

    public void Recovery()
    {
        m_isHacking = false;
    }

    private void Jump(GameObject target)
    {
        if (target == null) return; 

        if (target.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody.AddForce(m_jumpPower * transform.up, ForceMode.VelocityChange);

            Debug.Log(m_jumpPower * transform.up);
        }
    }
}
