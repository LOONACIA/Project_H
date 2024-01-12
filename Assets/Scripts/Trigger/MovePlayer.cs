using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private CheckPointManager m_checkPointManager;

    // Start is called before the first frame update
    void Start()
    {
        m_checkPointManager = GetComponent<CheckPointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Actor>(out var actor))
        {
            // Ignore
            return;
        }

        if (actor.IsPossessed)
        {
            if (actor.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.velocity = Vector3.zero;
            }

            other.transform.position = GameManager.Instance.savePoint;
        }
    }
}
