using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{

    public MonsterAttackData data;

    [SerializeField] private HitBox m_attackHitBox;

    private Animator m_animator;
    private Monster m_caster;

    public void Cast(Monster caster)
    {
        m_caster = caster;
        //1. 애니메이터의 실행
        m_animator.SetTrigger("Skill");
        //2. 공격 VFX의 출력
        //3. 공격 중인지(isAttacking) 체크
    }

    public void OnSkillAnimationEvent()
    {
        //TODO: 본인이라면 카메라 흔들기
        //TODO: CC기, 데미지 적용

        bool isCheck = false;

        //히트박스를 체크
        var hitObjects = m_attackHitBox.DetectHitBox(transform);

        //공격 체크
        foreach (var health in hitObjects)
        {
            if (health.gameObject == gameObject) { continue; }

            // 빙의되지 않은 몬스터가 타겟이 아닌 대상을 공격하는 경우
            if (!m_caster.IsPossessed &&
                health.gameObject.TryGetComponent<Actor>(out var actor) && !m_caster.Targets.Contains(actor))
            {
                continue;
            }

            Debug.Log($"{health.gameObject.name} is hit by {gameObject.name}, damage: {3}");
            health.TakeDamage(3, m_caster);
        }

        //EffectManager.instance.CameraShakeGoblinAttackStop();
    }

    public void OnSkillAnimationEnd()
    {

    }

    #region Event Functions

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void OnDrawGizmos()
    {
        m_attackHitBox.DrawHitBoxGizmo(transform);
    }
    #endregion

}
