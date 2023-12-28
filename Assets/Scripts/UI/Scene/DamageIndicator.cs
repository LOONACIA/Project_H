using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    #region PublicVariables

    public Image indicatorImage;

    public float fadeOutTime;

    #endregion

    #region PrivateVariables

    private readonly WaitForSeconds m_coroutineWaitCache = new(1);

    private readonly WaitForSeconds m_waitForSecondsCache = new(1 / 30f);

    private GameObject m_attacker;
    private Actor m_victim;

    private Quaternion m_rotation;
    private float m_angle;
    
    private CoroutineEx m_fadeOutCoroutine;

    #endregion

    #region PublicMethod
    
    void OnDisable()
    {
        m_fadeOutCoroutine?.Abort();
    }

    public void Init(in AttackInfo _info, Actor _sender, Color color)
    {
        m_attacker = _info.Attacker;
        m_victim = _sender;

        indicatorImage.color = color;
        TraceTarget();

        if (isActiveAndEnabled)
        {
            m_fadeOutCoroutine = CoroutineEx.Create(this, IE_FadeOut());
        }
    }

    private void Update()
    {
        TraceTarget();
    }

    #endregion

    #region PrivateMethod

    private void TraceTarget()
    {
        transform.rotation = Quaternion.Euler(0, 0, CalculateAngle());
    }

    private float CalculateAngle()
    {
        if (m_attacker == null || m_victim == null)
        {
            ManagerRoot.Resource.Release(gameObject);
            return default;
        }

        Vector3 posA = m_victim.transform.position;
        Vector3 posB = m_attacker.transform.position;
        Vector3 forward = m_victim.transform.forward;

        Vector2 viewVector = new(forward.x, forward.z);

        Vector2 from = new(posA.x, posA.z);
        Vector2 to = new(posB.x, posB.z);

        to -= from;

        return Vector2.SignedAngle(viewVector, to) + 90;
    }

    private IEnumerator IE_FadeOut()
    {
        yield return m_coroutineWaitCache;

        Color color = indicatorImage.color;
        float alpha = color.a;
        float value = alpha / (alpha * fadeOutTime * 30);
        while (true)
        {
            color = new(color.r, color.g, color.b, alpha);
            indicatorImage.color = color;
            alpha -= value;
            yield return m_waitForSecondsCache;

            if (alpha <= 0)
            {
                break;
            }
        }

        ManagerRoot.Resource.Release(gameObject);
    }

    #endregion
}