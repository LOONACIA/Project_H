using LOONACIA.Unity;
using System.Collections;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private BoxCollider m_collider;

    // 파편이 날라가는 정도
    private float m_explosionForce = 15;

    // 파편이 사라지는 시간을 관리하는 코루틴
    private Coroutine m_coroutine;

    // 파편이 사라지는 시간
    private float m_initInterval = 5f;
    private float m_onGroundInterval = 3f;

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
                m_coroutine = StartCoroutine(DestroyPart(m_onGroundInterval));
            }
        }
    }

    public void ReplacePart(DamageInfo info)
    {
        if (m_rigidbody == null ||  m_collider == null) return;

        m_rigidbody.isKinematic = false;
        m_collider.enabled = true;

        transform.parent = null;
        ExplodePart(info.AttackDirection);

        if (m_coroutine == null)
            m_coroutine = StartCoroutine(DestroyPart(m_initInterval));
    }

    private void ExplodePart(Vector3 attackDirection)
    {
        //m_rigidbody.AddExplosionForce(m_explosionForce, explosionPosition, m_explosionRadius);
        m_rigidbody.AddForce(attackDirection*m_explosionForce,ForceMode.Impulse);
    }

    private IEnumerator DestroyPart(float interver)
    { 
        yield return new WaitForSeconds(interver);
        Destroy(gameObject);
    }
}
