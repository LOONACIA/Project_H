using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour, IHackable
{
    [SerializeField]
    private float m_progressTime = 0.5f;

    [SerializeField]
    private float m_jumpPower = 10;

    // TODO : KnockDown 말고 다른 변수로 설정
    [SerializeField]
    private float m_knockDownTime = 0.5f;

    private bool m_isHacking;

    private bool m_canJump = true;

    private void OnTriggerStay(Collider other)
    {
        if (m_isHacking && other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            Jump(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_canJump = true;
    }

    public void Hacking()
    {
        if (m_isHacking) return;

        m_isHacking = true;
    }

    public void Recovery()
    {
        m_isHacking = false;
    }

    private void Jump(GameObject target)
    {
        if (target == null) return;
        if (!m_canJump) return;

        m_canJump = false;

        if (target.TryGetComponent<ActorStatus>(out var actorStatus))
        {
            actorStatus.SetKnockDown(m_knockDownTime);
        }

        if (target.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody.velocity = rigidbody.velocity.GetFlatVector();
            rigidbody.AddForce(m_jumpPower * transform.up, ForceMode.VelocityChange);
        }
    }
}
