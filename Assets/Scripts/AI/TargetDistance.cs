using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    public class TargetDistance : Conditional
    {
        [SerializeField]
        private SharedTransform m_target;
        private Transform m_position;
        public float m_distance;

        private float m_targetDistance;

        // Start is called before the first frame update
        public override TaskStatus OnUpdate()
        {
            //return bool1.Value == bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
            return m_targetDistance <= m_distance ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            m_targetDistance = Vector3.Distance(m_target.Value.position, this.transform.position);
        }
    }
}

