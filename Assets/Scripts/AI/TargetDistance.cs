using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    public class TargetDistance : Conditional
    {
        [SerializeField]
        private SharedTransform m_target;
        public SharedTransform m_self;
        private Transform m_position;
        public SharedFloat m_distance;

        private float m_targetDistance;

        // Start is called before the first frame update
        public override TaskStatus OnUpdate()
        {
            m_targetDistance = Vector3.Distance(m_target.Value.position, m_self.Value.position);
            //Debug.Log(m_targetDistance);
            //return bool1.Value == bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
            return m_targetDistance <= m_distance.Value ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            
        }
    }
}

