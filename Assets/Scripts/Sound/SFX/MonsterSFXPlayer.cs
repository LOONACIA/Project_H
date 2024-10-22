using UnityEngine;

public class MonsterSFXPlayer : MonoBehaviour
{
    #region PublicVariables
    public SFXMonsterData monsterSFX;
    public AudioSource audioSource;

    public MonsterMovement monsterMovement;
    #endregion

    #region PrivateVariables
    private bool isWalkSoundPlaying;
    #endregion

    #region PublicMethod
    public void OnPlayAttack1()
    {
        SendSFXData(monsterSFX.Attack1);
    }
    public void OnPlayAttack2()
    {
        SendSFXData(monsterSFX.Attack2);
    }
    public void OnPlayAttack3()
    {
        SendSFXData(monsterSFX.Attack3);
    }
    public void OnPlayAttack1TP()
    {   
        SendSFXDataAtPoint(GameManager.Sound.ChangeBlend(monsterSFX.Attack1), transform.position);
    }
    public void OnPlayAttack2TP()
    {
        SendSFXDataAtPoint(GameManager.Sound.ChangeBlend(monsterSFX.Attack2), transform.position);
    }
    public void OnPlayAttack3TP()
    {
        SendSFXDataAtPoint(GameManager.Sound.ChangeBlend(monsterSFX.Attack3), transform.position);
    }
    public void OnPlayJump()
    {
        SendSFXData(monsterSFX.Jump);
    }
    public void OnPlayWalk()
    {

    }

    public void OnPlayWalk2()
    {

    }

    public void OnPlayDash()
    {
        SendSFXData(monsterSFX.Dash);
    }
    public void OnPlaySkill()
    {
        SendSFXData(monsterSFX.Skill);
    }
    public void OnPlayShield1()
    {
        SendSFXData(monsterSFX.Shield1);
    }
    public void OnPlayShield2()
    {
        SendSFXData(monsterSFX.Shield2);
    }
    public void OnPlayShield3()
    {
        SendSFXData(monsterSFX.Shield3);
    }
    public void OnPlayShieldPush()
    {
        SendSFXData(monsterSFX.ShieldPush);
    }
    public void OnPlayShuriken()
    {
        SendSFXData(monsterSFX.Shuriken);
    }
    public void OnPlayLanding()
    {
        SendSFXData(monsterSFX.Landing);
    }
    public void OnPlayHit1()
    {
        SendSFXData(monsterSFX.Hit1);
    }
    public void OnPlayHit2()
    {
        SendSFXData(monsterSFX.Hit1);
    }
    public void OnPlayHit3()
    {
        SendSFXData(monsterSFX.Hit1);
    }
    public void OnPlayFPDeath()
    {
        GameManager.Sound.ChangeBGMDirectly(monsterSFX.FPDeath);
    }
    public void OnPlayTPDeath1()
    {
        SendSFXData(monsterSFX.TPDeath1);
    }
    public void OnPlayTPDeath2()
    {
        SendSFXData(monsterSFX.TPDeath2);
    }
    public void OnPlayTPDeat3()
    {
        SendSFXData(monsterSFX.TPDeath3);
    }
    #endregion

    #region PrivateMethod
    private void Awake()
    {
        TryGetComponent<AudioSource>(out  audioSource);     
    }

    private void SendSFXData(SFXInfo _info)
    {
        GameManager.Sound.Play(_info);
    }

    private void SendSFXDataAtPoint(SFXInfo _info, Vector3 _pos)
    {
        GameManager.Sound.PlayClipAt(_info, _pos);
    }

    private bool CheckIsGround()
    {
        return monsterMovement.IsOnGround;
    }
    #endregion
}
