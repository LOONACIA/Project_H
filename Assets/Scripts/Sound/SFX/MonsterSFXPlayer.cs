using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSFXPlayer : MonoBehaviour
{
    #region PublicVariables
    public SFXMonsterData monsterSFX;
    public AudioSource audioSource;
    #endregion

    #region PrivateVariables
    private bool isWalkSoundPlaying = false;
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
    public void OnPlayJump()
    {
        SendSFXData(monsterSFX.Jump);
    }
    public void OnPlayWalk()
    {
        SendSFXData(monsterSFX.Walk);
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
        SendSFXData(monsterSFX.FPDeath);
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
        GameManager.Sound.Play(_info.audio, _info.type, _info.volume, _info.pitch, _info.priority);
    }

    private void FixedUpdate()
    {
        Animator ani = GetComponent<Animator>();
        float speed = ani.GetFloat("MovementRatio");

        bool isGround = false;
        if (transform.parent.parent != null && transform.parent.parent.GetComponent<MonsterMovement>() != null)
            isGround = transform.parent.parent.GetComponent<MonsterMovement>().IsOnGround;

        if(speed > 0.1f && isGround)
        {
            if (!isWalkSoundPlaying)
            {
                PlayWalkSound();
                isWalkSoundPlaying = true;
            }
            return;
        }

        audioSource?.Stop();
        isWalkSoundPlaying = false;
    }

    private void PlayWalkSound()
    {
        audioSource.clip = monsterSFX.Walk.audio;
        audioSource.loop = true;
        audioSource.Play();
    }
    #endregion
}
