using LOONACIA.Unity.Coroutines;
using System;
using LOONACIA.Unity.Managers;
using System.Collections;
using UnityEngine;

public class PossessionProcessor : MonoBehaviour
{
    private static readonly int s_possess = Animator.StringToHash("Possess");

    [SerializeField]
    private float m_shurikenSphereRadius = 0.5f;

    [SerializeField]
    private GameObject m_ghostPrefab;

    [SerializeField]
    [Tooltip("표창이 막히는 레이어")]
    private LayerMask m_targetLayers;

    private Actor m_sender;

    private Actor m_target;

    // 빙의가 가능한지 여부 체크, 표창을 던질 지, 빙의를 할지를 판단함.
    private bool m_isPossessable;

    private bool m_isOnPossessing;
    
    private float m_currentCoolTime = ConstVariables.SHURIKEN_COOLTIME;

    private PossessionShuriken m_shuriken;

    private CoroutineEx m_possessionCoroutine;

    public float CoolTime => Mathf.Min(m_currentCoolTime / ConstVariables.SHURIKEN_COOLTIME, 1f);
    
    /// <summary>
    /// 표창을 던질 때 발생하는 이벤트.
    /// </summary>
    public event EventHandler ShurikenThrown;

    /// <summary>
    /// 빙의 타겟 선정에 성공할 경우, 빙의 시작 시 발생하는 이벤트.
    /// </summary>
    public event EventHandler Possessing;

    /// <summary>
    /// 빙의 시 발생하는 이벤트. 성공하면 대상 몬스터가 전달되고, 실패하면 null이 전달됨.
    /// </summary>
    public event EventHandler<Actor> Possessed;

    /// <summary>
    /// 빙의 타겟에게 표창이 적중할 경우 발생하는 이벤트.
    /// </summary>
    public event EventHandler<float> TargetHit;

    /// <summary>
    /// 해킹을 시작할 때 발생하는 이벤트. 해킹에 필요한 시간이 전달됨.
    /// </summary>
    public event EventHandler<float> HackStarted;

    /// <summary>
    /// 해킹이 완료되어 빙의가 가능할 때 발생하는 이벤트.
    /// </summary>
    public event EventHandler Possessable;

    /// <summary>
    /// 수리검 쿨타임이 돌고 있을 때 발생하는 이벤트.
    /// </summary>
    public event EventHandler CoolTimeChanged;
    
    private void Update()
    {
        // TODO: 로직 수정(불필요한 호출 있음)
        if (m_currentCoolTime <= ConstVariables.SHURIKEN_COOLTIME)
        {
            m_currentCoolTime += Time.deltaTime;
            CoolTimeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Hack(Actor sender)
    {
        // Cooldown Check
        if (m_currentCoolTime < ConstVariables.SHURIKEN_COOLTIME)
        {
            return;
        }
        
        m_sender = sender;
        IAnimationEventReceiver receiver = m_sender.GetComponentInChildren<IAnimationEventReceiver>();
        receiver?.SetPossession(this);

        m_sender.Animator.SetTrigger(s_possess);
    }

    public void TryPossess(Actor sender)
    {
        //표창이 박혀있는데 빙의가 아직 불가능하면 return
        if (!m_isPossessable)
        {
            return;
        }

        m_sender = sender;
        m_isPossessable = false;

        PossessTarget();

        //Shuriken이 시간이 지나면 사라지기도 하므로, 삭제가 되었을 때는 Release하지 않음
        if (m_shuriken != null)
        {
            ManagerRoot.Resource.Release(m_shuriken.gameObject);
        }
    }

    //표창이 박힌 타겟에게 빙의
    public void PossessTarget()
    {
        var position = m_sender.FirstPersonCameraPivot.transform.position;
        var ghostObj = ManagerRoot.Resource.Instantiate(m_ghostPrefab, position, m_sender.transform.rotation);
        if (!ghostObj.TryGetComponent<Ghost>(out var ghost))
        {
            throw new InvalidOperationException($"{ghostObj.name} does not have {nameof(Ghost)} component.");
        }

        m_isOnPossessing = true;
        Possessing?.Invoke(this, EventArgs.Empty);

        // 사운드
        GameManager.Sound.Play(GameManager.Sound.ObjectDataSounds.TryHackingSound);

        ghost.PossessToTarget(m_target, () => OnPossessed(m_target));
    }

    public void ClearTarget()
    {
        m_possessionCoroutine?.Abort();
        m_isPossessable = false;
        m_target = null;
        //m_shuriken = null;
        OnPossessed(null);
    }

    public void OnPossessAnimStart()
    {
        ThrowShuriken();
    }

    public void OnPossessAnimEnd()
    {
    }
    
    private void ThrowShuriken()
    {
        // 이미 빙의 중인 상태라면
        if (m_isOnPossessing)
        {
            // 리턴
            return;
        }
        
        //ClearTarget();
        m_currentCoolTime = 0f;
        var cameraPivot = GameManager.Camera.CurrentCamera;
        var cameraTransform = cameraPivot.transform;

        bool isHit = Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out var hit, 300f);
        //bool isHit = Physics.SphereCast(cameraTransform.position, m_shurikenSphereRadius, cameraTransform.forward,
        //    out var hit, 300f, m_targetLayers);

        Vector2 view = new(cameraPivot.transform.forward.x, cameraPivot.transform.forward.y);
        float objectAngle = Vector2.SignedAngle(Vector2.right, view);  

        var shuriken =
            Instantiate(m_sender.Data.ShurikenObj, cameraTransform.position + Vector3.down * 1 / 16f,
                Quaternion.Euler(objectAngle, 0, 0)).GetComponent<PossessionShuriken>();

        if (isHit && hit.transform.TryGetComponent<Actor>(out var actor) && actor.Status.Shield != null && actor.IsPossessed == false)
        {
            shuriken.InitSetting(actor, m_sender, OnTargetHit);
        }
        else
        {
            shuriken.InitSetting(cameraPivot.transform.forward, m_sender, OnTargetHit);
        }
        
        ShurikenThrown?.Invoke(this, EventArgs.Empty);

        #region Legacy

        // if (isHit && (hit.transform.gameObject.layer & m_obstacleLayers) != 0)
        // {
        //     m_shuriken.InitSetting(cameraPivot.transform.forward, m_sender, OnTargetHit);
        // }
        // else if (isHit && (hit.transform.gameObject.layer & m_targetLayers) != 0)
        // {
        //     // 몬스터가 쉴드를 가지고 있으면 빙의 불가
        //     if (hit.transform.GetComponent<ActorStatus>()?.Shield != null)
        //         m_shuriken.InitSetting(cameraPivot.transform.forward, m_sender, OnTargetHit);
        //     else
        //         m_shuriken.InitSetting(hit.transform.GetComponent<Actor>(), m_sender, OnTargetHit);
        // }
        // else
        // {
        //     m_shuriken.InitSetting(cameraPivot.transform.forward, m_sender, OnTargetHit);
        // }

        #endregion
    }

    private void TryHacking(Actor target)
    {
        target.Health.TakeDamage(new(m_sender.gameObject, target.Health, 0, Vector3.zero, Vector3.zero));

        // 해킹이 불가능한 Actor라면
        if (!target.Data.CanHack)
        {
            // return
            return;
        }

        target.PlayHackAnimation();
        
        if (target == m_target)
        {
            return;
        }
        
        ClearTarget();
        m_target = target;
        HackStarted?.Invoke(this, target.Data.PossessionRequiredTime);
        m_possessionCoroutine = CoroutineEx.Create(this, CoWaitForPossession(target.Data.PossessionRequiredTime));
    }

    private IEnumerator CoWaitForPossession(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        
        m_isPossessable = true;
        Possessable?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnPossessed(Actor actor)
    {
        m_isOnPossessing = false;
        Possessed?.Invoke(this, actor);
    }
    
    private void OnTargetHit(PossessionShuriken shuriken, Actor target)
    {
        if (target == null)
        {
            return;
        }
        
        // if (shuriken != m_shuriken)
        // {
        //     return;
        // }
        
        target.Dying -= OnTargetDying;
        target.Dying += OnTargetDying;
        TargetHit?.Invoke(this, target.Data.PossessionRequiredTime);
        TryHacking(target);
    }

    private void OnTargetDying(object sender, in AttackInfo info)
    {
        if (sender is not Actor target)
        {
            return;
        }
        
        target.Dying -= OnTargetDying;

        if (target == m_target)
        {
            ClearTarget();
        }
    }
}
