using System;
using UnityEngine;

public class BossStageRoot : MonoBehaviour
{
    [SerializeField]
    private Boss m_boss;

    [SerializeField]
    private BossStagePhase[] m_bossStagePhaseList;

    [Header("Start Event")]
    [SerializeField, Tooltip("플레이어를 날려버릴 위치")]
    private Transform m_throwPosition;

    [SerializeField, Tooltip("플레이어를 날려버릴떄 이동, 대쉬 불가능하게 만드는 시간")]
    private float m_throwKnockDownTime;

    [SerializeField]
    private Explosive m_dummyMachine;

    [SerializeField]
    private GameObject m_detectionLight;

    [Header("End Event")]
    [SerializeField, Tooltip("플레이어가 마지막 상호작용한 후, 게임이 끝날 때까지의 지연 시간")]
    private float m_clearDelayTime;

    private Actor m_character;

    private int m_currentPhase;

    private int m_lastPhase;


    private void Start()
    {
        foreach (var phase in m_bossStagePhaseList)
        {
            phase.PhaseEnd += OnPhaseEnd;
        }

        m_character = GameManager.Character.Controller.Character;
    }

    public void StageStart()
    {
        m_currentPhase = 0;
        m_lastPhase = m_bossStagePhaseList.Length - 1;

        m_boss.gameObject.SetActive(true);

        // Stage 시작하면 플레이어를 지정한 방향으로 날림
        ThrowPlayer();

        m_dummyMachine?.Explode(20, transform.forward * 5f, 100, 10);
        m_detectionLight?.SetActive(true);

        m_bossStagePhaseList[m_currentPhase]?.ReadyPhase();
    }

    private void OnPhaseEnd(object sender, EventArgs e)
    {
        m_boss.TakeDamage(new AttackInfo());
        
        NextPhase();
    }

    private void NextPhase()
    {
        if (m_currentPhase >= m_lastPhase)
        {
            m_boss.Kill();
            return;
        }

        m_bossStagePhaseList[++m_currentPhase].ReadyPhase();
    }

    private void ThrowPlayer()    {
        if (m_character.TryGetComponent<ActorStatus>(out var status))
        {
            status.SetKnockDown(m_throwKnockDownTime);
            status.IsFlying = true;
        }

        if (m_character.TryGetComponent<Rigidbody>(out var rigidbody))
        {
            var targetVector = m_throwPosition.position - transform.position;
            var direction = targetVector.normalized;
            var force = targetVector.magnitude;

            rigidbody.AddForce(direction * force, ForceMode.VelocityChange);
        }
    }
}
