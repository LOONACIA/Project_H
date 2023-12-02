using INab.Dissolve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGate : MonoBehaviour, IGate
{
    [SerializeField]
    private GameObject m_rightGate;
    
    [SerializeField]
    private GameObject m_leftGate;

    [SerializeField, Range(0, 20)]
    private float m_moveDegree = 3;

    [SerializeField]
    private float m_progressTime;

    public IEnumerator Close()
    {
        yield break;
    }

    public IEnumerator Open()
    {
        Vector3 rightGateOriginPos = m_rightGate.transform.localPosition;
        Vector3 rightGateDestinationPos = rightGateOriginPos + Vector3.right * m_moveDegree;

        Vector3 leftGateOriginPos = m_leftGate.transform.localPosition;
        Vector3 leftGateDestinationPos = leftGateOriginPos + Vector3.left * m_moveDegree;

        float time = 0;

        while (time < m_progressTime)
        {
            time += Time.deltaTime;

            m_rightGate.transform.localPosition = Vector3.Lerp(rightGateOriginPos, rightGateDestinationPos, time / m_progressTime);
            m_leftGate.transform.localPosition = Vector3.Lerp(leftGateOriginPos, leftGateDestinationPos, time / m_progressTime);

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 rightGatePos = m_rightGate.transform.position + transform.TransformDirection(Vector3.right * m_moveDegree);
        Vector3 rightGateSize = m_rightGate.transform.localScale;
        
        Vector3 leftGatePos = m_leftGate.transform.position + transform.TransformDirection(Vector3.left * m_moveDegree);
        Vector3 leftGateSize = m_leftGate.transform.localScale;

        Gizmos.matrix = Matrix4x4.TRS(m_rightGate.transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.right * m_moveDegree, rightGateSize);

        Gizmos.matrix = Matrix4x4.TRS(m_leftGate.transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.left * m_moveDegree, leftGateSize);
    }
}
