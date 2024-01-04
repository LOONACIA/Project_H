using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    public class CanGo : Conditional
    {
        [SerializeField]
        private SharedTransform m_target;

        [SerializeField]
        private SharedTransform m_self;

        private NavMeshPath m_path = new();


        // Start is called before the first frame update
        public override TaskStatus OnUpdate()
        {
            //return bool1.Value == bool2.Value ? TaskStatus.Success : TaskStatus.Failure;
            NavMesh.CalculatePath(m_self.Value.position, m_target.Value.position, -1, m_path);

            return m_path.status == NavMeshPathStatus.PathComplete ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {

        }
    }
}

