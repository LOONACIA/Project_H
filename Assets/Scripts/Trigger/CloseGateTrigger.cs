using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CloseGateTrigger : MonoBehaviour
{
    [SerializeField]
    private DissolveGate[] m_gates;

    private PossessionProcessor m_processor;

    private bool m_isClosed;

    private void Start()
    {
        m_processor = FindObjectOfType<PossessionProcessor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_isClosed == false 
            && other.gameObject.TryGetComponent<Monster>(out var montser) 
            && montser.IsPossessed)
        {
            m_isClosed = true;

            m_processor.ClearTarget();

            foreach (var gate in m_gates)
            {
                StartCoroutine(gate.Close());
            }
        }
    }
}
