using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

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

    private AnalogGlitchVolume m_analogGlitchVolume;

    private DigitalGlitchVolume m_digitalGlithVolume;

    private ChromaticAberration m_chromaticAberration;
	
	public void Init()
	{
        m_bloodEffect = GameManager.Settings.BloodEffect;
        
		m_volume = GameObject.Find("Global Volume").GetComponent<Volume>();
		m_volume.profile.TryGet(out m_colorAdjustments);
		m_volume.profile.TryGet(out m_vignette);
        m_volume.profile.TryGet(out m_analogGlitchVolume);
        m_volume.profile.TryGet(out m_digitalGlithVolume);
        m_volume.profile.TryGet(out m_chromaticAberration);
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
        m_vignette.color.value = new Color(0f, 0f, 0f);
		Utility.Lerp(0, 1, 1f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
        Time.timeScale = 0f;
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
        Time.timeScale = 1f;
    }
    
    public void ShowDetectionWarningEffect()
    {
        GameManager.UI.ShowWarning(2f, 1f);
    }

    public void ShowHitVignetteEffect()
    {
        m_vignette.color.value = new Color(255/255f, 83/255f, 82/255f);
        m_vignette.intensity.value = 0.4f;

        Utility.Lerp(0.3f, 0, 1f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);

    }

    /// <summary>
    /// 출혈 이펙트를 실행합니다.
    /// </summary>
    /// <param name="monster">이펙트 실행 위치</param>
    /// <param name="rotation">이펙트 방향</param>
    /// <param name="duration">이펙트 재생 시간</param>
    public void PlayBloodEffect(GameObject monster, Quaternion rotation, float duration = 0)
    {
        // 출혈 이펙트 오브젝트 생성
        GameObject go = ManagerRoot.Resource.Instantiate(m_bloodEffect, monster.transform.position + Vector3.up, rotation);

        // 이펙트 설정
        go.transform.SetParent(monster.transform);
        BloodEffect effect = go.GetOrAddComponent<BloodEffect>();
        effect.Show(monster.transform.position, duration);
    }
	
    public void PlayBrokenBodyViewEffect()
    {
        m_analogGlitchVolume.scanLineJitter.value = 0f;
        m_analogGlitchVolume.verticalJump.value = 0.01f;

        m_digitalGlithVolume.intensity.value = 0.1f;

        m_chromaticAberration.intensity.value = 1f;
    }

    public void StopBrokenBodyViewEffect()
    {
        m_analogGlitchVolume.scanLineJitter.value = 0f;
        m_analogGlitchVolume.verticalJump.value = 0f;

        m_digitalGlithVolume.intensity.value = 0f;

        m_chromaticAberration.intensity.value = 0f;
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
