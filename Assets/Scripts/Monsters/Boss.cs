using LOONACIA.Unity.Coroutines;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Boss : MonoBehaviour, IHealth
{
    private Animator m_animator;

    private RigBuilder m_rigBuilder;

    private MultiAimConstraint m_aimConstraint;

    private Transform m_targetTransform;

    private int m_currentHP = 1;

    private int m_maxHp = 1;

    private float m_rotateSpeed = .5f;

    private CoroutineEx m_waitLookCoroutine;

    private CoroutineEx m_lerpCoroutine;

    public int CurrentHp => m_currentHP;

    public int MaxHp => m_maxHp;

    public event RefEventHandler<AttackInfo> Damaged { add { } remove { } }
    public event RefEventHandler<AttackInfo> Dying;
    public event EventHandler Died { add { } remove { } }

    private void Start()
    {
        m_rigBuilder = GetComponentInChildren<RigBuilder>();
        m_aimConstraint = GetComponentInChildren<MultiAimConstraint>();
        TryGetComponent<Animator>(out m_animator);

        // 플레이어가 바뀔 때마다 바라보는 타겟을 변경함
        GameManager.Character.Controller.CharacterChanged += OnCharacterChanged;

        // 보스가 점프를 끝날 때까지는 플레이어를 바라보지 않음
        m_aimConstraint.weight = 0;
        m_waitLookCoroutine = CoroutineEx.Create(this, Co_WaitLook());
        SetTarget(GameManager.Character.Controller.Character.transform);
    }

    private void Update()
    {
        if (m_animator.GetBool(ConstVariables.ANIMATOR_PARAMETER_LOOK))
        {
            Rotate();
        }
    }

    public void Kill()
    {
        Dying?.Invoke(this, (new(null, this, MaxHp, Vector3.zero, Vector3.zero)));
    }

    public void TakeDamage(in AttackInfo damageInfo)
    {
        m_animator.SetTrigger(ConstVariables.ANIMATOR_PARAMETER_HIT);

        m_waitLookCoroutine?.Abort();
        m_waitLookCoroutine = CoroutineEx.Create(this, Co_WaitLook());
    }

    private void OnCharacterChanged(object sender, Actor e)
    {
        SetTarget(e.transform);
    }

    private void SetTarget(Transform transform)
    {
        m_targetTransform = transform;
        
        var data = m_aimConstraint.data.sourceObjects;
        data.Clear();
        data.Add(new WeightedTransform(transform, 1));
        m_aimConstraint.data.sourceObjects = data;
        m_rigBuilder.Build();
    }

    private void Rotate()
    {
        // 타겟 방향 구하기
        var targetDirection = m_targetTransform.position.GetFlatVector() - transform.position.GetFlatVector();
        targetDirection.Normalize();

        // lerp 값 구하기
        targetDirection = Vector3.Lerp(transform.forward, targetDirection, Time.deltaTime * m_rotateSpeed);

        // 회전하기
        transform.forward = targetDirection;
    }

    private IEnumerator Co_WaitLook()
    {
        m_animator.SetBool(ConstVariables.ANIMATOR_PARAMETER_LOOK, false);
        m_lerpCoroutine?.Abort();
        m_lerpCoroutine = CoroutineEx.Create(this, Co_LerpLook(.1f, 1, 0));

        yield return new WaitUntil(() => m_animator.GetBool(ConstVariables.ANIMATOR_PARAMETER_LOOK) == true);
        m_lerpCoroutine?.Abort();
        m_lerpCoroutine = CoroutineEx.Create(this, Co_LerpLook(.5f, 0, 1));
    }

    private IEnumerator Co_LerpLook(float progressTime, float from, float to)
    {
        float time = 0;
        while (time < progressTime)
        { 
            time += Time.deltaTime;
            m_aimConstraint.weight = Mathf.Lerp(from, to, time/progressTime);
            yield return null;
        }
    }
}
