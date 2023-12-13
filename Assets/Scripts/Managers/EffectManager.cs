using LOONACIA.Unity;
using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

public class EffectManager
{
    private GameObject m_sparkEffect;
    
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
        m_sparkEffect = GameManager.Settings.SparkEffect;

        InitComponents();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        InitComponents();
    }

    private void InitComponents()
    {
        if (m_volume != null)
        {
            return;
        }
        
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
		//GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = 150f;

        // Close Eye Effect
        const float duration = ConstVariables.HACKING_SUCCESS_EFFECT_DURATION;
        m_vignette.color.value = new Color(0f, 0f, 0f);
		Utility.Lerp(0, 1, duration, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
        Time.timeScale = 0f;
	}
	
    /// <summary>
    /// 빙의 성공 이펙트를 실행합니다.
    /// </summary>
	public void ShowPossessionSuccessEffect()
	{
		ClearColorAdjustments();
		
		//GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = 60f;

		// Open Eye Effect
        const float duration = ConstVariables.HACKING_SUCCESS_EFFECT_DURATION;
		Utility.Lerp(1, 0, duration, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
        GameManager.Instance.StartCoroutine(RevertTimeScale());
        return;
        
        IEnumerator RevertTimeScale()
        {
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }

    public void ShowGameOverEffect()
    {
        GameManager.UI.CloseSceneUI();
        var jitter = m_analogGlitchVolume.scanLineJitter;
        var verticalJump = m_analogGlitchVolume.verticalJump;
        var intensity = m_digitalGlithVolume.intensity;
        Utility.Lerp(jitter.value, 0.5f, 1f, value => jitter.value = value, ignoreTimeScale: true);
        Utility.Lerp(verticalJump.value, 0.5f, 1, value => verticalJump.value = value, ignoreTimeScale: true);
        Utility.Lerp(intensity.value, 1f, 1f, value => intensity.value = value, HideGameOverEffect, true);
    }

    public void HideGameOverEffect()
    {
        var jitter = m_analogGlitchVolume.scanLineJitter;
        var verticalJump = m_analogGlitchVolume.verticalJump;
        Utility.Lerp(jitter.value, 0f, 1f, value => jitter.value = value, ignoreTimeScale: true);
        Utility.Lerp(verticalJump.value, 0f, 1f, value => verticalJump.value = value, ignoreTimeScale: true);
    }
    
    public void ShowDetectionWarningEffect()
    {
        GameManager.UI.ShowWarning(2f, 1f);
    }

    public void ShowHitVignetteEffect()
    {
        m_vignette.color.value = new(255/255f, 83/255f, 82/255f);
        m_vignette.intensity.value = 0.4f;

        GameManager.Instance.StartCoroutine(Recovery());
        return;

        IEnumerator Recovery()
        {
            yield return new WaitForSeconds(0.5f);
            RecoverVignetteEffect();
        }
    }

    /// <summary>
    /// 스파크 이펙트를 실행합니다.
    /// </summary>
    /// <param name="monster">이펙트 대상</param>
    /// <param name="position">이펙트 위치</param>
    /// <param name="duration">이펙트 재생 시간</param>
    public void PlaySparkEffect(GameObject monster, Vector3 position, float duration = 0)
    {
        // 스파크 이펙트 오브젝트 생성
        GameObject go = ManagerRoot.Resource.Instantiate(m_sparkEffect, position, m_sparkEffect.transform.rotation);

        // 이펙트 실행
        go.transform.SetParent(monster.transform);
        GameManager.Instance.StartCoroutine(CoWait());
        return;

        IEnumerator CoWait()
        {
            yield return new WaitForSeconds(duration);
            ManagerRoot.Resource.Release(go);
        }
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

    private void RecoverVignetteEffect()
    {
        Utility.Lerp(0.5f, 0, 1.5f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
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
