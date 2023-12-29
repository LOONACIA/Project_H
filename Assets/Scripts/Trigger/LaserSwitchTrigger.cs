using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaserSwitchTrigger : MonoBehaviour
{
    private Collider m_Collider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HackingConsole>(out var console))
        {
            console.Hacking(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<HackingConsole>(out var console))
        {
            console.Recovery();
        }
    }
}
