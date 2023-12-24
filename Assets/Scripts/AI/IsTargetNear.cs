using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;
using UnityEngine;

public class IsTargetNear : Action
{
    //public SharedFloat radius;
    private float radius;
    public SharedLayerMask Recipient;
    
    private float detectionTime;
    private bool m_isTargetInside;
    private bool m_isTargetDetected;
    // Start is called before the first frame update
    public override void OnAwake()
    {
        detectionTime = 0;
        m_isTargetInside = false;
        m_isTargetDetected = false;
        radius = 3f;
        base.OnAwake();
    }

    public override TaskStatus OnUpdate()
    {
        CheckForTarget();
        if (!m_isTargetInside)
        {
            detectionTime -= Time.deltaTime;
            if (detectionTime < 0 )
            {
                detectionTime = 0;
            }
        }
        if (m_isTargetInside)
        {
            detectionTime += Time.deltaTime;
            if (detectionTime >= 3f)
            {
                m_isTargetDetected = true;
            }
            return TaskStatus.Running;
        }
        if (detectionTime <= 0f)
        {
            m_isTargetDetected = false;
        }
        return TaskStatus.Failure;
    }

    private void CheckForTarget()
    {
        var monstersInRadius = Physics.OverlapSphere(transform.position, radius, Recipient.Value)
           .Select(col => col.GetComponent<Monster>());

        if (monstersInRadius.Any(actor => actor.IsPossessed))
        {
            m_isTargetInside = true;
            if (m_isTargetDetected)
            {
                Debug.Log(m_isTargetDetected);
            }
        }
        else
        {
            m_isTargetInside = false;
        }

    }
}
