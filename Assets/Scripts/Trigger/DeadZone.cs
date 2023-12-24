using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //TODO: hit이 가능한 Layer만 가져오기 (최적화)
        if (other.TryGetComponent<Actor>(out var actor))
        {
            actor.Health.Kill();
        }
    }
}
