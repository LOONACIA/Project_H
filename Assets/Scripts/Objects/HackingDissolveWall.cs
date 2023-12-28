using System.Collections;
using UnityEngine;

public class HackingDissolveWall : HackingObject
{
    #region PublicVariables

    public MeshRenderer mr;
    public Material idleMat;
    public Material hackingMat;
    public Collider col;

    public bool isHacking = false;

    private float returnTime = 3f;

    #endregion

    #region PrivateVariables

    private static readonly int s_dissolveAmountID = Shader.PropertyToID("_DissolveAmount");

    [SerializeField]
    float m_minAmount = 0.85f;

    [SerializeField]
    float m_maxAmount = 0.95f;

    #endregion

    #region PublicMethod

    public override void Interact()
    {
        if (isHacking == true)
            return;

        ConvertMat();

        //Sound
        if (TryGetComponent<AudioSource>(out var audioSource) && audios.Length >= 1)
        {
            audioSource.clip = audios[(int)HackingSoundType.Hacking];
            audioSource.Play();
        }

        ConvertMat();
    }

    private void Start()
    {
        TryGetComponent<MeshRenderer>(out mr);
        TryGetComponent<Collider>(out col);

        idleMat = new Material(Resources.Load<Material>(ConstVariables.WALL_HACKINGNORMALMATERIAL_PATH));
        hackingMat = new Material(Resources.Load<Material>(ConstVariables.WALL_HACKINGMATERIAL_PATH));

        StartCoroutine(IE_Idle());
    }

    #endregion

    #region PrivateMethod

    private void ConvertMat()
    {
        isHacking = true;

        StopCoroutine(IE_Idle());

        StartCoroutine(IE_HackingEffect());
    }

    private void ReturnMat()
    {
        isHacking = false;

        mr.enabled = true;
        col.enabled = true;
        StopAllCoroutines();
        StartCoroutine(IE_Idle());
    }

    private void Dissolve()
    {
        mr.enabled = false;
        col.enabled = false;
    }

    private IEnumerator IE_Idle()
    {
        mr.material = idleMat;
        float value = (m_maxAmount + m_minAmount) / 2;
        idleMat.SetFloat(s_dissolveAmountID, value);
        float mul = -0.01f;
        while (true)
        {
            value += mul;
            idleMat.SetFloat(s_dissolveAmountID, value);

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
            mr.material.SetFloat(s_dissolveAmountID, value);

            value -= 0.005f;

            yield return null;

            if (value < 0.4f)
                break;
        }

        Dissolve();

        yield return new WaitForSeconds(returnTime);

        ReturnMat();

        //Sound
        if (TryGetComponent<AudioSource>(out var audioSource) && audios.Length >= 1)
        {
            audioSource.clip = audios[(int)HackingSoundType.Unhacking];
            audioSource.Play();
        }
    }

    #endregion
}