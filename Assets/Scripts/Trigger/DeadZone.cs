using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //TODO: hit이 가능한 Layer만 가져오기 (최적화)
        if (other.TryGetComponent<Actor>(out var actor))
        {
            DamageInfo info = new DamageInfo(99999, actor.transform.position - transform.position, actor.transform.position, actor);
            //actor.Health.TakeDamage(info);
            actor.Health.Kill();
        }
        
        
    }
}
