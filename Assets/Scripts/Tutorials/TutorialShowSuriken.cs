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
        if (!isActiveAndEnabled)
        {
            return;
        }
        
        StartCoroutine(nameof(IE_ShowShuriken));
    }

    public void ShowShurikenObject()
    {
        GameManager.UI.UpdateObject("R키를 눌러서 표창을 날린 다음, 다시 눌러 이동하세요!");
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

            if (value <= 0.7f)
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
