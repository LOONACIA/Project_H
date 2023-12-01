using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightZone : MonoBehaviour
{
    public event EventHandler OnClear;

    [SerializeField, Range(0, 30)]
    private float m_delay;    
    
    [SerializeField, Range(0, 1)]
    private float m_checkRate = 0.5f;

    private List<Actor> m_actors = new();

    private BoxCollider m_collider;

    #region Public Method

    public void BeginFight()
    {
        GetActorsInBox();
        InvokeRepeating(nameof(Check), m_delay, m_checkRate);
    }

    #endregion

    #region Private Method

    private void Start()
    {
        m_collider = GetComponent<BoxCollider>();
    }

    private void GetActorsInBox()
    {
        var actorColliders = Physics.OverlapBox(transform.position + m_collider.center,
                                        transform.localScale / 2,
                                        Quaternion.identity,
                                        LayerMask.GetMask("Monster"));

        foreach (var actorCollider in actorColliders)
        {
            if (actorCollider.gameObject.TryGetComponent<Actor>(out var actor))
                m_actors.Add(actor);
        }
    }

    private void Check()
    {
        foreach (var actor in m_actors.ToList())
        {
            if (actor == null)
            {
                m_actors.Remove(actor);
                continue;
            }

            if (!actor.IsPossessed)
                return;
        }

        FinishFight();
    }

    private void FinishFight()
    {
        OnClear.Invoke(this, EventArgs.Empty);
        CancelInvoke(nameof(Check));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
    
    #endregion
}
