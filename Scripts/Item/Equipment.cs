using System;
using System.Collections.Generic;

public class Equipment : Item
{
    public enum EquipmentStat
    {
        None = -1,
        Strength = 0,
        Awareness = 1,
        Intelligence = 2,
        Vitality = 3,
        Quickness = 4,
        Avoid = 5,
        MaxFocus = 6,
        PhysicalArmor = 7,
        Resistance = 8,
        PhysicalDamage = 9,
        MagicalDamage = 10,
    }

    // 장착되는 아이템 부위 구별
    public enum EquipmentSlotType
    {
        NONE = -1,
        PrimaryWeapon = 0, // 주무기
        Shield,       // 방패
        Clothes, // 갑옷
        Helmet,  // 헬멧
        Boots,   // 신발
        Ring,           // 반지
        Amulet,         // 목걸이
    }

    public EquipmentSlotType EquipmentSlottype { get; set; }
    
    public enum Immunity
    {
        None,
        Stun,
    }
    
    // 착용 장비에 대한 영향받는 스텟종류
    public EquipmentStat EquipmentExtraStatType { get; private set; }
    
    // 착용 장비에 대한 스텟 증가치 
    public int ModifyEquipmentExtraStatBonus { get; private set; }
    
    // 착용 장비에 대한 영향받는 스텟종류
    public EquipmentStat EquipmentPenaltyStatType { get; private set; }
    
    // 착용 장비에 대한 스텟 감소치
    public int ModifyEquipmentPenaltyStatBonus { get; private set; }
    
    // 착용 장비에 대한 물리방어력 증가 및 감소치
    public int ModifyEquipmentPhysicalArmorBonus { get; private set; }
    
    // 착용 장비에 대한 마법방어력 증가 및 감소치
    public int ModifyEquipmentResistanceArmorBonus { get; private set; }
    
    // 착용 장비에 대한 면역 토큰종류
    public Immunity ImmunityType { get; private set; }
    
    // 착용 장비에 대한 토큰 면역력 값
    public int ImmunityBonus { get; private set; }
    
    protected Equipment(int itemID, string itemName, int itemGrade,
        int physicalArmorBonus, int resistanceArmorBonus, int extraStat, int extraStatBonus, int penaltyStat, int penaltyStatBonus, int immunityType, 
        int buyPrice, int sellPrice, string itemDescription, 
        string itemCreatedAt, string itemUpdatedAt )
        : base(itemID, itemName, itemGrade, buyPrice, sellPrice, itemDescription, itemCreatedAt, itemUpdatedAt)
    {
        ModifyEquipmentPhysicalArmorBonus = physicalArmorBonus;
        ModifyEquipmentResistanceArmorBonus = resistanceArmorBonus;

        EquipmentExtraStatType = (EquipmentStat)extraStat;
        ModifyEquipmentExtraStatBonus = extraStatBonus;
        EquipmentPenaltyStatType = (EquipmentStat)penaltyStat;
        ModifyEquipmentPenaltyStatBonus = penaltyStatBonus;

        ImmunityType = (Immunity)immunityType;

    }

    #region 착용 장비에 대한 카테고리 분류
    
    // 각 장비에 대한 파츠를 구별하는 용도
    private static Dictionary<Type, Func<int, EquipmentSlotType>> _equipmentCategoryDictionary = new Dictionary<Type, Func<int, EquipmentSlotType>>()
    {
            { typeof(Armor.ArmorType), armorType => ArmorCategory(armorType) },
            { typeof(Weapon.WeaponType), weaponType => EquipmentSlotType.PrimaryWeapon },
            { typeof(Accessory.AccessoryType), accessoryType => AccessoryCategory(accessoryType) }
    };

    /// <summary>
    /// 방어구 카테고리 분류
    /// </summary>
    /// <param name="equipmentArmorType">방어구 타입</param>
    /// <returns>방어구 파츠 값</returns>
    private static EquipmentSlotType ArmorCategory(int equipmentArmorType)
    { 
        EquipmentSlotType resultSlotType = EquipmentSlotType.NONE;
        Armor.ArmorType armorType = (Armor.ArmorType)equipmentArmorType;
        
        switch (armorType)
        {
            case >= Armor.ArmorType.HelmetCloth and <= Armor.ArmorType.HelmetWizard:
                // 모자일 때 처리 
                resultSlotType = EquipmentSlotType.Helmet;
                break;
            case >=  Armor.ArmorType.ShieldPhysical and <=  Armor.ArmorType.ShieldMagic:
                // 방패일 때 처리
                resultSlotType = EquipmentSlotType.Shield;
                break;
            case  Armor.ArmorType.Boots:
                // 신발일 때 처리
                resultSlotType = EquipmentSlotType.Boots;
                break;
            case >=  Armor.ArmorType.ArmorSilk and <=  Armor.ArmorType.ArmorPlate:
                // 갑옷일 때 처리
                resultSlotType = EquipmentSlotType.Clothes;
                break;
            default: UnityEngine.Debug.LogError($"{equipmentArmorType}은 처리할수 없는 아이템 타입입니다.");
                // 해당하지 않는 경우 처리
                break;
        }
        return resultSlotType;
    }
    
    /// <summary>
    /// 장신구 카테고리 분류
    /// </summary>
    /// <param name="equipmentArmorType">장신구 타입</param>
    /// <returns>장신구 파츠 값</returns>
    private static EquipmentSlotType AccessoryCategory(int equipmentAccessoryType)
    { 
        EquipmentSlotType resultSlotType = EquipmentSlotType.NONE;
        Accessory.AccessoryType accessoryType = (Accessory.AccessoryType)equipmentAccessoryType;

        // 목걸이 아니면 반지 결과확인
        resultSlotType = (EquipmentSlotType)(accessoryType == 0 ? Accessory.AccessoryType.Amulet : Accessory.AccessoryType.Ring);
    
        return resultSlotType;
    }

    /// <summary>
    /// 각 착용장비의 파츠를 구별하기 위한함수 (장비에서 호출함)
    /// </summary>
    /// <param name="equipmentItemType">장비에 대한 타입 확인(방어구, 무기, 장신구)</param>
    /// <param name="itemTypeCode">각 착용장비에 대한 부위별 확인</param>
    /// <returns>해당 착용장비에 대한 파츠 결과 값</returns>
    protected static EquipmentSlotType EquipmentCategory(Type equipmentItemType, int itemTypeCode)
    {
        if(_equipmentCategoryDictionary.TryGetValue(equipmentItemType, out Func<int, EquipmentSlotType> categoryFunction))
        {
            return categoryFunction(itemTypeCode);
        }
        
        // 딕셔너리에 해당 타입이 없는 경우 또는 딕셔너리 값이 null인 경우
        UnityEngine.Debug.LogError($"{equipmentItemType}에 대한 카테고리 함수를 찾을 수 없습니다.");
        return EquipmentSlotType.NONE;
    }
    /* [사용예시] EquipmentSlottype = EquipmentCategory(typeof(ArmorType), (int)Armortype);  */
    
    #endregion

}
