using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanSeeTargets : Conditional
{
    // Start is called before the first frame update
    public SharedGameObject target;
    public SharedGameObject self;
    public LayerMask targetLayer;
    public float fovAngle;
    public SharedFloat viewDistance;

    public override TaskStatus OnUpdate()
    {
        Vector3 selfPosition = self.Value.transform.position + new Vector3(0f, 0.5f, 0f);
        Vector3 targetPosition = target.Value.transform.position;

        Ray ray = new Ray(selfPosition, targetPosition - selfPosition);
        RaycastHit hit;

        Vector2 selfVector = new Vector2(selfPosition.x, selfPosition.z);
        Vector2 targetVector = new Vector2(targetPosition.x, targetPosition.z);

        float distance = Vector3.Distance(selfPosition, targetPosition);
        float angle = Vector2.Angle(selfVector, targetVector);
        float dotProduct = Vector2.Dot(selfVector.normalized, targetVector.normalized);

        if (angle < fovAngle && dotProduct > 0.9f)
        {
            if (Physics.Raycast(ray, out hit, distance, targetLayer) || distance > viewDistance.Value)
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
