using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossStageMachine : InteractableObject
{
    [SerializeField]
    private JumpPad m_jumpPad;

    private void Start()
    {
        if (m_jumpPad == null)
            m_jumpPad = GetComponentInChildren<JumpPad>(true);
    }

    protected override void OnInteract(Actor actor)
    {
        if (m_jumpPad == null)
        {
            Debug.Log($"{nameof(JumpPad)}가 존재하지 않음!");
            return;
        }

        // 점프 패드 활성화
        m_jumpPad.gameObject.SetActive(true);
        m_jumpPad.Activate();
    }
}
