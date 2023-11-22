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
        PossessionImage
    }
    
    private PossessionProcessor m_processor;

    protected override void Init()
    {
        base.Init();
        
        Bind<Image, Images>();
    }

    public void SetPossessionProcessor(PossessionProcessor processor)
    {
        if (m_processor != null)
        {
            m_processor.Possessable -= OnPossessable;
            m_processor.TargetHit -= OnTargetHit;
            m_processor.Possessed -= OnPossessed;
        }

        m_processor = processor;
        m_processor.Possessable += OnPossessable;
        m_processor.TargetHit += OnTargetHit;
        m_processor.Possessed += OnPossessed;
    }

    private void OnTargetHit(object sender, EventArgs e)
    {
        Get<Image, Images>(Images.ShurikenImage).gameObject.SetActive(false);
        
        var possessionImage = Get<Image, Images>(Images.PossessionImage);
        possessionImage.gameObject.SetActive(true);

        Color color = possessionImage.color;
        color.a = 0.5f; // 타겟에 표창 적중 시의 알파 값
        possessionImage.color = color;
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
    }
}
