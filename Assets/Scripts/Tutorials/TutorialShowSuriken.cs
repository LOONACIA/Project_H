using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialShowSuriken : MonoBehaviour
{
    #region PublicVariables
    public MeshRenderer mr;

    public Material curMaterial;
    public Material shurikenMaterial;
    public Material shurikenOutlineMaterial;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();

        curMaterial = mr.materials[0];
        shurikenMaterial = Instantiate<Material>(Resources.Load<Material>(ConstVariables.TUTORIAL_BROKENSHURIKEN_MATERIAL_PATH));
        shurikenOutlineMaterial = Instantiate<Material>(Resources.Load<Material>(ConstVariables.TUTORIAL_BROKENSHURIKEN_OUTLINEMATERIAL_PATH));
    }

    public void ShowShuriken()
    {
        StartCoroutine(nameof(IE_ShowShuriken));
    }
    #endregion

    #region PrivateMethod
    private IEnumerator IE_ShowShuriken()
    {
        float value = 1f;

        while (true)
        {   
            curMaterial.SetFloat("_DissolveAmount", value);
            value -= 0.0003f;
            yield return null;

            if (value <= 0.6f)
                break;
        }

        ChangeMaterial();
    }

    private void ChangeMaterial()
    {
        Material[] mat = new Material[2];

        
        mat[0] = shurikenMaterial;
        mat[1] = shurikenOutlineMaterial;

        mr.materials = mat;
    }
    #endregion
}
