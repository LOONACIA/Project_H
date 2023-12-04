using LOONACIA.Unity.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialShowSuriken : MonoBehaviour
{
    #region PublicVariables

    public MeshRenderer mr;

    public Material curMaterial;
    public Material shurikenMaterial;
    public Material shurikenOutlineMaterial;

    #endregion

    #region PrivateVariables

    [SerializeField]
    private InputActionReference m_inputAction;

    private bool m_isTriggered;

    #endregion

    #region PublicMethod

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();

        curMaterial = mr.materials[0];
        shurikenMaterial =
            Instantiate<Material>(Resources.Load<Material>(ConstVariables.TUTORIAL_BROKENSHURIKEN_MATERIAL_PATH));
        shurikenOutlineMaterial =
            Instantiate<Material>(
                Resources.Load<Material>(ConstVariables.TUTORIAL_BROKENSHURIKEN_OUTLINEMATERIAL_PATH));
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }
    
    private void OnActionChange(object arg1, InputActionChange arg2)
    {
        UpdateText();
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
        // Trigger the tutorial text
        m_isTriggered = true;
        UpdateText();
    }

    public void UpdateText()
    {
        // If not triggered
        if (!m_isTriggered)
        {
            // Ignore
            return;
        }
        
        string path = m_inputAction.action.bindings
            .SingleOrDefault(binding => binding.groups.Equals(ManagerRoot.Input.CurrentControlScheme))
            .ToDisplayString();
        GameManager.UI.UpdateObject($"{path}키를 눌러서 표창을 날린 다음, 다시 눌러 이동하세요!");
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