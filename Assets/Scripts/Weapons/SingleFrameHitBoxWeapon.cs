using System.Linq;
using UnityEngine;

public class SingleFrameHitBoxWeapon : Weapon
{
    //About HitBox
    [Header("HitBox의 기준이 될 Transform을 정해줍니다. 할당하지 않을 경우 현재 오브젝트의 트랜스폼이 자동으로 할당됩니다.")]
    public Transform hitBoxBaseTransform = null;
    [Header("공격 범위를 나타냅니다.")]
    public HitBox hitBox;
    
    //About AttackDirection
    [Header("AttackDirection의 기준이 될 Transform을 정해줍니다. 할당하지 않을 경우 HitBoxTransform으로 설정됩니다.")]
    public Transform attackDirectionBaseTransform;
    [Header("공격의 방향성을 설정해줍니다.")]
    public Vector3 attackDirection = Vector3.forward;


    private void InitTransforms()
    {
        if(hitBoxBaseTransform==null)
            hitBoxBaseTransform = transform;
        if (attackDirectionBaseTransform == null)
            attackDirectionBaseTransform = hitBoxBaseTransform;
    }

    protected override void Attack()
    {
        
    }

    protected override void OnHitMotion()
    {
        Vector3 dir = transform.TransformDirection(attackDirection);
        
        var detectedList = hitBox.DetectHitBox(hitBoxBaseTransform)
                                 .Select(info => new WeaponAttackInfo(info,dir.normalized))
                                 ;

        if (detectedList.Any())
        {
            InvokeHitEvent(detectedList);
        }
    }

    public void OnDrawGizmosSelected()
    {
        hitBox.DrawGizmo(hitBoxBaseTransform);
        Gizmos.matrix = Matrix4x4.TRS(
            attackDirectionBaseTransform.position, 
            attackDirectionBaseTransform.rotation, 
            attackDirectionBaseTransform.localScale);
        Gizmos.DrawLine(Vector3.zero,attackDirection);
    }
    
    
    private void OnValidate()
    {
        InitTransforms();
    }

    private void Awake()
    {
        InitTransforms();
    }
}
