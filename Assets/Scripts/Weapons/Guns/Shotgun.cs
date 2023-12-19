using LOONACIA.Unity.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun")]
    [SerializeField]
    [Tooltip("발사체 프리팹")]
    private GameObject m_projectilePrefab;
    
    [SerializeField]
    private float m_shootForce;
    
    [SerializeField]
    [Tooltip("샷건의 탄 퍼짐 각도")]
    private float m_shotgunSpreadAngle = 10f;
    
    [SerializeField]
    [Tooltip("한 번에 발사할 탄환의 수")]
    private int m_shotgunBulletCount = 5;
    
    [SerializeField]
    [Tooltip("샷건의 사거리")]
    private float m_shotgunRange = 10f;

    protected override void Fire()
    {
        float distance = m_shotgunRange * 2;
        
        Ray ray = GetRay();
        bool isHit = Physics.Raycast(ray, out var hit, distance, m_aimLayers);
        Vector3 target = isHit ? hit.point : ray.GetPoint(distance);
        Vector3 spawnPosition = m_spawnPosition.position;
        Vector3 direction = (target - spawnPosition).normalized;
        for (int index = 0; index < m_shotgunBulletCount; index++)
        {
            Vector3 coneDirection = GetRandomConeDirection(direction, m_shotgunSpreadAngle);

            var projectile =
                ManagerRoot.Resource.Instantiate(m_projectilePrefab, spawnPosition, m_spawnPosition.rotation)
                    .GetComponent<Projectile>();
        
            projectile.Init(Owner.gameObject, Damage, m_shotgunRange, m_aimLayers, info => Hit(Enumerable.Repeat(info, 1)));
            projectile.Rigidbody.AddForce(coneDirection * m_shootForce, ForceMode.VelocityChange);
        }
    }
    
    private static Vector3 GetRandomConeDirection(Vector3 coneDirection, float maxAngle)
    {
        float angle = Random.Range(-maxAngle, maxAngle);
        Vector3 randomDirection = Random.insideUnitSphere;
        return Vector3.RotateTowards(coneDirection, randomDirection, Mathf.Deg2Rad * angle, 0.0f);
    }
}
