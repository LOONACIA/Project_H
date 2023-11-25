using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CanSeeTargets : Conditional
{
    // Start is called before the first frame update
    public SharedGameObject target;
    public SharedGameObject self;
    public LayerMask targetLayer;
    public float detectRange;
    public float fovAngle;
    public float viewDistance;


    public override TaskStatus OnUpdate()
    {
        Vector3 selfPosition = self.Value.transform.position;
        Vector3 targetPosition = target.Value.transform.position;

        Ray ray = new Ray(selfPosition, targetPosition-selfPosition);
        RaycastHit hit;

        Vector2 selfVector = new Vector2(selfPosition.x, selfPosition.z);
        Vector2 targetVector = new Vector2(targetPosition.x, targetPosition.z);


        float angle = Vector2.Angle(selfVector, targetVector);
        float dotProduct = Vector2.Dot(selfVector.normalized, targetVector.normalized);

        if (angle < fovAngle && dotProduct > 0.9f)
        {
            if(Physics.Raycast(ray, out hit, Vector3.Distance(selfPosition, targetPosition), targetLayer))
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
