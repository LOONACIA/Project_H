using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using LOONACIA.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectManager
{
	// TODO: Data로 분리
	private float m_lensDistortionEffectVariable = 0.04f;
	
	public GameObject targetEffect;
	
	public GameObject bloodEffect;
	// TODO END //
	
	private Volume m_volume;
	
	private GameObject m_possessionTargetObj;
	
	private GameObject m_targetCameraPivot;

	private ColorAdjustments m_colorAdjustments;
	
	private Vignette m_vignette;
	
	public void Init()
	{
		m_volume = GameObject.Find("Global Volume").GetComponent<Volume>();
		m_volume.profile.TryGet(out m_colorAdjustments);
		m_volume.profile.TryGet(out m_vignette);
	}
	
	public void ShowPreparePossessionEffect()
	{
		m_colorAdjustments.saturation.Override(-100f);
	}

	public void ShowPossessionStartEffect()
	{
		GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = 150f;

		// Close Eye Effect
		Utility.Lerp(0, 1, 1f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
	}
	
	public void ShowPossessionFailEffect()
	{
		ClearColorAdjustments();
	}
	
	public void ShowPossessionSuccessEffect()
	{
		ClearColorAdjustments();
		
		GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = 60f;

		// Open Eye Effect
		Utility.Lerp(1, 0, 1f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
	}
	
	private void ClearColorAdjustments()
	{
		m_colorAdjustments.saturation.Override(0f);
	}
}
