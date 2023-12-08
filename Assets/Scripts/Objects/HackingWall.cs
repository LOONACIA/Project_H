using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class HackingWall : HackingObject
{
    #region PublicVariables
    public MeshRenderer mr;
    public Material idleMat;
    public Material hackingMat;
    public Collider col;

    public bool isHacking = false;
    #endregion

    #region PrivateVariables
    [SerializeField] float m_minAmount = 0.85f;
    [SerializeField] float m_maxAmount = 0.95f;
    #endregion

    #region PublicMethod
    public override void Interact()
    {
        ConvertMat();
    }

    private void Start()
    {
        TryGetComponent<MeshRenderer>(out mr);
        TryGetComponent<Collider>(out col);

        idleMat = mr.materials[0];
        StartCoroutine(IE_Idle());
        col.isTrigger = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ConvertMat();
        }
    }
    #endregion

    #region PrivateMethod
    private void ConvertMat()
    {
        StopCoroutine(IE_Idle());

        col.isTrigger = false;
        StartCoroutine(IE_HackingEffect());
    }

    private IEnumerator IE_Idle()
    {
        float value = m_maxAmount;
        float mul = -0.01f;
        while (true)
        {
            value += mul;
            idleMat.SetFloat("_DissolveAmount", value);

            yield return new WaitForSeconds(0.05f);

            if (value < m_minAmount || value > m_maxAmount)
                mul *= -1;
        }
    }

    private IEnumerator IE_HackingEffect()
    {
        mr.material = hackingMat;
        float value = 0.6f;

        while (true)
        {
            mr.material.SetFloat("_DissolveAmount", value);

            value -= 0.005f;

            yield return null;

            if (value < 0f)
                break;
        }
    }
    #endregion
}
