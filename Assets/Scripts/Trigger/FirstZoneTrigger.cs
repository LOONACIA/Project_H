using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirstZoneTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject m_firstZone;

    private void OnTriggerEnter(Collider other)
    {
        var character = FindObjectsOfType<Monster>()
            .SingleOrDefault(monster => monster.IsPossessed);

        if(other.name == character.name)
        {
            Debug.Log(other);
            m_firstZone.SetActive(true);
            Destroy(this);
        }
    }
}
