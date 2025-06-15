using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_ItemCard : UI_Base
{
    /*  <아이템 카드>
     *  1. 플레이어 장착 아이템 카드, 메인 아이템 카드
     *  2. 공통점
     *     2-1. 아이템 이름
     *     2-2. 아이템 등급
     *     2-3. 아이템 설명
     *       2-3-1. 무기 (무기 데미지, 무기 데미지 타입, 무기 옵션 설명, 무기 요구 스텟 아이콘, 무기 스킬 아이콘, 무기 디테일 아이콘, 무기 스토리)
     *       2-3-2. 방어구 (방어구 옵션 설명, 방어구 스토리)
     *       2-3-3. 소모품 (소모품 옵션 설명, 소모품 스토리)
     *     2-4. 아이템 초상화 아이콘
     *  3. 차이점
     *    3-1. 메인 아이템 카드는 아이템 3D 초상화가 존재
     *    3-2. 플레이어 장착 아이템 카드는 3D 초상화가 존재 하지 않음
     *    3-3. 메인 아이템 카드는 Item Stock 텍스트가 존재 (ex : 재화 개수)
     */

    protected enum Images
    {
        // 무기 요구 스텟 아이콘
        WeaponBaseStatIcon_0 = 0,
        WeaponBaseStatIcon_1 = 1,
        WeaponBaseStatIcon_2 = 2,
        WeaponBaseStatIcon_3 = 3,
        WeaponBaseStatIcon_4 = 4,
        WeaponBaseStatIcon_5 = 5,
        
        // 무기 등급에 맞는 배경 색상
        ItemCardBackGround,
        ItemImageBackGround,
        ItemTitleBackGround,
        ItemCardDividerBottomBackGround,
        ItemCardDividerTopBackGround,
        
        // 무기 아이콘
        ItemIcon,
    }
    
    protected enum Texts
    {
        ItemTitleText,
        ItemRarityText,

        /* 무기 파트 */
        WeaponDamageText,
        WeaponDamageTypeText,
        WeaponDetailStatsText,
        
        /* 방어구 파트 */
        EquippableDetailStatsText,
        
        /* 소모품 파트 */
        ConsumableItemDetailText,
    }

    protected enum GameObjects
    {
        // 무기 설명
        WeaponDetail,
        // 방어구 설명
        EquippableDetail,
        // 소모품 설명
        ConsumableItemDetail,

        // 무기 특징 아이콘
        WeaponTwoHandedIcon,
        WeaponNonFocusableIcon,
        WeaponBreakableIcon,
        
        // 무기 스킬 특징 아이콘
        WeaponSingleTargetIcon,
        WeaponSplashTargetIcon,
        WeaponAoeTargetIcon,
        WeaponPierceTargetIcon,
        WeaponHeavyTargetIcon,
    }

    // 무기 등급에 맞는 색상 코드
    private static  Dictionary<Item.Grade, string> _ITEM_GRADE_COLOR = new Dictionary<Item.Grade, string>()
    {
        { Item.Grade.COMMON,  "#FFFFFF" },     // White
        { Item.Grade.ADVANCED, "#52A859"  },   // Green
        { Item.Grade.RARE, "#0096FF" },        // Blue
        { Item.Grade.HERO, "#B54CFF" },        // Purple
        { Item.Grade.LEGEND, "#FF8C00"  },      // Gold
        { Item.Grade.MYTH, "#C90000" },         // 빨간색
        { Item.Grade.QUEST, "#FFBF00"}         // 노랑색
    };
    
    // 무기 등급에 맞는 텍스트
    private static  Dictionary<Item.Grade, string> _ITEM_GRADE_TEXT = new Dictionary<Item.Grade, string>()
    {
        { Item.Grade.COMMON,  "일반" },     // White
        { Item.Grade.ADVANCED, "고급"  },   // Green
        { Item.Grade.RARE, "희귀" },        // Blue
        { Item.Grade.HERO, "영웅" },        // Purple
        { Item.Grade.LEGEND, "전설"  },      // Gold
        { Item.Grade.MYTH, "신화" },         // 빨간색
        { Item.Grade.QUEST, "퀘스트"}         // 노랑색
    };
    
    // 방어구, 무기에 능력치 텍스트
    private static Dictionary<Equipment.EquipmentStat, string> _STAT_FORMAT_TEXT = new Dictionary<Equipment.EquipmentStat, string>()
    {
        { Equipment.EquipmentStat.Strength, "힘"},  
        { Equipment.EquipmentStat.Awareness, "인지"  },  
        { Equipment.EquipmentStat.Intelligence, "지력" },  
        { Equipment.EquipmentStat.Vitality, "활력"}  ,  
        { Equipment.EquipmentStat.Quickness, "속도"}  ,
        { Equipment.EquipmentStat.Avoid,  "회피력" },
        { Equipment.EquipmentStat.PhysicalDamage, "물리 공격력" },        
        { Equipment.EquipmentStat.MagicalDamage, "마법 공격력"  },     
        { Equipment.EquipmentStat.MaxFocus, "최대 집중력" },   
        { Equipment.EquipmentStat.PhysicalArmor, "물리 방어력" }, 
        { Equipment.EquipmentStat.Resistance, "마법 방어력" }
    };

    // 긍정적 효과 텍스트 색상
    private static string _BUFF_TEXT_COLOR = "#86B0F4"; // Blue
    
    // 부정적 효과 텍스트 색상
    private static string _DEBUFF_TEXT_COLOR = "#FF4752"; // Red
    
    // 아이템 스텟설명 텍스트
    private StringBuilder detailBaiscStatsTextFormat = new StringBuilder();
    
    public Item Item { get; private set; }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
    }

    public void SetItemInfo(Item item, bool isRewardItem)
    {
        this.Item = item;
        
        // 무기 등급에 맞는 배경 이미지 색상 (16진수 -> RGBA 코드로 변환)
        ColorUtility.TryParseHtmlString(_ITEM_GRADE_COLOR[item.ItemGrade], out Color itemGradeColor);

        // 아이템 스텟 설명 텍스트 초기화
        detailBaiscStatsTextFormat.Clear();
        
        // 아이템 이름 설정
        Get<TextMeshProUGUI>((int)Texts.ItemTitleText).text = Item.ItemName;
        
        // 아이템 등급 텍스트 설정
        Get<TextMeshProUGUI>((int)Texts.ItemRarityText).text = _ITEM_GRADE_TEXT[Item.ItemGrade];
        
        // 아이템 등급 배경색상 설정
        Get<Image>((int)Images.ItemCardBackGround).color = itemGradeColor;
        Get<Image>((int)Images.ItemImageBackGround).color = itemGradeColor;
        Get<Image>((int)Images.ItemTitleBackGround).color = itemGradeColor;
        Get<Image>((int)Images.ItemCardDividerBottomBackGround).color = itemGradeColor;
        Get<Image>((int)Images.ItemCardDividerTopBackGround).color = itemGradeColor;
        Get<TextMeshProUGUI>((int)Texts.ItemRarityText).color = itemGradeColor;


        Sprite itemIcon = null;
        
        switch (item.Itemtype)
        {
            case Item.ItemType.Weapon : 
                
                string detailStatsTextFormat = "<color=#FFDF9A>";
                
                // 무기가 요구하는 스텟 아이콘 초기화
                Get<Image>((int)Images.WeaponBaseStatIcon_0).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_1).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_2).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_3).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_4).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_5).gameObject.SetActive(false);
                
                // 무기 타입 설명창만 활성화
                Get<GameObject>((int)GameObjects.EquippableDetail).SetActive(false);
                Get<GameObject>((int)GameObjects.ConsumableItemDetail).SetActive(false);

                // 무기 스킬 아이콘 초기화
                Get<GameObject>((int)GameObjects.WeaponSingleTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponSplashTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponAoeTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponPierceTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponHeavyTargetIcon).SetActive(false);
                
                // 무기 특별 조건 아이콘 초기화
                Get<GameObject>((int)GameObjects.WeaponTwoHandedIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponNonFocusableIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponBreakableIcon).SetActive(false);
                
                Weapon weapon = (Weapon)item;
                itemIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    weapon.Weapontype.ToString(), true);
                
                List<int> weaponSkillIndex = weapon.Skills;
                
                for (int i = 0; i < weaponSkillIndex.Count; i++)
                {
                    Skill skill = Managers.Data.GetUsedSkillInfo(weaponSkillIndex[i]);

                    // 스킬 이름 텍스트
                    detailStatsTextFormat += skill.Name;
                                       
                    if (i < weaponSkillIndex.Count - 1)
                    {
                        detailStatsTextFormat += ", ";
                    }

                    // 스킬 초상화 아이콘
                    switch (skill.Target)
                    {
                        case (int)Skill.Targets.SingleTarget:
                            Get<GameObject>((int)GameObjects.WeaponSingleTargetIcon).SetActive(true);
                            break;
                        
                        case (int)Skill.Targets.TargetGroup:
                            Get<GameObject>((int)GameObjects.WeaponAoeTargetIcon).SetActive(true);
                            break;
                        
                        case (int)Skill.Targets.Splash:
                            Get<GameObject>((int)GameObjects.WeaponSplashTargetIcon).SetActive(true);
                            break;
                    }
 
                }

                // 해당 무기가 요구하는 스텟 아이콘 이미지
                Sprite skillStatIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    weapon.WeaponBasestat.ToString()); 
                
                //  무기가 요구하는 슬롯 스텟 아이콘 개수만큼 활성화 및 스텟 아이콘 설정
                for (int i = 0; i < Managers.Data.GetUsedSkillInfo(weaponSkillIndex[0]).Rolls; i++)
                {
                    Get<Image>(i).gameObject.SetActive(true);
                    Get<Image>(i).sprite = skillStatIcon;
                }
                
                // 무기가 양손 무기인 경우 양손 무기 아이콘 활성화
                Get<GameObject>((int)GameObjects.WeaponTwoHandedIcon).SetActive(weapon.WeaponHandlegrip is Weapon.WeaponHandleGrip.TwoHand);
                
                // 무기 데미지 텍스트 및 색상 변경
                Get<TextMeshProUGUI>((int)Texts.WeaponDamageText).text = weapon.Damage.ToString();
                Get<TextMeshProUGUI>((int)Texts.WeaponDamageText).color = Get<TextMeshProUGUI>((int)Texts.WeaponDamageTypeText).color = weapon.Damagetype is Weapon.DamageType.Physical
                    ? UI_PlayerMainHUD.PHYSICAL_DAMAGE_TYPE_COLOR
                    : UI_PlayerMainHUD.MAGICAL_DAMAGE_TYPE_COLOR;

                Get<TextMeshProUGUI>((int)Texts.WeaponDamageTypeText).text =
                    weapon.Damagetype is Weapon.DamageType.Physical ? "물리 공격력" : "마법 공격력";
                
                detailStatsTextFormat += "</color>\n";

                // 무기 상세 설명 텍스트 최종 설정
                Get<TextMeshProUGUI>((int)Texts.WeaponDetailStatsText).text = detailStatsTextFormat;
                
                Get<GameObject>((int)GameObjects.WeaponDetail).SetActive(true);

                break;
            
            case Item.ItemType.Armor :  
                // 방어구 타입 설명창만 활성화
                Get<GameObject>((int)GameObjects.WeaponDetail).SetActive(false);
                Get<GameObject>((int)GameObjects.ConsumableItemDetail).SetActive(false);
                
                Armor armor = (Armor)item;  
                
                itemIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    armor.Armortype.ToString(), true);

               

                /*
                  string detailArmorStatsTextFormat = string.Empty;
                 
                // 방어력 능력치 텍스트
                
                if (armor.ModifyEquipmentPhysicalArmorBonus > 0)
                {
                    detailArmorStatsTextFormat += "<color"+_BUFF_TEXT_COLOR+">" + "+" + armor.ModifyEquipmentPhysicalArmorBonus + " " + _STAT_TEXT[Equipment.EquipmentStat.PhysicalArmor] + "</color>\n";
                }
                else if (armor.ModifyEquipmentPhysicalArmorBonus < 0)
                {
                    detailArmorStatsTextFormat += "<color"+_DEBUFF_TEXT_COLOR+">" + "-" + armor.ModifyEquipmentPhysicalArmorBonus + " " + _STAT_TEXT[Equipment.EquipmentStat.PhysicalArmor]+ "</color>\n";
                }
                
                if (armor.ModifyEquipmentResistanceArmorBonus > 0)
                {
                    detailArmorStatsTextFormat += "<color"+_BUFF_TEXT_COLOR+">" + "+" + armor.ModifyEquipmentResistanceArmorBonus + " " + _STAT_TEXT[Equipment.EquipmentStat.Resistance]+ "</color>\n";
                }
                else if (armor.ModifyEquipmentResistanceArmorBonus < 0)
                {
                    detailArmorStatsTextFormat += "<color"+_DEBUFF_TEXT_COLOR+">" + "-" + armor.ModifyEquipmentResistanceArmorBonus + " " + _STAT_TEXT[Equipment.EquipmentStat.Resistance]+ "</color>\n";
                }

                // 스텟 능력치 텍스트
                
                if (armor.ModifyEquipmentExtraStatBonus > 0)
                {
                    detailArmorStatsTextFormat += "<color"+_BUFF_TEXT_COLOR+">" + "+" + armor.ModifyEquipmentExtraStatBonus + " " + _STAT_TEXT[armor.EquipmentExtraStatType]+ "</color>\n";
                }

                if (armor.ModifyEquipmentPenaltyStatBonus > 0)
                {
                    detailArmorStatsTextFormat += "<color"+_DEBUFF_TEXT_COLOR+">" + "+" + armor.ModifyEquipmentPenaltyStatBonus + " " + _STAT_TEXT[armor.EquipmentPenaltyStatType]+ "</color>\n";
                }
                */

                FormatArmorStats(armor);
                
                
                // 방어구 상세 설명 텍스트 최종 설정
                Get<TextMeshProUGUI>((int)Texts.EquippableDetailStatsText).text = detailBaiscStatsTextFormat.ToString();
                
                Get<GameObject>((int)GameObjects.EquippableDetail).SetActive(true);
                
                
                break;
            case Item.ItemType.Consumable or Item.ItemType.GoldCoin :  
                // 소모품 아이템 타입 설명창만 활성화
                Get<GameObject>((int)GameObjects.WeaponDetail).SetActive(false);
                Get<GameObject>((int)GameObjects.EquippableDetail).SetActive(false);

                itemIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    Managers.Data.GetItemName(Item.Itemtype is Item.ItemType.Consumable ? Item.ItemType.Consumable : Item.ItemType.GoldCoin, Item.ItemID), true);
                
                Get<TextMeshProUGUI>((int)Texts.ConsumableItemDetailText).text = Item.ItemDescription;
                
                break;
            case Item.ItemType.Accessory : Item = (Accessory)item;  break;
        }
        
        
        // 아이템 초상화 아이콘 설정
        Get<Image>((int)Images.ItemIcon).sprite = itemIcon;
        
        /*
            <color=#FFDF9A>Molto Crescendo, Molto Rubato, Animato Assai</color>
            <color=#86B0F4><color=#FFFFFF>+50% Damage Against Dark</color>
            <color=#FFFFFF>+1 Health Regeneration</color>
            <color=#C4B6A6><color=#C4B6A6>Skill: Party Heal</color></color>
            <color=#C4B6A6>Support Combat Range</color></color>
         */
        SetItemInfo(isRewardItem);
    }
    
  
    /// <summary>
    /// 방어구, 무기에 대한 상세 스텟 설명 텍스트 설정
    /// </summary>
    /// <param name="equipmentItem">설명할 아이템</param>
    private void FormatArmorStats(Equipment equipmentItem)
    {
        FormatArmorStat(equipmentItem.ModifyEquipmentPhysicalArmorBonus, Equipment.EquipmentStat.PhysicalArmor);
        FormatArmorStat(equipmentItem.ModifyEquipmentResistanceArmorBonus, Equipment.EquipmentStat.Resistance);
        FormatArmorStat(equipmentItem.ModifyEquipmentExtraStatBonus, equipmentItem.EquipmentExtraStatType);
        FormatArmorStat(equipmentItem.ModifyEquipmentPenaltyStatBonus, equipmentItem.EquipmentPenaltyStatType);
        
        // TODO : 토큰 면역력 능력치 텍스트
    }

    /// <summary>
    /// 상세 스텟 포맷 설정 (디버프 및 버프 구별)
    /// </summary>
    /// <param name="statBonus">스텟 수치 값</param>
    /// <param name="statType">스텟 종류</param>
    private void FormatArmorStat(int statBonus, Equipment.EquipmentStat statType)
    {
        // 버프인 경우
        if (statBonus > 0)
        {
            detailBaiscStatsTextFormat.Append("<color=").Append(_BUFF_TEXT_COLOR).Append(">")
                .Append("+").Append(statBonus).Append(" ").Append(_STAT_FORMAT_TEXT[statType]).Append("</color>\n");
        }
        // 디버프 인경우
        else if (statBonus < 0)
        {
            /* 디버프 인경우 데이터 정수 값이 "음수"로 들어온다면 Append("-") 연산자가 필요없음 */
             
           /* detailBaiscStatsTextFormat.Append("<color=").Append(_DEBUFF_TEXT_COLOR).Append(">")
                .Append("-").Append(statBonus).Append(" ").Append(_STAT_FORMAT_TEXT[statType]).Append("</color>\n"); */
              
            detailBaiscStatsTextFormat.Append("<color=").Append(_DEBUFF_TEXT_COLOR).Append(">")
                .Append(statBonus).Append(" ").Append(_STAT_FORMAT_TEXT[statType]).Append("</color>\n");
        }
    }

    protected abstract void SetItemInfo(bool isRewardItem);
}
