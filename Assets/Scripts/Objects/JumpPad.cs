using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour, IActivate
{
    [SerializeField]
    [Tooltip("대상을 날리는 방향")]
    private JumpType m_jumpType;

    [SerializeField]
    [Tooltip("JumpPad로 점프하는 힘")]
    private float m_jumpPower = 10;

    [Tooltip("JumpPad가 사용 가능한지의 여부")]
    private bool m_isActive;

    [Tooltip("JumpPad로 점프를 할 수 있는지의 여부")]
    private bool m_canJump = true;

    [Tooltip("IE_SetIsFlying 코루틴을 저장")]
    private Coroutine m_coroutine;

    private void OnTriggerStay(Collider other)
    {
        if (m_isActive 
            && other.gameObject.layer == LayerMask.NameToLayer("Monster")
            && other.gameObject.TryGetComponent<Monster>(out var monster)
            && monster.IsPossessed)
        {
            Jump(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_canJump = true;
    }

    public void Activate()
    {
        if (m_isActive) return;

        m_isActive = true;
    }

    public void Deactivate()
    {
        m_isActive = false;
    }

    /// <summary>
    /// 대상을 점프시키는 함수
    /// </summary>
    /// <param name="target"></param>
    private void Jump(GameObject target)
    {
        if (target == null) return;
        if (!m_canJump) return;

        m_canJump = false;

        if (target.TryGetComponent<ActorStatus>(out var status))
        {
            // 대상을 잠시 KnockDown 상태로 만듦 => 대쉬 상태를 끊음
            status.SetKnockDown(0.1f);

            // 대상을 IsFlying 상태로 만듦
            if (target.TryGetComponent<MonsterMovement>(out var movement))
            {
                if (m_coroutine != null)
                    StopCoroutine(m_coroutine);
                m_coroutine = StartCoroutine(IE_SetIsFlying(status, movement));
            }
        }

        switch (m_jumpType)
        {
            case JumpType.Reflection:
                JumpUp(target);
                break;
            case JumpType.Up:
                JumpUp(target);
                break;
        }
    }

    /// <summary>
    /// target을 위로 날림
    /// </summary>
    /// <param name="target"></param>
    private void JumpUp(GameObject target)
    {
        if (target.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody.velocity = Vector3.zero;

            rigidbody.AddForce(transform.up * m_jumpPower, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// target을 반사 벡터 방향으로 날림
    /// </summary>
    /// <param name="target"></param>
    private void JumpReflection(GameObject target) 
    {
        if (target.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            var normalVec = transform.up;
            var reflectionVec = GetReflectionVector(rigidbody.velocity.normalized, normalVec).normalized;
            var targetVec = (reflectionVec + transform.up).normalized;

            rigidbody.velocity = Vector3.zero;

            rigidbody.AddForce(targetVec * m_jumpPower, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// 대상을 IsFlying 상태로 만드는 코루틴
    /// </summary>
    private IEnumerator IE_SetIsFlying(ActorStatus status, MonsterMovement movement)
    {
        float startTime = Time.time;

        while (movement.IsOnGround)
        { 
            if (Time.time - startTime > 1f)
                yield break;

            yield return null;
        }

        status.IsFlying = true;
    }

    /// <summary>
    /// 반사 벡터를 반환하는 함수
    /// </summary>
    /// <param name="incidentVec">입사 벡터</param>
    /// <param name="normalVec">법선 벡터</param>
    /// <returns></returns>
    private Vector3 GetReflectionVector(Vector3 incidentVec, Vector3 normalVec)
    { 
        return incidentVec + 2 * normalVec * Vector3.Dot(-incidentVec, normalVec);
    }

    private enum JumpType 
    {
        Reflection,
        Up
    }
}
