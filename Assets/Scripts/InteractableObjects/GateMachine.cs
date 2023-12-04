using UnityEngine;

[RequireComponent(typeof(Alarm))]
public class GateMachine : InteractableObject
{
    [SerializeField]
    private GameObject m_gate;

    [SerializeField]
    private bool m_alarmWhenOpen = true;

    private IGate m_gateScript;
    
    private Alarm m_alarm;

    private void Awake()
    {
        m_gateScript = m_gate.GetComponent<IGate>();
        m_alarm = GetComponentInChildren<Alarm>();
    }

    protected override void OnInteractStart(Actor actor)
    {
        if (m_alarmWhenOpen)
        {
            m_alarm.Trigger(actor);
        }
    }

    protected override void OnInteract(Actor actor)
    {
        IsInteractable = false;

        if (m_gate != null && m_gateScript != null)
        {
            StartCoroutine(m_gateScript.Open());
        }
    }
}
