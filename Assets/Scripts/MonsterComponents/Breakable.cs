using LOONACIA.Unity;
using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    // 파편이 사라지는 시간
    private const float DESTROY_INTERVAL_DEFALUT = 5f;
    private const float DESTROY_INTERVAL_GROUND = 3f;
 
    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;

    // 파편 폭발 효과 관련 변수
    private float m_throwForce = 10f;
    private float m_explosionForce = 10f;
    private float m_explosionRadius = 1f;
    private float m_upwardsModifier = 1f;

    // 파편이 사라지는 시간을 관리하는 코루틴
    private Coroutine m_coroutine;

    private void Start()
    {
        m_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
        m_rigidbody.isKinematic = true;

        m_collider = gameObject.GetOrAddComponent<BoxCollider>();
        m_collider.enabled = false;

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer = LayerMask.NameToLayer("Ground") | LayerMask.NameToLayer("Wall");
        if (collision.gameObject.layer == layer)
        {
            if (m_coroutine != null)
            {
                StopCoroutine(m_coroutine);
                m_coroutine = StartCoroutine(DestroyPart(DESTROY_INTERVAL_GROUND));
            }
        }
    }

    public void ReplacePart(in AttackInfo info, float destroyInterval = DESTROY_INTERVAL_DEFALUT)
    {
        if (m_rigidbody == null || m_collider == null) return;

        m_rigidbody.isKinematic = false;
        m_collider.enabled = true;

        transform.parent = null;
        AddForce(info);
        AddExplosion(info.HitPoint);

        if (m_coroutine == null)
        {
            m_coroutine = StartCoroutine(DestroyPart(destroyInterval));
        }
    }
   
    private void AddForce(in AttackInfo info)
    {
        Vector3 attackDirection = info.AttackDirection;
        attackDirection += info.Attacker.transform.forward * 1f;
        attackDirection += Random.insideUnitSphere * 1f;
        attackDirection = attackDirection.GetFlatVector();
        attackDirection = attackDirection.normalized;

        m_rigidbody.AddForce(attackDirection * m_throwForce, ForceMode.VelocityChange);
    }

    private void AddExplosion(Vector3 hitPoint)
    {
        m_rigidbody.AddExplosionForce(m_explosionForce, hitPoint, m_explosionRadius, m_upwardsModifier, ForceMode.VelocityChange);
    }


    private IEnumerator DestroyPart(float interval)
    {
        float randomInterval = Random.Range(interval * 0.95f, interval * 1.05f);
        yield return new WaitForSeconds(randomInterval);
        Destroy(gameObject);
    }
}