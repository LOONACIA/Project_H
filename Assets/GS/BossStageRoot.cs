using System;
using UnityEngine;

public class BossStageRoot : MonoBehaviour
{
    public Boss boss;

    [SerializeField]
    private BossStagePhase[] m_bossStagePhaseList;

    [SerializeField, Tooltip("플레이어를 날려버릴 위치")]
    private Vector3 m_throwPosition;

    [SerializeField]
    private GameObject m_fence;

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
        m_fence.SetActive(true);

        m_bossStagePhaseList[m_currentPhase].ReadyPhase();
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
        // 클리어
        boss.Kill();
        Debug.Log("game clear");
    }

    private void ThrowPlayer()
    {
        if (m_character.TryGetComponent<ActorStatus>(out var status))
        {
            status.IsFlying = true;
        }

        if (m_character.TryGetComponent<Rigidbody>(out var rigidbody))
        { 
            var direction = (m_throwPosition - transform.position).normalized;
            var force = Vector3.Distance(m_throwPosition, transform.position);

            rigidbody.AddForce(direction * force, ForceMode.VelocityChange);
        }
    }

    // test
    private bool active;
    private void Update()
    {

        if (Input.GetKey(KeyCode.Alpha0)) 
        {
            active = false;
            boss.Kill();
        }
    }
}
