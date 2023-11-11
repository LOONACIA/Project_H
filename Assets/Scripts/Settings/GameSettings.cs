using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Settings/" + nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    [Header("Effect")]
    [SerializeField]
    private GameObject m_bloodEffect;
    
    public GameObject BloodEffect => m_bloodEffect;
}