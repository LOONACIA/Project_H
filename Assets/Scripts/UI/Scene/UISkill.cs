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
        SkillGaugeOutline,
        SkillGauge
    }
    
    private PlayerController m_playerController;

    private ActorStatus m_actorStatus;

    private Image m_skillGauge;

    private Canvas m_canvas;

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    public void SetActorStatus(PlayerController playerController)
    {
        // Unregister events
        if (m_playerController != null)
        {
            m_playerController.CharacterChanged -= OnCharacterChanged;
        }
        UnregisterEvents();

        m_actorStatus = playerController.Character.Status;
        m_playerController = playerController;
        RegisterEvents();

        m_playerController.CharacterChanged += OnCharacterChanged;
    }

    protected override void Init()
    {
        base.Init();

        Bind<Image, Images>();

        m_skillGauge = Get<Image, Images>(Images.SkillGauge);
        m_canvas = GetComponent<Canvas>();
    }

    private void RegisterEvents()
    {
        if (m_actorStatus != null)
        {
            m_actorStatus.SkillCoolTimeChanged += OnSkillCollTimeChanged;
        }
    }

    private void UnregisterEvents()
    {
        if (m_actorStatus != null)
        {
            m_actorStatus.SkillCoolTimeChanged -= OnSkillCollTimeChanged;
        }
    }

    private void OnSkillCollTimeChanged(object sender, float e)
    {
        if (m_skillGauge is null)
        {
            return;
        }

        m_skillGauge.fillAmount = e;
    }

    private void OnCharacterChanged(object sender, Actor actor)
    {
        if (actor == null)
        {
            return;
        }

        // 기존 캐릭터에서 이벤트 제거 
        UnregisterEvents();

        // 새로운 캐릭터에 이벤트 등록
        m_actorStatus = actor.Status;
        RegisterEvents();

        // 게이지 초기화
        m_skillGauge.fillAmount = m_actorStatus.SkillCoolTime;

        // 스킬을 가지고 있는 몬스터의 경우에만 표시
        m_canvas.enabled = HasSkill(actor);
    }

    private bool HasSkill(Actor actor)
    {
        if (actor is not Monster monster)
        {
            return false;
        }

        return monster.Attack.SkillWeapon != null;
    }
}
