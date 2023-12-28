using LOONACIA.Unity.Managers;
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

    private void OnCharacterDamaged(object sender, in AttackInfo e)
    {
        // TODO: 로직 최적화
        var indicator = ManagerRoot.Resource.Instantiate(damageIndicator, indicatorParent.transform)
            .GetComponent<DamageIndicator>();
        Color color = new(1f, 0.29803923f, 0.29803923f);
        //DamageIndicator indi = Instantiate(damageIndicator, indicatorParent.transform).GetComponent<DamageIndicator>();

        indicator.Init(e, m_characterController.Character, color);
    }

    private void OnCharacterBlocked(object sender, in AttackInfo e)
    {
        // TODO: 로직 최적화
        //DamageIndicator indi = Instantiate(damageIndicator, indicatorParent.transform).GetComponent<DamageIndicator>();
        var indicator = ManagerRoot.Resource.Instantiate(damageIndicator, indicatorParent.transform)
            .GetComponent<DamageIndicator>();
        Color color = new(192f / 255f, 192f / 255f, 192f / 255f);

        indicator.Init(e, m_characterController.Character, color);
    }
    #endregion
}
