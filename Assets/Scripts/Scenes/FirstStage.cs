using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstStage : MonoBehaviour
{
    [SerializeField]
    private AudioClip m_bgm;

    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Start()
	{
        if (m_bgm != null)
        {
            GameManager.Sound.Play(m_bgm, SoundType.Bgm);
        }
	}
}
