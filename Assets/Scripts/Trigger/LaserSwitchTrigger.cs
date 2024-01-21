using LOONACIA.Unity.Coroutines;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaserSwitchTrigger : MonoBehaviour
{
    [SerializeField]
    private float m_delayTime = 1f;

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
            CoroutineEx.Create(this, Co_WaitDelayTime(console));
        }
    }

    private IEnumerator Co_WaitDelayTime(HackingConsole console)
    { 
        yield return new WaitForSeconds(m_delayTime);
        console?.Recovery();
    }
}
