using System;
using System.Transactions;
using UnityEngine;

public class BossStageRoot : MonoBehaviour
{
    public Boss boss;

    [SerializeField]
    private BossStagePhase[] m_bossStagePhaseList;

    [SerializeField, Tooltip("플레이어를 날려버릴 위치")]
    private Transform m_throwPosition;

    [SerializeField, Tooltip("플레이어를 날려버릴떄 이동, 대쉬 불가능하게 만드는 시간")]
    private float m_throwKnockDownTime;

    [SerializeField]
    private GameObject m_fence;

    [SerializeField]
    private GameObject m_detectionLight;

    private Actor m_character;

    private int m_currentPhase;

    private int m_lastPhase;

    private void Start()
    {
        foreach (var phase in m_bossStagePhaseList)
        {
            phase.PhaseEnd += NextPhase;
        }

        m_character = GameManager.Character.Controller.Character;
    }

    public void StageStart()
    {
        m_currentPhase = 0;
        m_lastPhase = m_bossStagePhaseList.Length - 1;

        boss.gameObject.SetActive(true);

        // Stage 시작하면 플레이어를 지정한 방향으로 날림
        ThrowPlayer();
        
        // Stage 주변을 감쌀 울타리 활성화
        m_fence?.SetActive(true);

        m_detectionLight?.SetActive(true);

        m_bossStagePhaseList[m_currentPhase]?.ReadyPhase();
    }

    private void NextPhase(object sender, EventArgs e)
    {
        if (m_currentPhase >= m_lastPhase)
        {
            StageClear();
            return;
        }

        m_bossStagePhaseList[++m_currentPhase].ReadyPhase();
    }

    private void StageClear()
    {
        boss.Kill();
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
