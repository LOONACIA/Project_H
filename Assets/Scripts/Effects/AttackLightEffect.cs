using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;

public class AttackLightEffect : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    
    private WaitForSeconds m_waitForSecondsCache = new(0.1f);

    private float m_duration = 0.1f;
    
    private CoroutineEx m_coroutine;
    
    private Light m_light;
    
    #endregion

    public float Duration
    {
        get => m_duration;
        set
        {
            if (Mathf.Approximately(value, m_duration))
            {
                return;
            }
            
            m_coroutine?.Abort();
            m_duration = value;
            m_waitForSecondsCache = new(m_duration);
            m_coroutine = CoroutineEx.Create(this, IE_ReleaseSelf());
        }
    }

    public float Intensity
    {
        set => m_light.intensity = value;
    }

    private void Awake()
    {
        m_light = GetComponent<Light>();
    }

    #region PublicMethod
    private void OnEnable()
    {
        m_coroutine = CoroutineEx.Create(this, IE_ReleaseSelf());
    }

    private void OnDisable()
    {
        m_coroutine?.Abort();
    }

    #endregion

    #region PrivateMethod
    private IEnumerator IE_ReleaseSelf()
    {
        
        yield return m_waitForSecondsCache;
        ManagerRoot.Resource.Release(gameObject);
    }
    #endregion
}
