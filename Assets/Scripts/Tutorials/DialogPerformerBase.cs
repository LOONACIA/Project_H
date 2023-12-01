using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DialogPerformerBase : MonoBehaviour
{
    protected int m_dialogVersion;
    
    [SerializeField]
    [Tooltip("다이얼로그 메시지")]
	private MessageDialogInfo[] m_dialogTexts;

    [SerializeField]
    [Tooltip("다이얼로그 표시 간격")]
    private float m_interval = 1f;
    
    [SerializeField]
    [Tooltip("다이얼로그 유지 시간. 0보다 작거나 같으면 계속 유지")]
    private float m_dialogDuration;
    
    private int m_dialogIndex = -1;
    
    protected void ShowDialog()
    {
        m_dialogVersion = GameManager.UI.ShowDialog(m_dialogTexts, m_interval);
        if (m_dialogDuration > 0f)
        {
            CoroutineEx.Create(this, CoWaitForSeconds(m_dialogDuration, HideDialog));
        }
    }
    
    protected void HideDialog()
    {
        GameManager.UI.HideDialog(m_dialogVersion);
    }
    
    protected IEnumerator CoWaitForSeconds(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
