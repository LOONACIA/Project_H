using System;
using UnityEngine;

public class BossStageRoot : MonoBehaviour
{
    [SerializeField]
    private Boss m_boss;

    [SerializeField]
    private BossStagePhase[] m_bossStagePhaseList;

    [Header("Phase Info")]
    [SerializeField]
    private BossPhaseType m_currentPhase = BossPhaseType.None;

    private SaveMachine[] m_saveMachineList;

    [Header("Start Event")]
    [SerializeField, Tooltip("플레이어를 날려버릴 위치")]
    private Transform m_throwPosition;

    [SerializeField, Tooltip("플레이어를 날려버릴떄 이동, 대쉬 불가능하게 만드는 시간")]
    private float m_throwKnockDownTime;

    [SerializeField]
    private Explosive m_dummyMachine;

    [SerializeField, Tooltip("보스전 시작하면 켤 객체")]
    private GameObject[] m_onStartActiveObjects;

    [SerializeField, Tooltip("보스전 시작하면 끌 객체")]
    private GameObject[] m_onStartDeactiveObjects;

    [Header("End Event")]
    [SerializeField, Tooltip("플레이어가 마지막 상호작용한 후, 게임이 끝날 때까지의 지연 시간")]
    private float m_clearDelayTime;

    private Actor m_character;

    private int m_lastPhase;

    public BossPhaseType CurrentPhase 
    {
        get { return m_currentPhase; }
        set { m_currentPhase = value; }
    }

    private void Start()
    {
        m_currentPhase = GameManager.Character.CurrentBossPhase;
        m_character = GameManager.Character.Controller.Character;

        foreach (var phase in m_bossStagePhaseList)
        {
            phase.PhaseEnd += OnPhaseEnd;
        }

        // 특정 페이즈부터 실행
        if (m_currentPhase != BossPhaseType.None)
        {
            StageStart((int)m_currentPhase);
        }

        // 보스 페이즈 저장
        m_saveMachineList = GetComponentsInChildren<SaveMachine>(true);
        foreach (var saveMachine in m_saveMachineList)
        { 
            saveMachine.Interacted += SaveBossPhase;
        }
    }


    public void StageStart(int phase = ((int)BossPhaseType.None))
    {
        m_lastPhase = m_bossStagePhaseList.Length - 1;

        // 보스 설정
        m_boss.gameObject.SetActive(true);
        Vector3 bossLookVec = (m_character.transform.position - m_boss.transform.position).GetFlatVector();
        m_boss.transform.forward = bossLookVec;
        if (m_currentPhase == BossPhaseType.None && m_boss.gameObject.TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger(ConstVariables.ANIMATOR_PARAMETER_WAKE_UP);
        }

        // 보스 스테이지 시작시 활성화하는 객체
        foreach (var activeObject in m_onStartActiveObjects)
        {
            activeObject.SetActive(true);
        }

        if (phase == ((int)BossPhaseType.None))
        {
            // Stage 시작하면 플레이어를 지정한 방향으로 날림
            ThrowPlayer();
            m_dummyMachine?.Explode(20, transform.forward * 5f, 100, 10);

            m_bossStagePhaseList[(int)(++m_currentPhase)]?.ReadyPhase();
        }
        else
        {
            // 보스 스테이지 시작시 비활성화하는 객체
            foreach (var deactiveObject in m_onStartDeactiveObjects)
            {
                deactiveObject.SetActive(false);
            }

            for (int i = 0; i < (int)m_currentPhase; i++) 
            {
                m_bossStagePhaseList[i].gameObject.SetActive(false);
            }

            m_bossStagePhaseList[phase].StartPhase();
        }
    }

    private void OnPhaseEnd(object sender, EventArgs e)
    {
        m_boss.TakeDamage(new AttackInfo());
        
        NextPhase();
    }

    private void NextPhase()
    {
        if ((int)m_currentPhase >= m_lastPhase)
        {
            m_boss.Kill();
            return;
        }

        m_bossStagePhaseList[(int)(++m_currentPhase)].ReadyPhase();
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

    private void SaveBossPhase(object sender, Transform e)
    {
        GameManager.Character.CurrentBossPhase = (BossPhaseType)CurrentPhase;
    }
}
