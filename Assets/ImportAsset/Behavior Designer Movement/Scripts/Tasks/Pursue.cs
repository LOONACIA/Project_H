using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Pursue the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PursueIcon.png")]
    public class Pursue : NavMeshMovement
    {
        [Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
        public SharedFloat targetDistPrediction = 20;
        [Tooltip("Multiplier for predicting the look ahead distance")]
        public SharedFloat targetDistPredictionMult = 20;
        [Tooltip("The GameObject that the agent is pursuing")]
        public SharedTransform target;

        [Tooltip("공격 사거리, 해당 값까지 가서 멈춥니다. 값이 0.5 미만이거나 null이라면 arriveDistance가 적용됩니다.")]
        public SharedFloat attackDistance;

        private MonsterMovement m_monsterMovement;
        private Vector3 m_lastTarget;

        // The position of the target at the last frame
        private Vector3 targetPosition;
        public override void OnAwake()
        {
            base.OnAwake();
            m_monsterMovement = GetComponent<MonsterMovement>();
        }
        public override void OnStart()
        {
            base.OnStart();

            targetPosition = target.Value.transform.position;
            m_lastTarget = Target();
            m_monsterMovement.MoveTo(m_lastTarget);
        }

        // Pursue the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            //TargetDistance 로직이 포지션보다 (Vector3.up/2) 높은 위치에서 판별하므로, 해당 값을 갱신해줌
            if (attackDistance != null||attackDistance.Value<0.3f)
            {
                arriveDistance.Value = attackDistance.Value - 0.3f;
            }

            if (HasArrived()) {
                m_monsterMovement.StopAgentMove();
                return TaskStatus.Success;
            }

            // Target will return the predicated position
            m_lastTarget = Target();
            if (m_lastTarget == Vector3.zero)
            {
                m_monsterMovement.StopAgentMove();
                return TaskStatus.Failure;
            }
            m_monsterMovement.MoveTo(m_lastTarget);

            return TaskStatus.Running;
        }

        public override void OnConditionalAbort()
        {
            m_monsterMovement.StopAgentMove();
        }

        // Predict the position of the target
        private Vector3 Target()
        {
            if (target.Value == null)
            {
                return Vector3.zero;
            }

            // Calculate the current distance to the target and the current speed
            var distance = (target.Value.transform.position - transform.position).magnitude;
            var speed = Velocity().magnitude;

            float futurePrediction = 0;
            // Set the future prediction to max prediction if the speed is too small to give an accurate prediction
            //if (speed <= distance / targetDistPrediction.Value) {
            futurePrediction = targetDistPrediction.Value*0.15f;
            //} else {
            //    futurePrediction = (distance / speed) * targetDistPredictionMult.Value; // the prediction should be accurate enough
            //}

            // Predict the future by taking the velocity of the target and multiply it by the future prediction
            var prevTargetPosition = targetPosition;
            targetPosition = target.Value.transform.position;
            return targetPosition + (targetPosition - prevTargetPosition) * futurePrediction;
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            targetDistPrediction = 20;
            targetDistPredictionMult = 20;
            target = null;
        }
    }
}