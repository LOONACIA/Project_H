using UnityEngine;

public class TriggerSetActive : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_gameObject; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {   
        if(!other.TryGetComponent<Actor>(out var actor) || actor.IsPossessed == false)
        {
            return;
        }

        foreach (var item in m_gameObject) { item.SetActive(true); }

    }
}
