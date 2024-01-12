using UnityEngine;

public class ActivateOnInteracted : MonoBehaviour
{
    [SerializeField]
    private InteractableObject m_interactableObject;

    [SerializeField]
    private GameObject m_gameObject;

    private void Start()
    {
        m_interactableObject.Interacted += OnInteracted;

        m_gameObject?.SetActive(false);
    }

    private void OnInteracted(object sender, Transform e)
    {
        m_gameObject?.SetActive(true);
    }
}
