using UnityEngine;

public class PossessionWallLoadingEffect : MonoBehaviour
{
    #region PublicVariables
    public Material normalMaterial;
    public Material normalOutlineMaterial;
    public Material possessionLoadingMaterial;

    public MeshRenderer mr;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Awake()
    {
        mr = this.GetComponent<MeshRenderer>();

        normalMaterial = Instantiate<Material>(Resources.Load<Material>(ConstVariables.WALL_NORMALMATERIAL_PATH));
        normalOutlineMaterial = Instantiate<Material>(Resources.Load<Material>(ConstVariables.WALL_NORMALOUTLINEMATERIAL_PATH));
        possessionLoadingMaterial = Instantiate<Material>(Resources.Load<Material>(ConstVariables.WALL_POSSESSIONLOADINGMATERIAL_PATH));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ShowPossessionEffect();
        }
    }
    #endregion

    #region PrivateMethod
    private void ShowPossessionEffect()
    {
        Material[] mat = mr.materials;

        //this.GetComponent<Renderer>().material.SetFloat("SciFI_URP/Sci-FiURP", 0f);

        mat[0] = Instantiate<Material>(Resources.Load<Material>(ConstVariables.WALL_POSSESSIONLOADINGMATERIAL_PATH));
        mat[1] = null;

        Shader.SetGlobalFloat("_Time", 0f);
        Debug.Log(Shader.GetGlobalVector("_Time"));
        mr.materials = mat;

    }
    #endregion
}
