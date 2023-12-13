using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    // temp
public class TestScript : MonoBehaviour
{
    [SerializeField]
    private Monster m_monster;

    [SerializeField]
    private Monster m_character;

    [SerializeField]
    private Animator m_animator;

    [SerializeField]
    private float m_angle;

    private void Start()
    {
    }


    private void Update()
    {
        Vector3 from = (m_monster.transform.up * -1).normalized;
        Vector3 to = (m_character.transform.position - m_monster.transform.position).normalized;
        m_angle = Vector3.Angle(from, to)/180;
        m_angle = Mathf.Clamp(m_angle, 0.1f, 0.9f);

        Debug.Log(m_angle);
        m_animator.SetFloat("AimAngle", m_angle);
    }

    public static float GetAngle(Vector3 from, Vector3 to)
    {
        Vector3 v = to - from;
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }
}
