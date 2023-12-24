using UnityEngine;

public class TargetClearZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Monster>(out var monster))
        {
            monster.Targets.Clear();
        }
    }
}