using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeactiveTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_gameObjectList;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Actor>(out var actor) || actor.IsPossessed == false)
        {
            return;
        }

        foreach (var item in m_gameObjectList) 
        {
            item.SetActive(false); 
        }
    }
}
