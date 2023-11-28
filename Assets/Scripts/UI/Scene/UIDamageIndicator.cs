using LOONACIA.Unity.UI;
using UnityEngine;

public class UIDamageIndicator : UIScene
{
    #region PublicVariables
    public GameObject indicatorParent;
    public GameObject damageIndicator;
    #endregion

    #region PrivateVariables
    private PlayerController m_characterController;
    #endregion

    #region PublicMethod
    private void Start()
    {
        m_characterController = GameObject.Find("@Character").GetComponent<PlayerController>();
        RegisterEvents();
    }
    #endregion

    #region PrivateMethod
    private void RegisterEvents()
    {
        m_characterController.Damaged += OnCharacterDamaged;
        m_characterController.Blocked += OnCharacterBlocked;
    }

    private void OnCharacterDamaged(object sender, DamageInfo e)
    {
        DamageIndicator indi = Instantiate(damageIndicator, indicatorParent.transform).GetComponent<DamageIndicator>();

        indi.Init(e, m_characterController.Character);
    }

    private void OnCharacterBlocked(object sender, DamageInfo e)
    {
        DamageIndicator indi = Instantiate(damageIndicator, indicatorParent.transform).GetComponent<DamageIndicator>();
        Color color = new Color(192f / 255f, 192f / 255f, 192f / 255f);

        indi.Init(e, m_characterController.Character, color);
    }
    #endregion
}
