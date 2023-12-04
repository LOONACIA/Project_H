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

        // 스킬을 가지고 있는 몬스터의 경우에만 표시
        m_canvas.enabled = HasSkill(player.Character) ? true : false;
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
        Debug.Log(m_skillGague.fillAmount);
    }

    private void ChangeCharacter(object sender, Actor actor)
    {
        if (actor == null) return;

        // 기존 캐릭터에서 이벤트 제거 
        UnregisterEvents();

        // 새로운 캐릭터에 이벤트 등록
        m_actorSuatus = actor.Status;
        RegisterEvents();

        // 게이지 초기화
        m_skillGague.fillAmount = m_actorSuatus.SkillCoolTime;

        // 스킬을 가지고 있는 몬스터의 경우에만 표시
        m_canvas.enabled = HasSkill(actor) ? true : false;
    }

    private bool HasSkill(Actor actor)
    {
        Monster monster = actor as Monster;
        if (monster != null && monster.Attack.SkillWeapon != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
