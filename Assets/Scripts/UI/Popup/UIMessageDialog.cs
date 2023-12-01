using LOONACIA.Unity.Coroutines;
using LOONACIA.Unity.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIMessageDialog : UIPopup
{
    private enum Texts
    {
        MessageText
    }
    
    private CoroutineEx m_typeCoroutine;

    private TextMeshProUGUI m_textBox;

    private string m_text;
    
    public bool IsTyping => m_typeCoroutine?.IsRunning is true;

    public void SetText(string text, Action onFinish = null)
    {
        m_typeCoroutine?.Abort();
        m_text = text;
        m_typeCoroutine = CoroutineEx.Create(this, Type(text, ConstVariables.UI_DIALOG_TEXT_TYPE_INTERVAL, onFinish));
    }
    
    public void Abort()
    {
        m_typeCoroutine?.Abort();
        if (m_textBox != null)
        {
            m_textBox.text = m_text;
        }
    }

    protected override void Init()
    {
        Bind<TextMeshProUGUI, Texts>();

        m_textBox = Get<TextMeshProUGUI, Texts>(Texts.MessageText);
    }
    
    private IEnumerator Type(string text, float interval, Action onFinish)
    {
        int cursor = text.Length;

        while (cursor >= 0)
        {
            yield return new WaitForSeconds(interval);
            m_textBox.text = text[..^cursor--];
        }
        
        onFinish?.Invoke();
    }
}
