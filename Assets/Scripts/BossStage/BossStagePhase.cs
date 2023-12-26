using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static InteractableObject;

public class BossStagePhase : MonoBehaviour
{
    [Header("Spawn Date")]
    [SerializeField, Tooltip("몬스터 스폰 지연 시간")]
    private float m_spawnDelay;

    [Header("Objects Date")]
    [SerializeField, Tooltip("Phase 범위 트리거")]
    private BossStagePhaseTrigger m_phaseTrigger;

    [SerializeField, Tooltip("Phase 끝나면 끌 ground 객체")]
    private GameObject m_onEndDeactiveGround;

    [SerializeField, Tooltip("Phase 시작하면 켤 객체")]
    private GameObject[] m_onStartActiveObjects;

    [SerializeField, Tooltip("Phase 끝나면 끌 객체")]
    private GameObject[] m_onEndDeactiveObjects;

    [Header("Explosion Data")]
    [SerializeField, Tooltip("폭발의 지연 시간")]
    private float m_explosionDelay;

    [SerializeField, Tooltip("폭발 크기")]
    private float m_explosionForce;

    [SerializeField, Tooltip("폭발 반지름")]
    private float m_explosionRadius;

    [SerializeField]
    private ParticleSystem m_explosionParticle;

    [SerializeField]
    private Animator m_explosionAnimator;

    [Tooltip("Phase 종료 이벤트를 발생시킬 오브젝트 리스트")]
    private BossStageMachine[] m_machineList;

    [Tooltip("Phase 종료 시 날려버릴 오브젝트 리스트")]
    private Explosive[] m_explosiveList;

    private ISpawn[] m_spawnerList;

    private Actor m_character;

    private bool m_active;

    private PossessionProcessor m_processor;

    private Coroutine m_readyPhaseCoroutine;

    private Coroutine m_spawnCoroutine;

    public event EventHandler PhaseEnd;

    private void Start()
    {
        m_machineList = GetComponentsInChildren<BossStageMachine>(true);
        foreach (var machine in m_machineList)
        {
            machine.Interacted += OnInteracted;
        }

        m_phaseTrigger = GetComponentInChildren<BossStagePhaseTrigger>();

        m_explosiveList = GetComponentsInChildren<Explosive>(true);

        m_spawnerList = GetComponentsInChildren<ISpawn>();

        m_character = GameManager.Character.Controller.Character;

        m_processor = FindObjectOfType<PossessionProcessor>();

        // 객체 비활성화
        foreach (var activeObject in m_onStartActiveObjects)
        {
            activeObject.SetActive(false);
        }
    }

    public void ReadyPhase()
    {
        if (m_active) return;

        m_active = true;

        // 플레이어가 특정 위치에 존재할 때 Phase 진행
        m_readyPhaseCoroutine = StartCoroutine(IE_WaitForStageReady());

        // 스테이지 시작하면 groundTrigger 활성화
        // 플레이어가 Trigger에 검출되면 바닥 생성
        m_phaseTrigger?.Activate();
    }

    private void StartPhase()
    {
        // 몬스터 스폰
        m_spawnCoroutine = StartCoroutine(IE_WaitSpawnDelay());

        // 객체 활성화
        foreach (var activeObject in m_onStartActiveObjects)
        {
            activeObject.SetActive(true);
        }
    }

    private void EndPhase(Transform interactedTransform)
    {
        if (!m_active) return;

        m_active = false;

        if (m_readyPhaseCoroutine != null)
            StopCoroutine(m_readyPhaseCoroutine);

        if (m_spawnCoroutine != null)
            StopCoroutine(m_spawnCoroutine);

        foreach (var spawner in m_spawnerList)
        {
            spawner.EndSpawn();
        }

        // 객체 비활성화
        foreach (var deactiveObject in m_onEndDeactiveObjects)
        {
            deactiveObject.SetActive(false);
        }

        StartCoroutine(IE_WaitEndEvent(interactedTransform));
    }

    private void OnInteracted(object sender, Transform e)
    {
        EndPhase(e);
    }

    /// <summary>
    /// Phase 끝나면 발생하는 폭발 효과
    /// </summary>
    private void Explode(Transform interactedTransform)
    {
        if (m_onEndDeactiveGround != null)
        {
            if (m_onEndDeactiveGround.TryGetComponent<Renderer>(out var renderer))
                renderer.enabled = false;
            if (m_onEndDeactiveGround.TryGetComponent<Collider>(out var collider))
                collider.enabled = false;
        }
        
        foreach (var explosive in m_explosiveList)
        {
            explosive.gameObject.SetActive(true);
            explosive.Explode(m_explosionForce, interactedTransform.transform.position, m_explosionRadius);
        }
    }

    private void ExcuteExplodeEffect(Transform interactedTransform)
    {
        m_explosionParticle.transform.position = interactedTransform.transform.position;

        m_explosionParticle?.Play();
        if (m_explosionAnimator != null)
            m_explosionAnimator.enabled = true;
    }

    /// <summary>
    /// spawnDelay 이후에 플레이어가 PhaseTrigger 안에 존재하면 몬스터를 생성하기 시작
    /// </summary>
    /// <returns></returns>
    private IEnumerator IE_WaitForStageReady()
    {
        var charater = GameManager.Character.Controller.Character as Monster;
        if (charater == null)
            yield break;

        yield return new WaitUntil(() => m_phaseTrigger?.IsInArea(charater.transform.position) == true);

        StartPhase();
    }

    private IEnumerator IE_WaitSpawnDelay()
    { 
        yield return new WaitForSeconds(m_spawnDelay);

        var charater = GameManager.Character.Controller.Character as Monster;
        if (charater == null)
            yield break;

        yield return new WaitUntil(() => charater.Movement.IsOnGround == true);

        foreach (var spawner in m_spawnerList)
        { 
            spawner.StartSpawn();
        }
    }

    private IEnumerator IE_WaitEndEvent(Transform interactedTransform)
    {
        float explosionInterval = 1f;

        // 폭발 이펙트 실행
        yield return new WaitForSeconds(m_explosionDelay - explosionInterval);
        ExcuteExplodeEffect(interactedTransform);

        // 폭발 실행
        yield return new WaitForSeconds(explosionInterval);
        Explode(interactedTransform);

        // 몬스터 사망 처리
        m_processor.ClearTarget();
        var monsterList = GameObject.FindObjectsOfType<Monster>().Where(x => x.IsPossessed == false);
        foreach (var monster in monsterList)
        {
            monster.Health.Kill();
        }

        PhaseEnd?.Invoke(this, EventArgs.Empty);
    }
}
