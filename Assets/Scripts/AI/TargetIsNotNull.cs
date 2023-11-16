using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    public class TargetIsNotNull : Conditional
    {
        [SerializeField]
        private SharedTransform m_target;


        // Start is called before the first frame update
        public override TaskStatus OnUpdate()
        {
            //return bool1.Value == bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
            return m_target.Value != null ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {

        }
    }
}

