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

    // �����Ǵ� ������ ���� ����
    public enum EquipmentSlotType
    {
        NONE = -1,
        PrimaryWeapon = 0, // �ֹ���
        Shield,       // ����
        Clothes, // ����
        Helmet,  // ���
        Boots,   // �Ź�
        Ring,           // ����
        Amulet,         // �����
    }

    public EquipmentSlotType EquipmentSlottype { get; set; }
    
    public enum Immunity
    {
        None,
        Stun,
    }
    
    // ���� ��� ���� ����޴� ��������
    public EquipmentStat EquipmentExtraStatType { get; private set; }
    
    // ���� ��� ���� ���� ����ġ 
    public int ModifyEquipmentExtraStatBonus { get; private set; }
    
    // ���� ��� ���� ����޴� ��������
    public EquipmentStat EquipmentPenaltyStatType { get; private set; }
    
    // ���� ��� ���� ���� ����ġ
    public int ModifyEquipmentPenaltyStatBonus { get; private set; }
    
    // ���� ��� ���� �������� ���� �� ����ġ
    public int ModifyEquipmentPhysicalArmorBonus { get; private set; }
    
    // ���� ��� ���� �������� ���� �� ����ġ
    public int ModifyEquipmentResistanceArmorBonus { get; private set; }
    
    // ���� ��� ���� �鿪 ��ū����
    public Immunity ImmunityType { get; private set; }
    
    // ���� ��� ���� ��ū �鿪�� ��
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

    #region ���� ��� ���� ī�װ� �з�
    
    // �� ��� ���� ������ �����ϴ� �뵵
    private static Dictionary<Type, Func<int, EquipmentSlotType>> _equipmentCategoryDictionary = new Dictionary<Type, Func<int, EquipmentSlotType>>()
    {
            { typeof(Armor.ArmorType), armorType => ArmorCategory(armorType) },
            { typeof(Weapon.WeaponType), weaponType => EquipmentSlotType.PrimaryWeapon },
            { typeof(Accessory.AccessoryType), accessoryType => AccessoryCategory(accessoryType) }
    };

    /// <summary>
    /// �� ī�װ� �з�
    /// </summary>
    /// <param name="equipmentArmorType">�� Ÿ��</param>
    /// <returns>�� ���� ��</returns>
    private static EquipmentSlotType ArmorCategory(int equipmentArmorType)
    { 
        EquipmentSlotType resultSlotType = EquipmentSlotType.NONE;
        Armor.ArmorType armorType = (Armor.ArmorType)equipmentArmorType;
        
        switch (armorType)
        {
            case >= Armor.ArmorType.HelmetCloth and <= Armor.ArmorType.HelmetWizard:
                // ������ �� ó�� 
                resultSlotType = EquipmentSlotType.Helmet;
                break;
            case >=  Armor.ArmorType.ShieldPhysical and <=  Armor.ArmorType.ShieldMagic:
                // ������ �� ó��
                resultSlotType = EquipmentSlotType.Shield;
                break;
            case  Armor.ArmorType.Boots:
                // �Ź��� �� ó��
                resultSlotType = EquipmentSlotType.Boots;
                break;
            case >=  Armor.ArmorType.ArmorSilk and <=  Armor.ArmorType.ArmorPlate:
                // ������ �� ó��
                resultSlotType = EquipmentSlotType.Clothes;
                break;
            default: UnityEngine.Debug.LogError($"{equipmentArmorType}�� ó���Ҽ� ���� ������ Ÿ���Դϴ�.");
                // �ش����� �ʴ� ��� ó��
                break;
        }
        return resultSlotType;
    }
    
    /// <summary>
    /// ��ű� ī�װ� �з�
    /// </summary>
    /// <param name="equipmentArmorType">��ű� Ÿ��</param>
    /// <returns>��ű� ���� ��</returns>
    private static EquipmentSlotType AccessoryCategory(int equipmentAccessoryType)
    { 
        EquipmentSlotType resultSlotType = EquipmentSlotType.NONE;
        Accessory.AccessoryType accessoryType = (Accessory.AccessoryType)equipmentAccessoryType;

        // ����� �ƴϸ� ���� ���Ȯ��
        resultSlotType = (EquipmentSlotType)(accessoryType == 0 ? Accessory.AccessoryType.Amulet : Accessory.AccessoryType.Ring);
    
        return resultSlotType;
    }

    /// <summary>
    /// �� ��������� ������ �����ϱ� �����Լ� (��񿡼� ȣ����)
    /// </summary>
    /// <param name="equipmentItemType">��� ���� Ÿ�� Ȯ��(��, ����, ��ű�)</param>
    /// <param name="itemTypeCode">�� ������� ���� ������ Ȯ��</param>
    /// <returns>�ش� ������� ���� ���� ��� ��</returns>
    protected static EquipmentSlotType EquipmentCategory(Type equipmentItemType, int itemTypeCode)
    {
        if(_equipmentCategoryDictionary.TryGetValue(equipmentItemType, out Func<int, EquipmentSlotType> categoryFunction))
        {
            return categoryFunction(itemTypeCode);
        }
        
        // ��ųʸ��� �ش� Ÿ���� ���� ��� �Ǵ� ��ųʸ� ���� null�� ���
        UnityEngine.Debug.LogError($"{equipmentItemType}�� ���� ī�װ� �Լ��� ã�� �� �����ϴ�.");
        return EquipmentSlotType.NONE;
    }
    /* [��뿹��] EquipmentSlottype = EquipmentCategory(typeof(ArmorType), (int)Armortype);  */
    
    #endregion

}
