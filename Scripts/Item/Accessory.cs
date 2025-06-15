using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Accessory : Equipment
{
    
    public enum AccessoryStat
    {
        None,
    }

    public enum AccessoryType
    {
        NONE = -1,
        Amulet = 0, // 목걸이
        Ring = 1,  // 반지
    }
    
    public AccessoryType Accessorytype { get; private set; }

    private AccessoryStat _accessoryStat;
    private int _accessoryDamage;
    private int _accessoryDefense;
    private Armor.Immunity _immunity;
    private bool _accessoryMovement;
    private int _speed;
    private Skill _skill;



    /*

    public Accessory(int itemID, string itemName, int itemGrade, AccessoryStat accessoryStat,
        int buyPrice, int sellPrice, int accessoryDamage, int accessoryDefense, Armor.Immunity immunity, bool accessoryMovement, int speed,
        string itemDescription, Skill skill, string itemCreatedAt, string itemUpdatedAt) : base(itemID, itemName, itemGrade, buyPrice, sellPrice, itemDescription, itemCreatedAt, itemUpdatedAt)
    {
        _accessoryStat = accessoryStat;
        _accessoryDamage = accessoryDamage;
        _accessoryDefense = accessoryDefense;
        _immunity = immunity;
        _accessoryMovement = accessoryMovement;
        _speed = speed;
        _skill = skill;
    }
    */
    protected Accessory(int itemID, string itemName, int itemGrade, 
        int physicalArmorBonus, int resistanceArmorBonus, int extraStat, int extraStatBonus, int penaltyStat, int penaltyStatBonus, int immunityType, 
        int buyPrice, int sellPrice, string itemDescription, string itemCreatedAt, string itemUpdatedAt) : 
        base(itemID, itemName, itemGrade, 
            physicalArmorBonus, resistanceArmorBonus, extraStat, extraStatBonus, penaltyStat, penaltyStatBonus, immunityType,
            buyPrice, sellPrice, itemDescription, itemCreatedAt, itemUpdatedAt)
    {
        
        // 해당 장신구에 대한 파츠(부위)확인
        EquipmentSlottype = EquipmentCategory(typeof(AccessoryType), (int)Accessorytype);
    }
}


