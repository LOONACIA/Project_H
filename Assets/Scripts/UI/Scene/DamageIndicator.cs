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
    private Actor m_attacker;
    private Actor m_victim;

    

    private Quaternion m_rotation;
    private float m_angle;
    #endregion

    #region PublicMethod
    public void Init(DamageInfo _info, Actor _sender)
    {
        m_attacker = _info.Attacker;
        m_victim = _sender;

        TraceTarget();

        StartCoroutine(nameof(IE_FadeOut));
    }

    private void Update()
    {
        TraceTarget();
    }
    #endregion

    #region PrivateMethod
    private void TraceTarget()
    {
        transform.rotation = Quaternion.Euler(0, 0, CaculateAngle());
    }

    private float CaculateAngle()
    {
        Vector3 posA = m_victim.transform.position;
        Vector3 posB = m_attacker.transform.position;

        Vector2 viewVector = new Vector2(m_victim.transform.forward.x, m_victim.transform.forward.z);

        Vector2 from = new Vector2(posA.x, posA.z);
        Vector2 to = new Vector2(posB.x, posB.z);

        to = to - from;

        return Vector2.SignedAngle(viewVector, to) + 90;
    }

    private IEnumerator IE_FadeOut()
    {
        yield return new WaitForSeconds(1f);

        Color color = indicatorImage.color;
        float alpha = color.a;
        float value = alpha / (alpha * fadeOutTime * 30);
        while (true)
        {
            color = new Color(color.r, color.g, color.b, alpha);
            indicatorImage.color = color;
            alpha -= value;
            yield return new WaitForSeconds(1/30);

            if (alpha <= 0)
                break;
        }

        Destroy(gameObject);
    }
    #endregion
}
