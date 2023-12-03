using LOONACIA.Unity.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : UIScene
{
    private enum Images
    {
        SkillGagueOutline,
        SkillGague
    }

    private ActorStatus m_actorSuatus;

    private Image m_skillGague;

    private Canvas m_canvas;

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    public void SetActorStatus(PlayerController player, PossessionProcessor processor)
    {
        m_actorSuatus = player.Character.Status;
        
        RegisterEvents();

        processor.Possessed += ChangeCharacter;
    }

    protected override void Init()
    {
        base.Init();

        Bind<Image, Images>();

        m_skillGague = Get<Image, Images>(Images.SkillGague);
        m_canvas = GetComponent<Canvas>();
    }

    private void RegisterEvents()
    {
        if (m_actorSuatus != null)
        {
            m_actorSuatus.SkillCoolTimeChanged += UpdateGauge;
        }
    }

    private void UnregisterEvents()
    {
        if (m_actorSuatus != null)
        {
            m_actorSuatus.SkillCoolTimeChanged -= UpdateGauge;
        }
    }

    private void UpdateGauge(object sender, float e)
    {
        if (m_skillGague == null)
        {
            return;
        }

        m_skillGague.fillAmount = e;
    }

    private void ChangeCharacter(object sender, Actor e)
    {
        if (e == null) return;

        // 기존 캐릭터에서 이벤트 제거 
        UnregisterEvents();

        // 새로운 캐릭터에 이벤트 등록
        m_actorSuatus = e.Status;
        RegisterEvents();

        // 게이지 초기화
        m_skillGague.fillAmount = m_actorSuatus.SkillCoolTime;

        if (e.GetComponent<MonsterAttack>().SkillWeapon == null)
        {
            m_canvas.enabled = false;
        }
        else
        { 
            m_canvas.enabled = true;
        }
    }
}
