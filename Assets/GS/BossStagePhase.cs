using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class BossStagePhase : MonoBehaviour
{
    [Header("Wave Date"), ReadOnly]
    [SerializeField]
    private PhaseInfo[] m_spawnInfoList;

    private BossStageMachine[] m_machineList;

    private int m_currentPhase;

    private int m_lastWave = 3;

    private bool m_active;

    private Coroutine m_spawnCoroutine;

    private Actor m_character;

    private void Start()
    {
        m_machineList = GetComponentsInChildren<BossStageMachine>();
        foreach (var machine in m_machineList)
        {
            machine.Interacted += NextPhase;
        }

        m_character = GameManager.Character.Controller.Character;
    }

    public void StartPhase()
    {
        if (m_active)  return;

        m_active = true;

        // Wave 시작시 스폰 시작
        m_spawnCoroutine = StartCoroutine(IE_Spawn()); 
    }

    private void NextPhase(object sender, EventArgs e)
    {
        if (!m_active) return;

        // 기존 Spawner 비활성화
        m_spawnInfoList[m_currentPhase++].Spawner.EndSpawn();

        // 마지막 Wave까지 클리어 => 엔딩
        if (m_currentPhase >= m_lastWave)
        {
            m_active = false;
            EndPhase();
            return;
        }

        // 보스 데미지 이벤트 
        // temp
        Debug.Log("보스 아프다!!");

        // 새로운 Spawner 활성화
        if (m_spawnCoroutine != null)
            StopCoroutine(IE_Spawn());
        m_spawnCoroutine = StartCoroutine(IE_Spawn());
    }

    private void EndPhase()
    {
        // temp
        Debug.Log("보스 죽었다!!");
    }

    private IEnumerator IE_Spawn()
    { 
        yield return new WaitForSeconds(m_spawnInfoList[m_currentPhase].SpawnDelay);

        var charater = m_character as Monster;
        if (charater == null)
            yield break;

        while (true)
        {
            if (charater.Movement.IsOnGround == true)
                break;

            yield return null;
        }

        m_spawnInfoList[m_currentPhase].Spawner.StartSpawn();
    }

    [Serializable]
    private class PhaseInfo
    {
        [SerializeField]
        private BossStageSpawner m_spawner;

        [SerializeField]
        private float m_spawnDelay;

        public BossStageSpawner Spawner => m_spawner;

        public float SpawnDelay => m_spawnDelay;
    }
}
