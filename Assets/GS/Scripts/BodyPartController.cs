using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodyPartController : MonoBehaviour
{
    private IHealth m_health;

    private List<BodyPartScript> m_bodyPartScripts = new();

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
                var bodyPartScript = child.gameObject.AddComponent<BodyPartScript>();
                
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

    private void ReplaceBody(object sender, EventArgs e)
    {
        foreach (var bodyPartScript in m_bodyPartScripts)
        {
            bodyPartScript.gameObject.SetActive(true);

            bodyPartScript.ReplaceBodyPart();
            bodyPartScript.transform.SetParent(m_bodyPartCollector.transform);
        }

        gameObject.SetActive(false);
    }
}
