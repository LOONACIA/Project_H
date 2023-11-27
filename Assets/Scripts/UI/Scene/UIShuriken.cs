using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOONACIA.Unity.UI;
using System;
using UnityEngine.UI;

public class UIShuriken : UIScene
{
    private enum Images
    {
        ShurikenImage,
        PossessionImage,
        PossessionGagueOutline,
        PossessionGague
    }
    
    private PossessionProcessor m_processor;

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    public void SetPossessionProcessor(PossessionProcessor processor)
    {
        UnregisterEvents();
        m_processor = processor;
        RegisterEvents();
    }
    
    protected override void Init()
    {
        base.Init();
        
        Bind<Image, Images>();
    }

    private void RegisterEvents()
    {
        if (m_processor != null)
        {
            m_processor.Possessable += OnPossessable;
            m_processor.TargetHit += OnTargetHit;
            m_processor.Possessed += OnPossessed;
        }
    }
    
    private void UnregisterEvents()
    {
        if (m_processor != null)
        {
            m_processor.Possessable -= OnPossessable;
            m_processor.TargetHit -= OnTargetHit;
            m_processor.Possessed -= OnPossessed;
        }
    }

    private void OnTargetHit(object sender, float _time)
    {
        Get<Image, Images>(Images.ShurikenImage).gameObject.SetActive(false);
        
        var possessionImage = Get<Image, Images>(Images.PossessionImage);
        possessionImage.gameObject.SetActive(true);

        Color color = possessionImage.color;
        color.a = 0.5f; // 타겟에 표창 적중 시의 알파 값
        possessionImage.color = color;
        StartCoroutine(nameof(IE_PossessableGauge), _time);
    }

    private void OnPossessable(object sender, EventArgs e)
    {
        var possessionImage = Get<Image, Images>(Images.PossessionImage);

        Color color = possessionImage.color;
        color.a = 1; // 빙의가 가능할 때의 알파 값
        possessionImage.color = color;
    }
    
    private void OnPossessed(object sender, Actor e)
    {
        Get<Image, Images>(Images.ShurikenImage).gameObject.SetActive(true);
        Get<Image, Images>(Images.PossessionImage).gameObject.SetActive(false);

        var possessionGague = Get<Image, Images>(Images.PossessionGague);

        possessionGague.fillAmount = 0f;
    }

    private IEnumerator IE_PossessableGauge(float _time)
    {
        var possessionGague = Get<Image, Images>(Images.PossessionGague);
        float curTime = 0f;

        while (true)
        {
            possessionGague.fillAmount = curTime / _time;
            yield return null;
            curTime += Time.deltaTime;

            if (curTime >= _time)
                break;
        }
    }
}
