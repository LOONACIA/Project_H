using System.Linq;
using UnityEngine;

/*
 * 23.12.03: 기존 스킬 테스트용으로 제작한 로켓 런쳐 스킬입니다.
 * 연결된 프리팹이 있을 수 있어 프리프로덕션 제출 이후 확인 후 삭제 예정입니다.
 */

public class RocketProjectile : MonoBehaviour
{

    #region PublicVariables

    //시전자 정보
    [HideInInspector]public Weapon shooter = null;
    
    //방향성, 이동성
    public Vector3 direction = Vector3.forward;
    public float speed = 3.0f;
    
    //폭발 VFX
    public GameObject explosionVfx;

    #endregion

    #region PrivateVariables
    
    private Rigidbody m_rigidbody;

    #endregion
    public HitSphere hitSphere;
    

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_rigidbody.velocity = direction * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground")
            ||other.gameObject.layer==LayerMask.NameToLayer("Monster"))
        {
            //공격 판정
            var detectedObjects
                = hitSphere.DetectHitSphere(transform)
                           .Select(hit => new WeaponAttackInfo(
                                       hit,
                                       hit.gameObject.transform.position - transform.position
                                   ));

            //오브젝트가 하나라도 있다면?
            if (detectedObjects.Any())
            {
                shooter.InvokeHitEvent(detectedObjects);
            }
            
            //타격 이펙트
            GameObject v = Instantiate(explosionVfx, transform.position, transform.rotation);
            v.transform.localScale *= 5f;
            v.SetActive(true);
            Destroy(v, 2f);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        hitSphere.DrawGizmo(transform);
    }

}
