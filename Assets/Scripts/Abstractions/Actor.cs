using BehaviorDesigner.Runtime;
using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ActorHealth))]
[RequireComponent(typeof(ActorStatus))]
public abstract class Actor : MonoBehaviour
{
    private static readonly int s_hitAnimationKey = Animator.StringToHash("Hit");
    
    protected NavMeshAgent m_agent;

    protected Rigidbody m_rigidbody;

    protected Collider m_collider;
    
    [SerializeField]
    private ActorData m_data;

    protected BehaviorTree m_behaviorTree;

    [SerializeField]
    private GameObject m_firstPersonCameraPivot;
    
    [SerializeField]
    private Animator m_firstPersonAnimator;
    
    [SerializeField]
    private Animator m_thirdPersonAnimator;
    
    public ActorData Data => m_data;
    
    public ActorHealth Health { get; private set; }
    
    public ActorStatus Status { get; private set; }
    
    public bool IsPossessed { get; private set; }

    public Animator Animator => IsPossessed ? m_firstPersonAnimator : m_thirdPersonAnimator;
    
    public Animator FirstPersonAnimator => m_firstPersonAnimator;
    
    public Animator ThirdPersonAnimator => m_thirdPersonAnimator;

    public GameObject FirstPersonCameraPivot => m_firstPersonCameraPivot;

    public event EventHandler Dying;

    protected virtual void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_collider = GetComponent<Collider>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_behaviorTree = GetComponent<BehaviorTree>();
        Health = GetComponent<ActorHealth>();
        Status = GetComponent<ActorStatus>();
        EnableAIComponents();
    }
    
    protected virtual void OnEnable()
    {
        if (TryGetComponent<IHealth>(out var health))
        {
            health.Dying += OnDying;
            health.Died += OnDied;   
        }

        if (m_collider != null)
        {
            m_collider.enabled = true;
        }
    }

    protected virtual void OnDisable()
    {
        if (TryGetComponent<IHealth>(out var health))
        {
            health.Dying -= OnDying;
            health.Died -= OnDied;   
        }
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    public abstract void Move(Vector3 direction);

    public abstract void TryJump();

    public abstract void TryAttack();

    public abstract void Skill();

    public abstract void Dash();

    public abstract void Block(bool value);

    public virtual void Possessed()
    {
        Animator.gameObject.SetActive(false);
        IsPossessed = true;
        Animator.gameObject.SetActive(true);
        DisableAIComponents();
    }
    
    public virtual void Unpossessed()
    {
        Animator.gameObject.SetActive(false);
        IsPossessed = false;
        Animator.gameObject.SetActive(true);
        EnableAIComponents();
    }

    protected void EnableAIComponents()
    {
        if (m_agent != null)
        {
            m_agent.enabled = true;
        }

        if (m_rigidbody != null)
        {
            m_rigidbody.isKinematic = true;
        }

        if (m_behaviorTree != null)
        {
            m_behaviorTree.enabled = true;
        }

        if (Animator != null)
        {
            m_firstPersonAnimator.gameObject.SetActive(false);
        }
    }
    
    protected void DisableAIComponents()
    {
        if (m_behaviorTree != null)
        {
            m_behaviorTree.enabled = false;
        }

        if (m_agent != null)
        {
            m_agent.enabled = false;
        }

        if (m_rigidbody != null)
        {
            m_rigidbody.isKinematic = false;
        }
    }

    /// <summary>
    /// Actor가 죽을 때 호출됩니다.
    /// </summary>
    protected virtual void OnDying()
    {
        // Actor 제거 시 별도 처리가 필요한 경우 이곳에 작성합니다.
        Dying?.Invoke(this, EventArgs.Empty);
        
        if (m_collider != null)
        {
            m_collider.enabled = false;
        }
    }

    protected virtual void OnDied()
    {
        StartCoroutine(CoDie(ConstVariables.MONSTER_DESTROY_WAIT_TIME));
    }
    
    private void OnDying(object sender, EventArgs e)
    {
        OnDying();
    }
    
    private void OnDied(object sender, EventArgs e)
    {
        OnDied();
    }

    private IEnumerator CoDie(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ManagerRoot.Resource.Release(gameObject);
    }
    
    private void OnValidate()
    {
        if (m_data == null && GetType() != typeof(Ghost))
        {
            Debug.LogWarning($"{name}: {nameof(m_data)} is null");
        }
    }
}