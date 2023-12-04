using LOONACIA.Unity.Coroutines;
using UnityEngine;

public class TriggerDialogPerformer : DialogPerformerBase
{
    [SerializeField]
    [Tooltip("트리거 조건 충족 후 다이얼로그 팝업 대기 시간")]
    private float m_waitTime;

    private bool m_isPerformed;
    
	protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Actor>(out var actor) || !actor.IsPossessed)
        {
            return;
        }

        if (m_isPerformed)
        {
            return;
        }

        CoroutineEx.Create(this, CoWaitForSeconds(m_waitTime, Perform));
    }

    protected virtual void OnPerformed()
    {
        ShowDialog();
    }

    private void Perform()
    {
        OnPerformed();
        m_isPerformed = true;
    }
}
