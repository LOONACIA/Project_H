using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Shield : MonoBehaviour
{
    // 쉴드량
    [SerializeField]
    private float m_shieldPoint = 30;

    // 쉴드 제한 시간 여부
    [SerializeField]
    private bool m_hasTimeLimit;

    // 쉴드 제한 시간
    [SerializeField]
    private float m_timeLimit;

    // 쉴드 생성한 시간
    private float m_startTime;


    // 쉴드 프리팹
    public GameObject ShieldObject { get; private set; }

    // 남은 쉴드량
    public float ShieldPoint
    {
        get => Mathf.Max(m_shieldPoint, 0);
        private set => m_shieldPoint = value;
    }

    public float MaxShieldPoint { get; private set; }

    public bool IsValid
    {
        get
        {
            // 쉴드 시간 제한이 있고, 지속 시간보다 오랫동안 존재했을 경우 false 반환
            if (m_hasTimeLimit && Time.time - m_startTime > m_timeLimit)
            {
                return false;
            }

            // 쉴드량을 모두 소모했을 경우 false 반환
            if (ShieldPoint <= 0)
            {
                return false;
            }

            return true;
        }
    }


    public event EventHandler ShieldChanged;


    public void Init(float shieldPoint, float timeLimit, GameObject shieldObject)
    {
        MaxShieldPoint = shieldPoint;
        ShieldPoint = MaxShieldPoint;
        m_timeLimit = timeLimit;

        // 쉴드 제한 시간이 0 이하면, 제한 시간이 없는 것으로 처리
        m_hasTimeLimit = (timeLimit > 0 ? true : false); 

        if (shieldObject != null)
            ShieldObject = shieldObject;

        m_startTime = Time.time;
    }

    public void TakeDamage(float damage)
    {
        ShieldPoint -= damage;
        ShieldChanged?.Invoke(this, null);
    }

    private void Start()
    {
        Init(m_shieldPoint, m_timeLimit, gameObject);
    }

}
