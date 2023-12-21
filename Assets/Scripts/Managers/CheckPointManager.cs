using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public Vector3 spawnPosition;

    // Start is called before the first frame update
    void OnEnable()
    {
        spawnPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
