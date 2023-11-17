using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    //시전자 정보
    [HideInInspector]public GameObject owner = null;
    [HideInInspector]public Weapon shooter = null;
    
    
    public Vector3 direction = Vector3.forward;
    public float speed = 3.0f;
    public GameObject explosionVfx;

    public HitBox hitBox;
    
    private Rigidbody m_rigidbody;

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
                =hitBox.DetectHitBox(transform)
                               .Where(hit => hit.gameObject != owner.gameObject);

            //오브젝트가 하나라도 있다면?
            if (detectedObjects.Any())
            {
                AttackInfo info = new AttackInfo();
                info.damage = 5;
                info.attackDirection = direction;
                
                shooter.InvokeHitEvent(info, detectedObjects);
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
        hitBox.DrawGizmo(transform);
    }

}
