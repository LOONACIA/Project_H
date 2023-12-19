using UnityEngine;

public class BossStageBreakGround : MonoBehaviour
{
    #region PublicVariables
    public GameObject boss;
    #endregion

    #region PrivateVariables
    private MeshRenderer m_mr;
    private MeshCollider m_col;
    private BoxCollider m_trigger;
    #endregion

    #region PublicMethod
    private void Start()
    {
        m_mr = GetComponent<MeshRenderer>();
        m_col = GetComponent<MeshCollider>();
        m_trigger = GetComponent<BoxCollider>();
    }
    #endregion

    #region PrivateMethod
    private void OnTriggerEnter(Collider other)
    {
        m_trigger.enabled = false;
        ThrowPlayer(other.transform);
        BreakGround();
        boss.SetActive(true);
    }

    private void ThrowPlayer(Transform other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        rb.AddForce(Vector3.up * 70f, ForceMode.Impulse);
    }

    private void BreakGround()
    {
        m_mr.enabled = false;
        m_col.enabled = false;

        var children = GetComponentsInChildren<Rigidbody>(true);

        foreach (var child in children) 
        { 
            SetForce(child);
        }
    }

    private void SetForce(Rigidbody rb)
    {   
        rb.gameObject.SetActive(true);
        float x = Random.Range(-0.5f, 0.5f);
        float z = Random.Range(-0.5f, 0.5f);

        Vector3 dir = new Vector3(x, 1f, z).normalized;

        rb.AddForce(dir * 40f , ForceMode.Impulse);
    }
    #endregion
}
