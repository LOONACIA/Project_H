using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    public class IsNullOrMissing : Conditional
    {
        [SerializeField]
        private SharedGameObject m_target;


        // Start is called before the first frame update
        public override TaskStatus OnUpdate()
        {
            //return bool1.Value == bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
            return (m_target.Value == null||!m_target.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {

        }
    }
}

