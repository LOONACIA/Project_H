using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ChangeWeaponAbility : Ability
{
    [SerializeField]
    private WeaponInfo m_primaryWeaponInfo;

    [SerializeField]
    private WeaponInfo m_secondaryWeaponInfo;

    [SerializeField]
    private TwoBoneIKConstraint m_leftHandIK;

    [SerializeField]
    private TwoBoneIKConstraint m_rightHandIK;

    private void Start()
    {
        ChangeWeapon(m_secondaryWeaponInfo, m_primaryWeaponInfo);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ChangeWeapon(m_secondaryWeaponInfo, m_primaryWeaponInfo);
    }

    protected override void OnActivateState()
    {
        (WeaponInfo oldWeapon, WeaponInfo newWeapon) = m_primaryWeaponInfo.Weapon.IsEquipped
            ? (m_primaryWeaponInfo, m_secondaryWeaponInfo)
            : (m_secondaryWeaponInfo, m_primaryWeaponInfo);

        ChangeWeapon(oldWeapon, newWeapon);
    }

    private void ChangeWeapon(WeaponInfo oldWeapon, WeaponInfo newWeapon)
    {
        if (oldWeapon.WeaponObject != newWeapon.WeaponObject
            && oldWeapon.WeaponObject != null && newWeapon.WeaponObject != null)
        {
            oldWeapon.WeaponObject.SetActive(false);
            newWeapon.WeaponObject.SetActive(true);
        }

        if (Owner.Attack is not null)
        {
            Owner.Attack.ChangeWeapon(newWeapon.Weapon);
        }

        if (m_leftHandIK != null)
        {
            m_leftHandIK.data.target = newWeapon.LeftHand;
        }

        if (m_rightHandIK != null)
        {
            m_rightHandIK.data.target = newWeapon.RightHand;
        }
    }

    [Serializable]
    private class WeaponInfo
    {
        [field: SerializeField]
        public Weapon Weapon { get; private set; }

        [field: SerializeField]
        public GameObject WeaponObject { get; private set; }

        [field: SerializeField]
        public Transform LeftHand { get; private set; }

        [field: SerializeField]
        public Transform RightHand { get; private set; }
    }
}