using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class EffectManager
{
	// TODO: Data로 분리
	private GameObject m_bloodEffect;
	// TODO END //
	private Volume m_volume;
	
	private GameObject m_possessionTargetObj;
	
	private GameObject m_targetCameraPivot;

	private ColorAdjustments m_colorAdjustments;
	
	private Vignette m_vignette;
	
	public void Init()
	{
        m_bloodEffect = GameManager.Settings.BloodEffect;
        
		m_volume = GameObject.Find("Global Volume").GetComponent<Volume>();
		m_volume.profile.TryGet(out m_colorAdjustments);
		m_volume.profile.TryGet(out m_vignette);
	}
	
    /// <summary>
    /// 빙의 준비 시작 이펙트를 실행합니다.
    /// </summary>
	public void ShowPreparePossessionEffect()
    {
        GameManager.UI.ShowCrosshair();
		m_colorAdjustments.saturation.Override(-100f);
	}
    
    /// <summary>
    /// 빙의 준비 완료 이펙트를 실행합니다.
    /// </summary>
    public void ShowBeginPossessionEffect()
    {
        GameManager.UI.HideCrosshair();
    }
    
    /// <summary>
    /// 빙의 실패 이펙트를 실행합니다.
    /// </summary>
    public void ShowPossessionFailEffect()
    {
        ClearColorAdjustments();
    }

    /// <summary>
    /// 빙의 시작 이펙트를 실행합니다.
    /// </summary>
	public void ShowPossessionStartEffect()
	{
		GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = 150f;

		// Close Eye Effect
		Utility.Lerp(0, 1, 1f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
	}
	
    /// <summary>
    /// 빙의 성공 이펙트를 실행합니다.
    /// </summary>
	public void ShowPossessionSuccessEffect()
	{
		ClearColorAdjustments();
		
		GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = 60f;

		// Open Eye Effect
		Utility.Lerp(1, 0, 1f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
	}
    
    /// <summary>
    /// 출혈 이펙트를 실행합니다.
    /// </summary>
    /// <param name="monster">이펙트 실행 위치</param>
    /// <param name="rotaion">이펙트 방향</param>
    /// <param name="duration">이펙트 재생 시간</param>
    public void PlayBloodEffect(GameObject monster, Quaternion rotaion, float duration = 0)
    {
        // 출혈 이펙트 오브젝트 생성
        GameObject go = ManagerRoot.Resource.Instantiate(m_bloodEffect, monster.transform.position + Vector3.up, rotaion);

        // 이펙트 설정
        go.transform.SetParent(monster.transform);
        BloodEffect effect = go.GetOrAddComponent<BloodEffect>();
        effect.Show(monster.transform.position, duration);
    }
	
	private void ClearColorAdjustments()
	{
		m_colorAdjustments.saturation.Override(0f);
	}

    #region Camera
    /// <summary>
    /// 공격 시작에 따른 카메라 쉐이크 이펙트를 실행합니다.
    /// </summary>
    public void CameraShakeAttackStart()
    {
        //GameManager.Camera.Animator.Play(ConstVariables.CAMERASHAKE_GOBLINNORMALATTACKSTART_ANIMATION_NAME);
    }

    /// <summary>
    /// 공격 적중에 따른 카메라 쉐이크 이펙트를 실행합니다.
    /// </summary>
    public void CameraShakeAttackStop()
    {
        GameManager.Camera.Animator.SetTrigger("AttackHit");
    }

    /// <summary>
    /// 스킬 시작에 따른 카메라 쉐이크 이펙트를 실행합니다.
    /// </summary>
    public void CameraShakeSkillStart()
    {
        GameManager.Camera.Animator.Play(ConstVariables.CAMERASHAKE_GOBLINNORMALSKILLSTART_ANIMATION_NAME);
    }
    #endregion
}
