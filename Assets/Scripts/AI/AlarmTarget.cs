using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class AlarmTarget : Action
{
    // Start is called before the first frame update
    public override TaskStatus OnUpdate()
    {
        
        return TaskStatus.Success;
    }

    public override void OnReset()
    {

    }
}
