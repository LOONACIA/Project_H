using UnityEngine;

public class WallGridSizeSetting : MonoBehaviour
{
    #region PublicVariables
    public MeshRenderer mr;
    #endregion

    #region PrivateVariables
    #endregion

    #region PublicMethod
    private void Awake()
    {
        TryGetComponent<MeshRenderer>(out mr);
    }

    private void Start()
    {
        InitSetting();   
    }
    #endregion

    #region PrivateMethod
    private void InitSetting()
    {
        float sizeX = transform.lossyScale.x / 4f;
        float sizeZ = transform.lossyScale.z / 4f;
        //mr.material.SetTextureOffset("_MainTex", new Vector2(maxSize, maxSize));
        mr.material.mainTextureScale = new Vector2(sizeZ, sizeX);
    }
    #endregion
}
