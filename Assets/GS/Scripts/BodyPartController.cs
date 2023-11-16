using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartController : MonoBehaviour
{
    public ActorHealth actorHealth;
    public event EventHandler TestEvent;

    private List<Transform> parts = new();

    private void Start()
    {
        var temp = GetComponentsInChildren<Transform>(true);
        foreach (var temp2 in temp)
        {
            if (temp2.gameObject.name.Contains("_cell"))
            { 
                parts.Add(temp2);
                var script = temp2.gameObject.AddComponent<BodyPartScript>();
                script.actorHealth = actorHealth;
            }
        }
    }
}
