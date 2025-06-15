using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipment
{
    #region 변수선언부

    public enum WeaponType
    {
        None = -1,
        WeaponAxe = 0,
        WeaponBow = 1,
        WeaponDagger = 2,
        WeaponHammer = 3,
        WeaponKnuckle = 4,
        WeaponSpear = 6,
        WeaponStaff = 7,
        WeaponSword = 8,
    }
    
    // 무기 양손 또는 한손 판단
    public enum WeaponHandleGrip
    {
        OneHand = 0,
        TwoHand = 1,
    }

    public enum DamageType
    {
        None = -1,
        Physical = 0,
        Magical = 1,
    }
    
    /*
    public enum WeaponExtraStatType
    {
        None = -1,
        Strength,
        Intellect,
        Recognition,
        Health,
        PhysicalDefense,
        MagicalDefense,
        PhysicalAttack,
        MagicalAttack,
    }
    */
    
    public enum WeaponBaseStat
    {
        Strength = 0,
        Awareness,
        Intelligence,
        Vitality,
        Quickness
    }

    public WeaponType Weapontype { get; private set; }

    public WeaponHandleGrip WeaponHandlegrip { get; private set; }
    
    public WeaponBaseStat WeaponBasestat { get; private set; }
    
    public DamageType Damagetype { get; private set; }
    
    public int Damage { get; private set; }
    

    private List<int> _skills;
    
    public List<int> Skills
    {
        get => _skills;
        set
        {
            _skills = value;
        }
    }

    #endregion

    #region 생성자

    public Weapon(int itemID, string itemName, int itemGrade, int weaponType, int weaponBaseStat, int damageType,
        int damage, int extraStatType, int extraStatBonus,
        int skill1, int skill2, int skill3, int skill4, int skill5,
        int buyPrice, int sellPrice,
        string itemDescription, string itemCreatedAt, string itemUpdatedAt)
        : base(itemID, itemName, itemGrade, 
            0, 0,extraStatType, extraStatBonus, -1, 0, 0, 
            buyPrice, sellPrice, itemDescription, itemCreatedAt, itemUpdatedAt)
    {
        this.Itemtype = ItemType.Weapon;
        this.Damagetype = (DamageType)damageType;
        this.Damage = damage;
        this.Weapontype = (WeaponType)weaponType;
        this.WeaponBasestat = (WeaponBaseStat)weaponBaseStat;
        
        /* 무기 양손무기 판단 */
        if (Weapontype is WeaponType.WeaponKnuckle or WeaponType.WeaponStaff or WeaponType.WeaponBow)
        {
            this.WeaponHandlegrip = WeaponHandleGrip.TwoHand;
        }
        else if(skill1 is 2)
        {
            /* 0 ~ 5 */
            this.WeaponHandlegrip = WeaponHandleGrip.TwoHand;
        }
        else
        {
            this.WeaponHandlegrip = WeaponHandleGrip.OneHand;
        }
        
        int[] temp = { skill1, skill2, skill3, skill4, skill5 };
        this._skills = new List<int>(temp.Length);
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] != -1)
            {
                _skills.Add(temp[i]);

            }
        }
        
        // 해당 무기에 대한 파츠(부위)확인
        EquipmentSlottype = EquipmentCategory(typeof(WeaponType), (int)Weapontype);
    }

    public override object Clone()
    {
        Weapon weapon = (Weapon)base.Clone();
        weapon._skills = new List<int>(_skills);
        return weapon;
    }
    
    #endregion
}

