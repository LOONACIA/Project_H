using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using LOONACIA.Unity;
using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using URPGlitch.Runtime.AnalogGlitch;
using URPGlitch.Runtime.DigitalGlitch;

public class EffectManager
{
    private readonly WaitForSeconds m_waitForSecondsCache = new(0.1f);
    
    private GameObject m_sparkEffect;

    private GameObject m_dashEffect;

    private float m_dashFOV = 63f;
    
	private Volume m_volume;
	
	private GameObject m_possessionTargetObj;
	
	private GameObject m_targetCameraPivot;

	private ColorAdjustments m_colorAdjustments;
	
	private Vignette m_vignette;

    private AnalogGlitchVolume m_analogGlitchVolume;

    private DigitalGlitchVolume m_digitalGlithVolume;

    private ChromaticAberration m_chromaticAberration;
    
    private CoroutineEx m_hitVignetteCoroutine;


    private float m_originalTimeScale;
    private float m_originalFixedDeltaTime;

    private CoroutineEx m_timeScaleUpdateCoroutine = null;
    private float m_timeScaleUpdateEndTime;
    private float m_minimumTimeScale = 1f;

    public void Init()
	{
        m_sparkEffect = GameManager.Settings.SparkEffect;

        m_dashEffect = GameManager.Settings.DashEffect;
        //m_dashEffect = GameObject.Find("DashEffect");
        m_dashEffect.SetActive(false);

        m_originalTimeScale = Time.timeScale;
        m_originalFixedDeltaTime = Time.fixedDeltaTime;

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
        
        var volume = GameObject.Find("Global Volume");
        if (volume == null)
        {
            return;
        }
        m_volume = volume.GetComponent<Volume>();
        m_volume.profile.TryGet(out m_colorAdjustments);
        m_volume.profile.TryGet(out m_vignette);
        m_volume.profile.TryGet(out m_analogGlitchVolume);
        m_volume.profile.TryGet(out m_digitalGlithVolume);
        m_volume.profile.TryGet(out m_chromaticAberration);
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
        m_hitVignetteCoroutine?.Abort();
        m_vignette.color.value = new(255/255f, 83/255f, 82/255f);
        m_vignette.intensity.value = 0.4f;

        m_hitVignetteCoroutine = CoroutineEx.Create(GameManager.Instance, Recovery());
        return;

        IEnumerator Recovery()
        {
            yield return m_waitForSecondsCache;
            RecoverVignetteEffect();
        }
    }

    public void ShowDashEffect()
    {
        //m_dashEffect.SetActive(true);
        // 스파크 이펙트 오브젝트 생성
        Vector3 pos = Vector3.zero; //new Vector3(0, 0, 2.5f);
        GameObject go = ManagerRoot.Resource.Instantiate(m_dashEffect, Camera.main.transform);
        go.transform.localPosition = new Vector3(0, 0, 2.5f);
        go.transform.localEulerAngles = Vector3.zero;

        GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = m_dashFOV;

        GameManager.Instance.StartCoroutine(IE_EndDashEffect());
        return;

        IEnumerator IE_EndDashEffect()
        {
            yield return new WaitForSeconds(0.15f);
            GameManager.Camera.CurrentCamera.m_Lens.FieldOfView = 60f;
            yield return new WaitForSeconds(0.20f);
            //m_dashEffect.SetActive(false);
            ManagerRoot.Resource.Release(go);
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
        m_hitVignetteCoroutine = Utility.Lerp(0.5f, 0, 1f, value => m_vignette.intensity.Override(value), ignoreTimeScale: true);
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
    /// </summary>ssss
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

    public void ChangeTimeScale(MonoBehaviour caller, float targetTimeScale, float duration)
    {
        this.m_minimumTimeScale = targetTimeScale;
        m_timeScaleUpdateEndTime = Time.unscaledTime + duration;
        if (m_timeScaleUpdateCoroutine == null)
        {
            //TODO: Coroutine 동작 중 해킹을 하자마자 원래 몸이 죽으면 TimeScale이 초기화되지 않을 가능성이 있음
            m_timeScaleUpdateCoroutine = CoroutineEx.Create(caller, IE_UpdateTimeScale());
        }
    }

    public IEnumerator IE_UpdateTimeScale()
    {
        //TimeScale 변화 속도
        float ascendingSpeed = 10f;
        float descendingSpeed = 15f;

        //fixed delta time과 timescale간 비율
        float fdtRatio = m_originalFixedDeltaTime / m_originalTimeScale;

        while (true)
        {
            //현재 타임스케일이 목표보다 크다면, 타임스케일을 줄여줍니다.
            if (Time.timeScale > m_minimumTimeScale)
            {
                //타임스케일을 정해진 속도에 맞게 줄여줍니다. fixedDeltaTime도 함께 줄입니다.
                float temp = Time.timeScale - descendingSpeed * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(temp, m_minimumTimeScale, m_originalTimeScale);
                Time.fixedDeltaTime = Time.timeScale * fdtRatio;
            }
            //현재 타임스케일이 목표보다 작다면, 타임스케일을 늘려줍니다.
            if (Time.timeScale < m_minimumTimeScale)
            {
                //타임스케일을 정해진 속도에 맞게 줄여줍니다. fixedDeltaTime도 함께 줄입니다.
                float temp = Time.timeScale + ascendingSpeed * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(temp, 0f, m_minimumTimeScale);
                Time.fixedDeltaTime = Time.timeScale * fdtRatio;
            }

            //목표한 종료시간이 왔다면 종료
            if (m_timeScaleUpdateEndTime <= Time.unscaledTime)
            {
                m_minimumTimeScale = m_originalTimeScale;
                if(Time.timeScale == m_originalTimeScale)
                {
                    Debug.Log($"시간 정지 종료");
                    Time.timeScale = m_originalTimeScale;
                    Time.fixedDeltaTime = m_originalFixedDeltaTime;
                    m_timeScaleUpdateCoroutine = null;
                    break;
                }
            }
            yield return null;
        }
    }
    #endregion
}
