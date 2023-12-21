using System;
using UnityEngine;

public class BossStageRoot : MonoBehaviour
{
    public GameObject boss;

    [SerializeField]
    private BossStagePhase[] m_bossStagePhaseList;

    private int m_currentPhase;

    private int m_lastPhase;

    private void Start()
    {
        foreach (var phase in m_bossStagePhaseList)
        {
            phase.PhaseEnd += NextPhase;
        }
    }

    public void StageStart()
    {
        m_currentPhase = 0;
        m_lastPhase = m_bossStagePhaseList.Length - 1;

        boss.SetActive(true);

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
        // 게임 클리어
    }
}
