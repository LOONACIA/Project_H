using System;
using System.Collections;
using UnityEngine;

public class BossStagePhase : MonoBehaviour
{
    [Header("Spawn Date")]
    [SerializeField]
    private BossStageSpawner m_spawner;

    [SerializeField]
    private float m_spawnDelay;

    [Header("Objects Date")]
    [SerializeField, Tooltip("Phase 범위 트리거")]
    private BossStagePhaseTrigger m_phaseTrigger;

    [SerializeField, Tooltip("Phase 끝나면 끌 ground 객체")]
    private GameObject m_ground;

    private BossStageMachine[] m_machineList;

    private BossStageFallable[] m_fallableList;

    private JumpPad[] m_jumpPadList;

    private Actor m_character;

    private bool m_active;

    private Coroutine m_spawnCoroutine;

    public event EventHandler PhaseEnd;

    private void Start()
    {
        m_machineList = GetComponentsInChildren<BossStageMachine>();
        foreach (var machine in m_machineList)
        {
            machine.Interacted += EndPhase;
        }

        m_jumpPadList = GetComponentsInChildren<JumpPad>();

        m_phaseTrigger = GetComponentInChildren<BossStagePhaseTrigger>();

        m_fallableList = GetComponentsInChildren<BossStageFallable>(true);

        m_character = GameManager.Character.Controller.Character;
    }

    public void StartPhase()
    {
        if (m_active) return;

        m_active = true;

        // 플레이어가 땅 위에 존재할 때부터 몬스터 생성
        m_spawnCoroutine = StartCoroutine(IE_ReadySpawn());

        // 스테이지 시작하면 groundTrigger 활성화
        // 플레이어가 Trigger에 검출되면 바닥 생성
        m_phaseTrigger?.Activate();
    }

    private void EndPhase(object sender, EventArgs e)
    {
        if (!m_active) return;

        m_active = false;

        if (m_spawnCoroutine != null)
            StopCoroutine(IE_ReadySpawn());

        // 페이즈 끝나면 윗층으로 점프 가능
        foreach (var jumpPad in m_jumpPadList)
        { 
            jumpPad.Activate();
        }

        // 페이즈 끝나면 기존 땅 끄고 무너지는 효과
        if (m_ground != null)
        { 
            if (m_ground.TryGetComponent<Renderer>(out var renderer))
                renderer.enabled = false;

            if (m_ground.TryGetComponent<Collider>(out var collider))
                collider.enabled = false;

            var children = m_ground.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            { 
                child.gameObject.SetActive(true);
            }
        }
        foreach (var fallDownGround in m_fallableList)
        { 
            fallDownGround.Activate();
        }

        // 보스 데미지 이벤트 
        // temp
        Debug.Log("보스 아프다!!");

        PhaseEnd?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// spawnDelay 이후에 플레이어가 PhaseTrigger 안에 존재하면 몬스터를 생성하기 시작
    /// </summary>
    /// <returns></returns>
    private IEnumerator IE_ReadySpawn()
    {
        yield return new WaitForSeconds(m_spawnDelay);

        var charater = m_character as Monster;
        if (charater == null)
            yield break;

        while (true)
        {
            if (m_phaseTrigger.IsInArea(charater.transform.position) == true)
                break;

            yield return null;
        }

        m_spawner.StartSpawn();
    }
}
