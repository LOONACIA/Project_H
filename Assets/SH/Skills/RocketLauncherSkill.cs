using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherSkill : Skill
{
    public MonsterAttackData data;
    public GameObject launchVfx;
    public RocketProjectile rocketProjectile;

    private Animator m_animator;
    private Monster m_caster;
    
    public override void Cast(Monster caster, Animator animator)
    {
        //초기화
        m_caster = caster;
        m_animator = animator;

        //애니메이션 실행
        m_animator.SetTrigger("Skill");
    }

    #region AnimationEvent

    public override void OnSkillVfxEvent()
    {
        launchVfx.SetActive(false);
        launchVfx.SetActive(true);
    }

    public override void OnSkillAnimationEvent()
    {
        //런치 시 이펙트 생성
        //launchVfx.SetActive(false);
        //launchVfx.SetActive(true);

        RocketProjectile rp = Instantiate(rocketProjectile, transform.position, transform.rotation);
        rp.direction = transform.forward;

        //EffectManager.instance.CameraShakeGoblinAttackStop();
    }
    
    public override void OnSkillAnimationEnd()
    {
        
    }

    #endregion

    #region UnityFunction

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    #endregion

}
