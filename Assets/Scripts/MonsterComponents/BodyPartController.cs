using System.Collections.Generic;
using UnityEngine;

public class BodyPartController : MonoBehaviour
{
    private IHealth m_health;

    private List<Breakable> m_bodyPartScripts = new();

    private GameObject m_bodyPartCollector;

    private void Start()
    {
        m_health = GetComponentInParent<IHealth>(true);
        m_health.Dying += ReplaceBody;

        var children = GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Body Parts"))
            { 
                var bodyPartScript = child.gameObject.AddComponent<Breakable>();
                
                m_bodyPartScripts.Add(bodyPartScript);

                child.gameObject.layer = LayerMask.NameToLayer("Body Parts");
            }
        }

        m_bodyPartCollector = GameObject.Find("BodyPartCollector");
        if (m_bodyPartCollector == null)
        {
            m_bodyPartCollector = new GameObject() { name = "BodyPartCollector" };
        }
    }

    private void ReplaceBody(object sender, DamageInfo info)
    {
        foreach (var bodyPartScript in m_bodyPartScripts)
        {
            bodyPartScript.gameObject.SetActive(true);

            bodyPartScript.ReplacePart(info);
            bodyPartScript.transform.SetParent(m_bodyPartCollector.transform);
        }

        gameObject.SetActive(false);
    }
}
