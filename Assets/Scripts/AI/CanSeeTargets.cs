using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanSeeTargets : Conditional
{
    // Start is called before the first frame update
    public SharedTransform target;
    public SharedGameObject self;
    public LayerMask targetLayer;
    public float fovAngle;
    public SharedFloat viewDistance;
    public SharedBool isAiming;

    public override TaskStatus OnUpdate()
    {
        if (isAiming?.Value is true)
        {
            return TaskStatus.Success;
        }
        
        Vector3 selfPosition = self.Value.transform.position + Vector3.up;
        Vector3 targetPosition = target.Value.position + (Vector3.up / 2f);
        //Vector3 targetHeadPosition = target.Value.transform.position + Vector3.up;

        Ray ray = new Ray(selfPosition, (targetPosition - selfPosition).normalized);

        Vector2 selfVector = new Vector2(selfPosition.x, selfPosition.z);
        Vector2 targetVector = new Vector2(targetPosition.x, targetPosition.z);
        //Vector2 targetHeadVector = new Vector2(targetHeadPosition.x, targetHeadPosition.z);

        float distance = Vector3.Distance(selfPosition, targetPosition);
        float angle = Vector2.Angle(selfVector, targetVector);
        float dotProduct = Vector2.Dot(selfVector.normalized, targetVector.normalized);
        //float dotHeadProduct = Vector2.Dot(selfVector.normalized, targetHeadVector.normalized);

        if (angle < fovAngle && (dotProduct > 0.9f /*|| dotHeadProduct >0.9f*/))
        {
            if (Physics.Raycast(ray, out _, distance, targetLayer) || distance > viewDistance.Value)
            {
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Success;
            }
        }
        return base.OnUpdate();
    }
}
