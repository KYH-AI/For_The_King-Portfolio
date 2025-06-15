using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PlayerStats : ICloneable
{

    public struct PlayerMainHudDisplay
    {
        public string Text;

        public Color Color;
    }

    public enum StatType
    {
        Strength = 0,
        Intelligence,
        Awareness,
        Quickness,
        Vitality,
    }

    public enum DefenseType
    {
        PhysicalArmor = 0,
        Resistance,
        Avoid
    }

    // 변경이 필요한 스텟
    public enum ModifyStatType
    {
        NONE = -1,
        ModifyStrength = 0,
        ModifyAwareness = 1,
        ModifyIntelligence = 2,
        ModifyVitality = 3,
        ModifyQuickness = 4,
        ModifyMaxFocus = 5,
        ModifyMaxHealth = 6,
        ModifyPhysicalArmor = 7,
        ModifyResistance = 8,
        ModifyAvoid = 9,
        ModifyDamage = 10,
        ModifyPhysicalDamage = 11,
        ModifyMagicDamage = 12,
        ModifyCriticalChance = 13
    }

    private static readonly Dictionary<Equipment.EquipmentStat, ModifyStatType> _STAT_CONVERSION = new Dictionary<Equipment.EquipmentStat, ModifyStatType>()
    {
        { Equipment.EquipmentStat.Strength, ModifyStatType.ModifyStrength },
        { Equipment.EquipmentStat.Awareness, ModifyStatType.ModifyAwareness },
        { Equipment.EquipmentStat.Intelligence, ModifyStatType.ModifyIntelligence },
        { Equipment.EquipmentStat.Vitality, ModifyStatType.ModifyVitality },
        { Equipment.EquipmentStat.Quickness, ModifyStatType.ModifyQuickness },
        { Equipment.EquipmentStat.Avoid, ModifyStatType.ModifyAvoid },
        { Equipment.EquipmentStat.MaxFocus, ModifyStatType.ModifyMaxFocus },
        { Equipment.EquipmentStat.PhysicalArmor, ModifyStatType.ModifyPhysicalArmor },
        { Equipment.EquipmentStat.Resistance, ModifyStatType.ModifyResistance },
        { Equipment.EquipmentStat.PhysicalDamage, ModifyStatType.ModifyPhysicalDamage },
        { Equipment.EquipmentStat.MagicalDamage, ModifyStatType.ModifyMagicDamage },
    };

    private static readonly int[] _PLAYER_LEVEL_TABLE =
    {
        /* 0 ~ 14 레벨 */
        0, 
        25,
        70,
        130,
        220,
        350,
        510,
        920,
        1550,
        2800,
        4500,
        6150,
        820,
        11900,
        22900
    };

    private static readonly int PLAYER_MAX_LEVEL = _PLAYER_LEVEL_TABLE.Length - 1;
    
    private static readonly int _PLAYER_LEVEL_UP_FOCUS_GAIN = 2;

    private static readonly int _MAX_FOCUS_LIMIT = 10;

    public int TempUsedFocus = 0;

    #region 캐릭터 Dummy
    
    // 월드맵 캐릭터 Dummy
    public I_Dummy WorldMapCharacterDummy { get; set; }
    
    // 전투 캐릭터 Dummy
    public I_Dummy BattleMapCharacterDummy { get; set; }
    
    #endregion
    
    
    #region 속성

    /* 플레이어 인벤토리 */
    public  PlayerInventory PlayerInventory { get; private set; }
    
    /* 플레이어 성소 정보 */
    public Sanctuary.SanctuaryType CurrentSanctuaryType { get; private set; } = Sanctuary.SanctuaryType.None;


    /* 고유 속성 */
    public int PlayerID { get; private set; }
    public int BaseLevel { get; private set; }
    public string BaseClassName;
    private float _baseStrength;
    private float _baseAwareness;
    private float _baseIntelligence;
    private float _baseVitality;
    private float _baseQuickness;
    private int _baseMaxFocus;
    private int _baseMaxHealth;
    private int _basePhysicalArmor;
    private int _baseResistance;
    private int _baseAvoid;
    private int _baseDamage;
    public int BaseMovement { get; set; } = 5;
    private float _baseCriticalChance;
    public Sprite PlayerPortrait;
    public Weapon BaseWeapon { get;  set; }

    /* 변경되는 속성 */
    private float _modifyStrength;
    private float _modifyAwareness;
    private float _modifyIntelligence;
    private float _modifyVitality;
    private float _modifyQuickness;
    private int _modifyMaxFocus;
    public int ModifyMaxHealth { get; private set; }
    public int ModifyHpRegen { get; private set; }
    public int ModifyPhysicalArmor { get; private set; }
    public int ModifyResistance { get; private set; }
    private int _modifyAvoid;
    private int _modifyDamageMulti;
    public int ModifyPhysicalDamage { get; private set; }
    public int ModifyMagicDamage{ get; private set; }
    public int ModifyMovement { get; private set; }
    private float _modifyCriticalChance;
    public float ModifyXpMulti { get; private set; }
    public float ModifyGoldMulti { get; private set; }
    private int _modifyNextLevel;
    
    public int CurrentXp { get; private set; }

    public int MaxXp
    {
        get { return _PLAYER_LEVEL_TABLE[_modifyNextLevel]; }
    }
    
    public int CurrentHp { get; private set; }
    public int CurrentFocus { get; private set; }
    public int CurrentGold { get; private set; }
    public int CurrentWeaponId { get; set; } // 현재 장착한 무기 ID
    
    #endregion

    public PlayerStats(int playerID, int baseLevel, string baseClassName, 
                        int baseStrength, int baseAwareness, int baseIntelligence, int baseVitality, int baseQuickness, 
                        int baseAvoid, int baseMaxFocus, int baseMaxHealth, int basePhysicalArmor, int baseResistance, int baseCriticalChance, int baseWeaponId)
    {
        PlayerID = playerID;
        BaseLevel = baseLevel;
        BaseClassName = baseClassName;
        _baseStrength = CastingValue.IntToFloat(baseStrength);
        _baseAwareness = CastingValue.IntToFloat(baseAwareness);
        _baseIntelligence = CastingValue.IntToFloat(baseIntelligence);
        _baseVitality = CastingValue.IntToFloat(baseVitality);
        _baseQuickness = CastingValue.IntToFloat(baseQuickness);
        _baseMaxFocus = baseMaxFocus;
        _baseMaxHealth = baseMaxHealth;
        _basePhysicalArmor = basePhysicalArmor;
        _baseResistance = baseResistance;
        _baseDamage = baseLevel;
        CurrentWeaponId = baseWeaponId;
        _baseCriticalChance = CastingValue.IntToFloat(baseCriticalChance);
        _modifyNextLevel = Mathf.Min(BaseLevel + 1, PLAYER_MAX_LEVEL);
        CurrentHp = _baseMaxHealth;
        CurrentFocus = _baseMaxFocus;
    }

    public void InitPlayerInventory( Weapon weapon = null, 
                                    Armor shield = null, Armor armor = null, Armor helmet = null, Armor boots = null, 
                                    Accessory amulet = null, Accessory ring = null)
    {
        if (BaseWeapon != null) weapon = BaseWeapon;
        
        // 착용 장비 및 소유자 등록
        PlayerInventory = new PlayerInventory(this, weapon, shield, armor, helmet, boots, amulet, ring);

        Equipment[] equipments = Managers.Inventory.SlotEquipmentsItem(PlayerInventory);
        
        foreach (var equipItem in equipments)
        {
            if(equipItem == null) continue;
            
            // 장비착용
            PlayerInventory.SetEquipmentSlotItem(equipItem);
        
            // 아이템에 의해서 플레이어 스텟 변경
            ItemForUpdateModifyStat(equipItem, true);
        }
    }

    #region 최종 연산된 Stat

    public float Quickness => FormatStatValue(PlayerStats.StatType.Quickness, RawQuickness);

    public float Strength => FormatStatValue(PlayerStats.StatType.Strength, RawStrength);

    public float Awareness => FormatStatValue(PlayerStats.StatType.Awareness, RawAwareness);

    public float Intelligence => FormatStatValue(PlayerStats.StatType.Intelligence, RawIntelligence);

    public float Vitality => FormatStatValue(PlayerStats.StatType.Vitality, RawVitality);

    public int PhysicalArmor => FormatDefenseValue(PlayerStats.DefenseType.PhysicalArmor, RawPhysicalArmor);

    public int Resistance => FormatDefenseValue(PlayerStats.DefenseType.Resistance, RawResistance);

    public int Avoid => FormatDefenseValue(PlayerStats.DefenseType.Avoid, RawAvoid);

    public int MaxHeath => Mathf.Clamp(RawMaxHealth, 1, 999);

    /// <summary>
    /// 플레이어 최종 공격력 : 무기 기본 데미지 + 플레이어 기본 데미지 + 버프 데미지(아이템 속성, 버프 등)
    /// </summary>
    public int MaxDamage
    {
        get
        {
            int baseDamage = BaseWeapon.Damage + _baseDamage;
            int modifiedDamage   = (BaseWeapon.Damagetype is Weapon.DamageType.Physical) ? ModifyPhysicalDamage : ModifyMagicDamage;
            return baseDamage + modifiedDamage;
        }
    }

    public int MaxMovements => BaseMovement + ModifyMovement;

    public int MaxFocus => Mathf.Clamp(_baseMaxFocus + _modifyMaxFocus, 1, _MAX_FOCUS_LIMIT);
    
    public float CriticalChance => Mathf.Clamp(RawCriticalChance, 0.05f, 0.95f);
    
    #endregion

    #region 정제되지 않은 Stat

    private float RawQuickness => _baseQuickness + _modifyQuickness;
    
    private float RawStrength => _baseStrength + _modifyStrength;
    
    private float RawAwareness => _baseAwareness + _modifyAwareness;
    
    private float RawIntelligence => _baseIntelligence + _modifyIntelligence;
    
    private float RawVitality => _baseVitality + _modifyVitality;
    
    private int RawPhysicalArmor => _basePhysicalArmor + ModifyPhysicalArmor;
    
    private int RawResistance => _baseResistance + ModifyResistance;

    private int RawAvoid => (int)((Quickness * Quickness - 0.1f) / 3f * 100) + _modifyAvoid;

    private int RawMaxHealth => (_baseMaxHealth + BaseLevel + (int)((0.1) * (double)BaseLevel * Vitality)) + ModifyMaxHealth;

    private float RawCriticalChance => _baseCriticalChance + _modifyCriticalChance;

    #endregion

    private float FormatStatValue(PlayerStats.StatType statType, float rawStatValue)
    {
        /*
        if (_s != FTK_weaponStats2.SkillType.luck)
        {
            _raw *= 1f - GameLogic.Instance.m_PoisonStatPenalty * (float)m_PoisonLvl;
        }
        if (HasCurse(FTKUI.Instance.m_SkillCurseAssociation[_s]))
        {
            _raw *= GameLogic.Instance.m_CurseStatMultiplier;
        }
        */
        return Mathf.Clamp(rawStatValue, 0.25f, 0.95f);
    }

    private int FormatDefenseValue(PlayerStats.DefenseType defenseType, int rawDefenseValue)
    {
        return Mathf.Max(rawDefenseValue, 0);
    }

    /// <summary>
    /// 플레이어 고유 Stat
    /// </summary>
    /// <param name="statType">얻고자하는 플레이어 Stat</param>
    /// <returns>해당 Stat에 0.0 ~ 1.0 값</returns>
    private float GetBaseStatValue(PlayerStats.StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength: return _baseStrength;
            case StatType.Awareness: return _baseAwareness;
            case StatType.Intelligence: return _baseIntelligence;
            case StatType.Quickness: return _baseQuickness;
            case StatType.Vitality: return _baseVitality;
            default: return 0f;
        }
    }
    
    /// <summary>
    /// 최종 플레이어 Stat 
    /// </summary>
    /// <param name="statType">얻고자는 플레어어 Stat</param>
    /// <returns>해당 Stat에 0.25 ~ 0.95 값</returns>
    public float GetStatValue(PlayerStats.StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength: return Strength;
            case StatType.Awareness: return Awareness;
            case StatType.Intelligence: return Intelligence;
            case StatType.Quickness: return Quickness;
            case StatType.Vitality: return Vitality;
            default: return 0f;
        }
    }
    
    /// <summary>
    /// 최종 플레이어 Defense
    /// </summary>
    /// <param name="defenseType">얻고자는 플레이어 Defense</param>
    /// <returns>해당 Defense에 0 ~ N 값</returns>
    public int GetDefenseValue(PlayerStats.DefenseType defenseType)
    {
        switch (defenseType)
        {
            case DefenseType.PhysicalArmor : return PhysicalArmor;
            case DefenseType.Resistance : return Resistance;
            case DefenseType.Avoid : return Avoid;
            default: return 0;
        }
    }



    #region 플레이어 UI 출력함수
    
    /// <summary>
    /// 스텟 인벤토리 Text 값 출력
    /// </summary>
    /// <returns></returns>
    public PlayerMainHudDisplay[] GetInventoryStatUiDisplay()
    {
        var defaultTextColor = new Color(240f / 255f, 233f / 255f, 226f / 255f);
        PlayerMainHudDisplay[] statUiDisplay = new PlayerMainHudDisplay[Enum.GetNames(typeof(UI_Stats.Texts)).Length];

        statUiDisplay[(int)UI_Stats.Texts.ClassNameText].Text = BaseClassName;
        statUiDisplay[(int)UI_Stats.Texts.ClassNameText].Color = defaultTextColor;
        
        /* 무기 */
        
        statUiDisplay[(int)UI_Stats.Texts.MaxDamageText].Text = MaxDamage.ToString();
        statUiDisplay[(int)UI_Stats.Texts.MaxDamageText].Color = BaseWeapon.Damagetype is Weapon.DamageType.Physical
            ? UI_PlayerMainHUD.PHYSICAL_DAMAGE_TYPE_COLOR
            : UI_PlayerMainHUD.MAGICAL_DAMAGE_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.BaseAttackDamageText].Text = (BaseWeapon.Damage + _baseDamage).ToString();
        statUiDisplay[(int)UI_Stats.Texts.BaseAttackDamageText].Color = statUiDisplay[(int)UI_Stats.Texts.MaxDamageText].Color;

        statUiDisplay[(int)UI_Stats.Texts.CritChanceText].Text = (CriticalChance * 100) + "%";
        statUiDisplay[(int)UI_Stats.Texts.CritChanceText].Color = defaultTextColor;
        
        statUiDisplay[(int)UI_Stats.Texts.ModifyPhysicalDamageText].Text = ModifyPhysicalDamage.ToString();
        statUiDisplay[(int)UI_Stats.Texts.ModifyPhysicalDamageText].Color = UI_PlayerMainHUD.PHYSICAL_DAMAGE_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.ModifyMagicDamageText].Text = ModifyMagicDamage.ToString();
        statUiDisplay[(int)UI_Stats.Texts.ModifyMagicDamageText].Color = UI_PlayerMainHUD.MAGICAL_DAMAGE_TYPE_COLOR;;
        
        /* 방어력 */

        statUiDisplay[(int)UI_Stats.Texts.MaxPhysicalArmorText].Text = PhysicalArmor.ToString();
        statUiDisplay[(int)UI_Stats.Texts.MaxPhysicalArmorText].Color = UI_PlayerMainHUD.PHYSICAL_ARMOR_TYPE_COLOR;

        statUiDisplay[(int)UI_Stats.Texts.MaxResistanceText].Text = Resistance.ToString();
        statUiDisplay[(int)UI_Stats.Texts.MaxResistanceText].Color = UI_PlayerMainHUD.RESISTANCE_ARMOR_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.BasePhysicalArmorText].Text = _basePhysicalArmor.ToString();
        statUiDisplay[(int)UI_Stats.Texts.BasePhysicalArmorText].Color = UI_PlayerMainHUD.PHYSICAL_ARMOR_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.BaseResistanceText].Text = _baseResistance.ToString();
        statUiDisplay[(int)UI_Stats.Texts.BaseResistanceText].Color = UI_PlayerMainHUD.RESISTANCE_ARMOR_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.MaxEvadeText].Text = Avoid.ToString();
        statUiDisplay[(int)UI_Stats.Texts.MaxEvadeText].Color = UI_PlayerMainHUD.EVADE_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.ModifyPhysicalArmorText].Text = ModifyPhysicalArmor.ToString();
        statUiDisplay[(int)UI_Stats.Texts.ModifyPhysicalArmorText].Color = UI_PlayerMainHUD.PHYSICAL_ARMOR_TYPE_COLOR;;
        
        statUiDisplay[(int)UI_Stats.Texts.ModifyResistanceText].Text = ModifyResistance.ToString();
        statUiDisplay[(int)UI_Stats.Texts.ModifyResistanceText].Color = UI_PlayerMainHUD.RESISTANCE_ARMOR_TYPE_COLOR;;
        
        /* 기타 능력치 */
        
        statUiDisplay[(int)UI_Stats.Texts.CurrentLevelText].Text = BaseLevel.ToString();
        statUiDisplay[(int)UI_Stats.Texts.CurrentLevelText].Color = UI_PlayerMainHUD.LEVEL_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.CurrentXpText].Text = GetXpDisplayString();
        statUiDisplay[(int)UI_Stats.Texts.CurrentXpText].Color = defaultTextColor;

        statUiDisplay[(int)UI_Stats.Texts.XpMultiText].Text = ModifyXpMulti + "%";
        statUiDisplay[(int)UI_Stats.Texts.XpMultiText].Color = defaultTextColor;
        
        statUiDisplay[(int)UI_Stats.Texts.CurrentGoldText].Text = CurrentGold.ToString();
        statUiDisplay[(int)UI_Stats.Texts.CurrentGoldText].Color = UI_PlayerMainHUD.GOLD_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.GoldMultiText].Text = ModifyGoldMulti + "%";
        statUiDisplay[(int)UI_Stats.Texts.GoldMultiText].Color = defaultTextColor;
        
        statUiDisplay[(int)UI_Stats.Texts.CurrentMaxMovementText].Text = MaxMovements.ToString();
        statUiDisplay[(int)UI_Stats.Texts.CurrentMaxMovementText].Color = defaultTextColor;
        
        statUiDisplay[(int)UI_Stats.Texts.ModifyMovementText].Text = ModifyMovement.ToString();
        statUiDisplay[(int)UI_Stats.Texts.ModifyMovementText].Color = defaultTextColor;

        /* 체력 */
        
        statUiDisplay[(int)UI_Stats.Texts.MaxHpText].Text = CurrentHp + " / " + MaxHeath;
        statUiDisplay[(int)UI_Stats.Texts.MaxHpText].Color = UI_PlayerMainHUD.HP_TYPE_COLOR;

        statUiDisplay[(int)UI_Stats.Texts.BaseMaxHpText].Text = (RawMaxHealth - ModifyMaxHealth).ToString();
        statUiDisplay[(int)UI_Stats.Texts.BaseMaxHpText].Color = UI_PlayerMainHUD.HP_TYPE_COLOR;
        
        statUiDisplay[(int)UI_Stats.Texts.HpRegenText].Text = ModifyHpRegen.ToString();
        statUiDisplay[(int)UI_Stats.Texts.HpRegenText].Color = defaultTextColor;

        statUiDisplay[(int)UI_Stats.Texts.MaxFocusText].Text = GetFocusDisplayString();
        statUiDisplay[(int)UI_Stats.Texts.MaxFocusText].Color = defaultTextColor;
        
        statUiDisplay[(int)UI_Stats.Texts.ModifyMaxHpText].Text = ModifyMaxHealth.ToString();
        statUiDisplay[(int)UI_Stats.Texts.ModifyMaxHpText].Color = UI_PlayerMainHUD.HP_TYPE_COLOR;
        
        return statUiDisplay;

        //25 개..

    }

    /// <summary>
    /// 플레이어 Stat HUD 함수
    /// </summary>
    /// <param name="statType">얻고자는 Stat</param>
    /// <returns>PlayerMainHudDisplay 구조체 (Text, Color)</returns>
    public PlayerMainHudDisplay GetStatDisplay(PlayerStats.StatType statType)
    {
        PlayerMainHudDisplay result;
        // 실수형 -> 정수형 -> 문자열로 변환  (플레이어 HUD에 표시)
        result.Text = (GetStatValue(statType) * 100f).ToString();

        // 현재 스탯이 Max 값인 경우 금색
        if (GetStatValue(statType) >= 0.95f)
        {
            result.Color = new Color(255/255f, 223/255f, 0/255f);
            return result;
        }
        
        Color textColorStyle;
        if (GetStatValue(statType) > GetBaseStatValue(statType))         // 현재 스탯 버프 받는중 (초록색)
        {
            textColorStyle = new Color(41/255f, 207/255f, 25/255f);
        }
        else if (GetStatValue(statType) < GetBaseStatValue(statType))   //  현재 스탯 디버프 받는중 (붉은색)
        {
            textColorStyle = new Color(224/255f, 60/255f, 60/255f);
        }
        else                                                            // 현재 스탯이 기본 Base 스탯과 같은 경우
        {
            textColorStyle = Color.white;
        }
        
        result.Color = textColorStyle;
        return result;
    }

    /// <summary>
    /// 플레이어 Defense HUD 함수
    /// </summary>
    /// <param name="defenseType">얻고자는 Defense</param>
    /// <returns>해당 Defense 값 문자열</returns>
    public string GetDefenseDisplay(PlayerStats.DefenseType defenseType)
    {
        return GetDefenseValue(defenseType).ToString();
    }
    
    public PlayerMainHudDisplay GetDamageDisplay()
    {
        PlayerMainHudDisplay result;
        
        result.Color = BaseWeapon.Damagetype is Weapon.DamageType.Physical ? UI_PlayerMainHUD.PHYSICAL_DAMAGE_TYPE_COLOR : UI_PlayerMainHUD
            .MAGICAL_DAMAGE_TYPE_COLOR;
        result.Text = MaxDamage.ToString();
        
       // Debug.Log($"{BaseClassName}의 공격력 : {MaxDamage}, {BaseWeapon.Damage}, {_baseDamage}, {_modifyPhysicalDamage}, {_modifyMagicDamage}");
        
        return result;
    }

    public string GetHealthDisplayString()
    {
        return CurrentHp + " / " + MaxHeath;
    }

    public string GetXpDisplayString()
    {
         return CurrentXp + " / " + MaxXp;
    }
    
    /// <summary>
    /// 경험치 퍼센트 계산 (플레이어의 경험치가 현재 레벨에서 다음 레벨까지의 경험치 범위 중 얼마나 진행되었는지를 나타냄)
    /// </summary>
    public float GetXpPercent()
    {
        int lastMaxXpValue = 0;
        int currentMaxXpValue = 0;
        int resultMaxXpValue = 0;
        
        // 레벨이 0보다 큰 경우
        if (BaseLevel > 0)
        {
            lastMaxXpValue = _PLAYER_LEVEL_TABLE[BaseLevel];
            currentMaxXpValue = _PLAYER_LEVEL_TABLE[_modifyNextLevel] - _PLAYER_LEVEL_TABLE[BaseLevel];
        }
        else
        {
            currentMaxXpValue = _PLAYER_LEVEL_TABLE[_modifyNextLevel];
        }

        // NaN값 예외처리
        if (CurrentXp == 0) return 0f;
        
        resultMaxXpValue = CurrentXp - lastMaxXpValue;
        
        return (float)resultMaxXpValue / (float)currentMaxXpValue;
    }

    public string GetFocusDisplayString()
    {
        return CurrentFocus + " / " + MaxFocus;
    }
    
    #endregion

    #region 플레이어 업데이트 함수

    private void UpdatePlayerCharacterDummy(Equipment item, bool isEquip)
    {
        WorldMapCharacterDummy.UpdateCharacterItemPart(item, isEquip);
       // BattleMapCharacterDummy.UpdateCharacterItemPart(item, isEquip);
    }

    /// <summary>
    /// 아이템을 통해서 플레이어 스텟 변경 시
    /// </summary>
    /// <param name="item">사용한 아이템</param>
    /// <param name="isEquip">아이템 착용 여부</param>
    public void ItemForUpdateModifyStat(Item item, bool isEquip)
    {
        // 착용 장비
        if (item is Equipment equipment)
        {
            // 물리 방어력 스텟
            if (equipment.ModifyEquipmentPhysicalArmorBonus != 0)
            {
                UpdateModifyStat(ModifyStatType.ModifyPhysicalArmor, equipment.ModifyEquipmentPhysicalArmorBonus, isEquip);
            }
            // 마법 방어력 스텟
            if (equipment.ModifyEquipmentResistanceArmorBonus != 0)
            {
                UpdateModifyStat(ModifyStatType.ModifyResistance, equipment.ModifyEquipmentPenaltyStatBonus, isEquip);
            }
            // 증가 스텟
            if (equipment.ModifyEquipmentExtraStatBonus != 0)
            {
                UpdateModifyStat(_STAT_CONVERSION[equipment.EquipmentExtraStatType], equipment.ModifyEquipmentExtraStatBonus, isEquip);
            }
            // 감소 스텟
            if (equipment.ModifyEquipmentPenaltyStatBonus != 0)
            {
                UpdateModifyStat(_STAT_CONVERSION[equipment.EquipmentPenaltyStatType], equipment.ModifyEquipmentExtraStatBonus, !isEquip);
            }

            // 무기인 경우
            if (equipment is Weapon weapon)
            {
                // 장착한 무기 정보 변경
                if (isEquip)  BaseWeapon = weapon;
                else  BaseWeapon = (Weapon)Managers.Data.GetItemInfo(Item.ItemType.Weapon, 0); // 무기 해체 시 주먹으로 변경
   
                CurrentWeaponId = BaseWeapon.ItemID;

                // 무기 데미지 Text UI 업데이트
                Managers.UIManager.GetPlayerHUD(this).UpdatePlayerMainHudTexts(UI_PlayerMainHUD.TextType.AttackDamage);
            }
            
            // 월드, 인벤토리 Dummy 모델링 업데이트
            // 전투 Dummy 모델링 업데이트
            UpdatePlayerCharacterDummy(equipment, isEquip);

        }
        // 소모품
        else if (item is Consumable consumable)
        {
            
        }
    }

    /// <summary>
    /// 성소 효과 업데이트
    /// </summary>
    /// <param name="sanctuaryType">성소 타입</param>
    /// <param name="modifyStatType">효과 받을 스텟 타입</param>
    /// <param name="buffValue">버프량</param>
    /// <param name="isAdd">추가 또는 삭제여부</param>
    public void UpdateSanctuaryBuff(Sanctuary.SanctuaryType sanctuaryType, ModifyStatType modifyStatType, int buffValue, bool isAdd)
    {
        // 이미 플레이어가 성소 버프를 소지하고 있다면 생략
        if(CurrentSanctuaryType != Sanctuary.SanctuaryType.None && sanctuaryType != Sanctuary.SanctuaryType.None) return;

        CurrentSanctuaryType = sanctuaryType;
        UpdateModifyStat(modifyStatType, buffValue, isAdd);
        
        Debug.Log($"{sanctuaryType}획득으로 {modifyStatType}능력치 {buffValue} 값 증가");
      
        // TODO : 성소 타입에 맞는 이미지 Player HUD 적용
        // TODO : 성소 타입에 맞는 이미지 인벤토리 HUD 적용
    }


    public void UpdateModifyStat(ModifyStatType modifyStatType, int value, bool isAdd = true)
    {
    
        if (!isAdd) value *= -1;

        UI_PlayerMainHUD.TextType updateTextUI = UI_PlayerMainHUD.TextType.Stat;
        
        switch (modifyStatType)
        {
            case ModifyStatType.ModifyStrength :
                _modifyStrength += CastingValue.IntToFloat(value);
                break;
            
            case ModifyStatType.ModifyAwareness :
                _modifyAwareness += CastingValue.IntToFloat(value);
                break;
            
            case ModifyStatType.ModifyIntelligence :
                _modifyIntelligence += CastingValue.IntToFloat(value);
                break;
            
            case ModifyStatType.ModifyVitality :
                _modifyVitality += CastingValue.IntToFloat(value);
                break;
            
            case ModifyStatType.ModifyQuickness :
                _modifyQuickness += CastingValue.IntToFloat(value);
                break;
            
            case ModifyStatType.ModifyPhysicalArmor :
                updateTextUI = UI_PlayerMainHUD.TextType.Defense;
                ModifyPhysicalArmor += value;
                break;
            
            case ModifyStatType.ModifyResistance :
                updateTextUI = UI_PlayerMainHUD.TextType.Defense;
                ModifyResistance += value;
                break;
            
            case ModifyStatType.ModifyAvoid :
                updateTextUI = UI_PlayerMainHUD.TextType.Defense;
                _modifyAvoid += value;
                break;
            
            case ModifyStatType.ModifyMaxHealth :
                ModifyMaxHealth += value;
                Managers.UIManager.GetPlayerHUD(this).UpdateHealthDisplay(CurrentHp, MaxHeath);
                return;
            
            case ModifyStatType.ModifyMaxFocus :
                _modifyMaxFocus += value;
                Managers.UIManager.GetPlayerHUD(this).UpdateFocusDisplay(true);
                return;
            
            case ModifyStatType.ModifyDamage :
                updateTextUI = UI_PlayerMainHUD.TextType.AttackDamage;
                _modifyDamageMulti += value;
                break;
            
            case ModifyStatType.ModifyPhysicalDamage :
                updateTextUI = UI_PlayerMainHUD.TextType.AttackDamage;
                ModifyPhysicalDamage += value;
                break;
            
            case ModifyStatType.ModifyMagicDamage :
                updateTextUI = UI_PlayerMainHUD.TextType.AttackDamage;
                ModifyMagicDamage += value;
                break;
            
            case ModifyStatType.ModifyCriticalChance :
                _modifyCriticalChance += CastingValue.IntToFloat(value);
                break;
        }
        
        Managers.UIManager.GetPlayerHUD(this).UpdatePlayerMainHudTexts(updateTextUI);
    }

    // TODO:레벨업하면 basedamage 같이 올려주기
    public void UpdateXp(int xp, bool isAdd =true)
    {
        if (xp == 0)  return;

        if (!isAdd)  xp *= -1;

        CurrentXp += xp;
        CurrentXp = Mathf.Clamp(CurrentXp, 0, _PLAYER_LEVEL_TABLE[PLAYER_MAX_LEVEL]);

        // 레벨 업
        if (CurrentXp >= _PLAYER_LEVEL_TABLE[_modifyNextLevel])
        {
            UpdateLevel();
        }

        // 현재 경험치 퍼센트 계산
        float currentXpPercent = 0f;
        currentXpPercent = BaseLevel < PLAYER_MAX_LEVEL ? GetXpPercent() : 1f;
        
        // 경험치 Slider 업데이트
        Managers.UIManager.GetPlayerHUD(this).UpdateXpDisplay(currentXpPercent);
    }


    public void UpdateGold(int gold, bool isAdd = true)
    {
        if (gold == 0) return;

        string operand = "+";

        if (!isAdd)
        {
            operand = "-";
            gold *= -1;
        }

        CurrentGold += gold;
        CurrentGold = Mathf.Clamp(CurrentGold, 0, 10000);  // Mathf.Clamp(CurrentGold, 0, GameLogic.Instance.MAX_GOLD_VALUE); => (디버깅 모드가 아닌경우)

        // 현재 소지 골드 Text 업데이트
        Managers.UIManager.GetPlayerHUD(this).UpdatePlayerMainHudTexts(UI_PlayerMainHUD.TextType.Gold);
        
        // 골드획득 UI 연출
        Managers.UIManager.GetPlayerHUD(this).UpdateFloatingText(operand + gold + "Gold");
    }
    
    public void UpdateItem(Item item, bool isAdd = true)
    {
        Managers.UIManager.GetPlayerHUD(this).UpdateFloatingText("+" + item.ItemName);
    }

    /// <summary>
    /// 플레이어 레벨 증가 (최대체력, 현재체력, 현재 집중력 변경)
    /// </summary>
    private void UpdateLevel()
    {
        // 레벨업을 있으니 현재 체력을 최대 체력으로 증가함
        CurrentHp = MaxHeath;
        
        // 현재 플레이어 레벨 증가
        BaseLevel = Mathf.Min(BaseLevel + 1, PLAYER_MAX_LEVEL);
        Managers.UIManager.GetPlayerHUD(this).UpdatePlayerMainHudTexts(UI_PlayerMainHUD.TextType.Level);

        // 다음 목표 레벨 증가
        if (BaseLevel != PLAYER_MAX_LEVEL) _modifyNextLevel++;
        
        // TODO 집중력 회복
        UpdateCurrentFocus(_PLAYER_LEVEL_UP_FOCUS_GAIN);
        

        /*
    int num = 0;
    for (int i = 0; i < GameFlow.Instance.m_LevelXpValues.Length; i++)
    {
        if (m_PlayerXP < GameFlow.Instance.m_LevelXpValues[i])
        {
            num = i;
            break;
        }
    }
    if (m_PlayerLevel < num)
    {
        m_PlayerLevel = num;
        global::StatsAchievements.StatsAchievements.TryPlayerStatisticSetValue(FTK_statistic.ID.STAT_CHARACTER_LEVEL, m_PlayerLevel);
        TallyCharacterDefense();
        m_CharacterOverworld.m_UIPlayMainHud.UpdateHud();
        int num2 = MaxHealth - m_HealthCurrent;
        TallyCharacterHealth(m_PlayerLevel, false);
        m_HealthCurrent = MaxHealth - FTKUtil.RoundToInt((float)num2 * GameFlow.Instance.GameDif.m_LevelUpHealthDifference);
        TallyCharacterHealth(m_PlayerLevel);
        AudioManager.Instance.AudioEvent("Play_gui_stinger_level_up");
        m_CharacterOverworld.m_OverworldDummyFX.LevelUpFxOn();
        string txt = FTKHub.Localized<TextMisc>("STR_HudLevelUp");
        if (m_IsInCombat)
        {
            m_CharacterOverworld.m_CurrentDummy.m_DummyFX.LevelUpFxOn();
            m_CharacterOverworld.m_CurrentDummy.SpawnHudTextRPC(txt, string.Empty);
        }
        else
        {
            m_CharacterOverworld.SpawnHudTextRPC(txt, string.Empty);
        }
        if (base.IsOwner)
        {
            UpdateFocusPoints(GameFlow.Instance.GameDif.m_LevelUpFocusGain);
        }
    }
    if (m_CharacterOverworld.m_PhotonView.isMine)
    {
        SyncMember("m_PlayerXP");
    }
    m_CharacterOverworld.m_UIPlayMainHud.UpdateHud()
    */
    }
    
    /// <summary>
    /// 플레이어 현재 체력 업데이트
    /// </summary>
    /// <param name="newHp">최종적으로 갱신된 체력</param>
    public void UpdateCurrentHealth(int newHp)
    {
        CurrentHp = Mathf.Clamp(newHp, 0, MaxHeath);
        Managers.UIManager.GetPlayerHUD(this).UpdateHealthDisplay(CurrentHp, MaxHeath);
    }

    public void UpdateCurrentFocus(int newFocus, bool isAdd = true)
    {
        if (newFocus == 0)  return;

        if (!isAdd)
        {
            TempUsedFocus -= newFocus;
            newFocus *= -1;
        }
        else
        {
            TempUsedFocus += newFocus;
        }

        CurrentFocus = Mathf.Clamp(CurrentFocus + newFocus, 0, MaxFocus);
        Managers.UIManager.GetPlayerHUD(this).UpdateFocusDisplay(false, CurrentFocus);
    }

    public void RefundFocus()
    {
        CurrentFocus = Mathf.Clamp(CurrentFocus - TempUsedFocus, 0, MaxFocus);
        TempUsedFocus = 0;
        Managers.UIManager.GetPlayerHUD(this).UpdateFocusDisplay(false, CurrentFocus);
    }

    #endregion


    public object Clone()
    {
        PlayerStats copyPlayerStats = (PlayerStats)MemberwiseClone();
        copyPlayerStats.BaseWeapon = (Weapon)BaseWeapon.Clone();
        return MemberwiseClone();
    }
}
