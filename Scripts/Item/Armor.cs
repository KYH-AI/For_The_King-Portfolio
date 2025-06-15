using UnityEngine;

public class Armor : Equipment
{
    
    #region 변수선언부

    public enum ArmorType
    {
        NONE = -1,
        ArmorSilk = 0,     // 의복
        ArmorCloth = 1,    // 천갑옷
        ArmorLeather = 2,  // 가죽갑옷
        ArmorPlate = 3,    // 중갑
        
       ///////////////////// Headgear /////////////////
       
        Boots = 5,        // 신발
        ShieldPhysical = 6,  // 물리방패
        ShieldMagic = 7,     // 마법방패
        
        HelmetCloth = 999,   // 가죽 모자
        HelmetPlate = 1000,  // 중갑 헬멧
        HelmetWizard = 1001, // 마법사 모자
    }
    
    public ArmorType Armortype { get; private set; }
    
    
    #endregion
    
    
    #region 생성자
    
    public Armor(int itemID, string itemName, int armorType, int itemGrade, 
        int physicalArmorBonus, int resistanceArmorBonus, int extraStat, int extraStatBonus, int penaltyStat, int penaltyStatBonus, int immunity, 
        int buyPrice, int sellPrice, string itemDescription, string itemCreatedAt, string itemUpdatedAt ) 
        : base(itemID, itemName, itemGrade,
            physicalArmorBonus, resistanceArmorBonus, extraStat, extraStatBonus, penaltyStat, penaltyStatBonus, immunity,
            buyPrice, sellPrice, itemDescription, itemCreatedAt, itemUpdatedAt)
    {
        Itemtype = ItemType.Armor;
        Armortype = (ArmorType)armorType;

        // 해당 방어구에 대한 파츠(부위)확인
        EquipmentSlottype = EquipmentCategory(typeof(ArmorType), (int)Armortype);
    }

    public override object Clone()
    {
        Armor armor = (Armor)base.Clone();
        return armor;
    }

    #endregion
    
    
}

