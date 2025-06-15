using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_ItemCard : UI_Base
{
    /*  <������ ī��>
     *  1. �÷��̾� ���� ������ ī��, ���� ������ ī��
     *  2. ������
     *     2-1. ������ �̸�
     *     2-2. ������ ���
     *     2-3. ������ ����
     *       2-3-1. ���� (���� ������, ���� ������ Ÿ��, ���� �ɼ� ����, ���� �䱸 ���� ������, ���� ��ų ������, ���� ������ ������, ���� ���丮)
     *       2-3-2. �� (�� �ɼ� ����, �� ���丮)
     *       2-3-3. �Ҹ�ǰ (�Ҹ�ǰ �ɼ� ����, �Ҹ�ǰ ���丮)
     *     2-4. ������ �ʻ�ȭ ������
     *  3. ������
     *    3-1. ���� ������ ī��� ������ 3D �ʻ�ȭ�� ����
     *    3-2. �÷��̾� ���� ������ ī��� 3D �ʻ�ȭ�� ���� ���� ����
     *    3-3. ���� ������ ī��� Item Stock �ؽ�Ʈ�� ���� (ex : ��ȭ ����)
     */

    protected enum Images
    {
        // ���� �䱸 ���� ������
        WeaponBaseStatIcon_0 = 0,
        WeaponBaseStatIcon_1 = 1,
        WeaponBaseStatIcon_2 = 2,
        WeaponBaseStatIcon_3 = 3,
        WeaponBaseStatIcon_4 = 4,
        WeaponBaseStatIcon_5 = 5,
        
        // ���� ��޿� �´� ��� ����
        ItemCardBackGround,
        ItemImageBackGround,
        ItemTitleBackGround,
        ItemCardDividerBottomBackGround,
        ItemCardDividerTopBackGround,
        
        // ���� ������
        ItemIcon,
    }
    
    protected enum Texts
    {
        ItemTitleText,
        ItemRarityText,

        /* ���� ��Ʈ */
        WeaponDamageText,
        WeaponDamageTypeText,
        WeaponDetailStatsText,
        
        /* �� ��Ʈ */
        EquippableDetailStatsText,
        
        /* �Ҹ�ǰ ��Ʈ */
        ConsumableItemDetailText,
    }

    protected enum GameObjects
    {
        // ���� ����
        WeaponDetail,
        // �� ����
        EquippableDetail,
        // �Ҹ�ǰ ����
        ConsumableItemDetail,

        // ���� Ư¡ ������
        WeaponTwoHandedIcon,
        WeaponNonFocusableIcon,
        WeaponBreakableIcon,
        
        // ���� ��ų Ư¡ ������
        WeaponSingleTargetIcon,
        WeaponSplashTargetIcon,
        WeaponAoeTargetIcon,
        WeaponPierceTargetIcon,
        WeaponHeavyTargetIcon,
    }

    // ���� ��޿� �´� ���� �ڵ�
    private static  Dictionary<Item.Grade, string> _ITEM_GRADE_COLOR = new Dictionary<Item.Grade, string>()
    {
        { Item.Grade.COMMON,  "#FFFFFF" },     // White
        { Item.Grade.ADVANCED, "#52A859"  },   // Green
        { Item.Grade.RARE, "#0096FF" },        // Blue
        { Item.Grade.HERO, "#B54CFF" },        // Purple
        { Item.Grade.LEGEND, "#FF8C00"  },      // Gold
        { Item.Grade.MYTH, "#C90000" },         // ������
        { Item.Grade.QUEST, "#FFBF00"}         // �����
    };
    
    // ���� ��޿� �´� �ؽ�Ʈ
    private static  Dictionary<Item.Grade, string> _ITEM_GRADE_TEXT = new Dictionary<Item.Grade, string>()
    {
        { Item.Grade.COMMON,  "�Ϲ�" },     // White
        { Item.Grade.ADVANCED, "���"  },   // Green
        { Item.Grade.RARE, "���" },        // Blue
        { Item.Grade.HERO, "����" },        // Purple
        { Item.Grade.LEGEND, "����"  },      // Gold
        { Item.Grade.MYTH, "��ȭ" },         // ������
        { Item.Grade.QUEST, "����Ʈ"}         // �����
    };
    
    // ��, ���⿡ �ɷ�ġ �ؽ�Ʈ
    private static Dictionary<Equipment.EquipmentStat, string> _STAT_FORMAT_TEXT = new Dictionary<Equipment.EquipmentStat, string>()
    {
        { Equipment.EquipmentStat.Strength, "��"},  
        { Equipment.EquipmentStat.Awareness, "����"  },  
        { Equipment.EquipmentStat.Intelligence, "����" },  
        { Equipment.EquipmentStat.Vitality, "Ȱ��"}  ,  
        { Equipment.EquipmentStat.Quickness, "�ӵ�"}  ,
        { Equipment.EquipmentStat.Avoid,  "ȸ�Ƿ�" },
        { Equipment.EquipmentStat.PhysicalDamage, "���� ���ݷ�" },        
        { Equipment.EquipmentStat.MagicalDamage, "���� ���ݷ�"  },     
        { Equipment.EquipmentStat.MaxFocus, "�ִ� ���߷�" },   
        { Equipment.EquipmentStat.PhysicalArmor, "���� ����" }, 
        { Equipment.EquipmentStat.Resistance, "���� ����" }
    };

    // ������ ȿ�� �ؽ�Ʈ ����
    private static string _BUFF_TEXT_COLOR = "#86B0F4"; // Blue
    
    // ������ ȿ�� �ؽ�Ʈ ����
    private static string _DEBUFF_TEXT_COLOR = "#FF4752"; // Red
    
    // ������ ���ݼ��� �ؽ�Ʈ
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
        
        // ���� ��޿� �´� ��� �̹��� ���� (16���� -> RGBA �ڵ�� ��ȯ)
        ColorUtility.TryParseHtmlString(_ITEM_GRADE_COLOR[item.ItemGrade], out Color itemGradeColor);

        // ������ ���� ���� �ؽ�Ʈ �ʱ�ȭ
        detailBaiscStatsTextFormat.Clear();
        
        // ������ �̸� ����
        Get<TextMeshProUGUI>((int)Texts.ItemTitleText).text = Item.ItemName;
        
        // ������ ��� �ؽ�Ʈ ����
        Get<TextMeshProUGUI>((int)Texts.ItemRarityText).text = _ITEM_GRADE_TEXT[Item.ItemGrade];
        
        // ������ ��� ������ ����
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
                
                // ���Ⱑ �䱸�ϴ� ���� ������ �ʱ�ȭ
                Get<Image>((int)Images.WeaponBaseStatIcon_0).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_1).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_2).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_3).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_4).gameObject.SetActive(false);
                Get<Image>((int)Images.WeaponBaseStatIcon_5).gameObject.SetActive(false);
                
                // ���� Ÿ�� ����â�� Ȱ��ȭ
                Get<GameObject>((int)GameObjects.EquippableDetail).SetActive(false);
                Get<GameObject>((int)GameObjects.ConsumableItemDetail).SetActive(false);

                // ���� ��ų ������ �ʱ�ȭ
                Get<GameObject>((int)GameObjects.WeaponSingleTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponSplashTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponAoeTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponPierceTargetIcon).SetActive(false);
                Get<GameObject>((int)GameObjects.WeaponHeavyTargetIcon).SetActive(false);
                
                // ���� Ư�� ���� ������ �ʱ�ȭ
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

                    // ��ų �̸� �ؽ�Ʈ
                    detailStatsTextFormat += skill.Name;
                                       
                    if (i < weaponSkillIndex.Count - 1)
                    {
                        detailStatsTextFormat += ", ";
                    }

                    // ��ų �ʻ�ȭ ������
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

                // �ش� ���Ⱑ �䱸�ϴ� ���� ������ �̹���
                Sprite skillStatIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    weapon.WeaponBasestat.ToString()); 
                
                //  ���Ⱑ �䱸�ϴ� ���� ���� ������ ������ŭ Ȱ��ȭ �� ���� ������ ����
                for (int i = 0; i < Managers.Data.GetUsedSkillInfo(weaponSkillIndex[0]).Rolls; i++)
                {
                    Get<Image>(i).gameObject.SetActive(true);
                    Get<Image>(i).sprite = skillStatIcon;
                }
                
                // ���Ⱑ ��� ������ ��� ��� ���� ������ Ȱ��ȭ
                Get<GameObject>((int)GameObjects.WeaponTwoHandedIcon).SetActive(weapon.WeaponHandlegrip is Weapon.WeaponHandleGrip.TwoHand);
                
                // ���� ������ �ؽ�Ʈ �� ���� ����
                Get<TextMeshProUGUI>((int)Texts.WeaponDamageText).text = weapon.Damage.ToString();
                Get<TextMeshProUGUI>((int)Texts.WeaponDamageText).color = Get<TextMeshProUGUI>((int)Texts.WeaponDamageTypeText).color = weapon.Damagetype is Weapon.DamageType.Physical
                    ? UI_PlayerMainHUD.PHYSICAL_DAMAGE_TYPE_COLOR
                    : UI_PlayerMainHUD.MAGICAL_DAMAGE_TYPE_COLOR;

                Get<TextMeshProUGUI>((int)Texts.WeaponDamageTypeText).text =
                    weapon.Damagetype is Weapon.DamageType.Physical ? "���� ���ݷ�" : "���� ���ݷ�";
                
                detailStatsTextFormat += "</color>\n";

                // ���� �� ���� �ؽ�Ʈ ���� ����
                Get<TextMeshProUGUI>((int)Texts.WeaponDetailStatsText).text = detailStatsTextFormat;
                
                Get<GameObject>((int)GameObjects.WeaponDetail).SetActive(true);

                break;
            
            case Item.ItemType.Armor :  
                // �� Ÿ�� ����â�� Ȱ��ȭ
                Get<GameObject>((int)GameObjects.WeaponDetail).SetActive(false);
                Get<GameObject>((int)GameObjects.ConsumableItemDetail).SetActive(false);
                
                Armor armor = (Armor)item;  
                
                itemIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    armor.Armortype.ToString(), true);

               

                /*
                  string detailArmorStatsTextFormat = string.Empty;
                 
                // ���� �ɷ�ġ �ؽ�Ʈ
                
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

                // ���� �ɷ�ġ �ؽ�Ʈ
                
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
                
                
                // �� �� ���� �ؽ�Ʈ ���� ����
                Get<TextMeshProUGUI>((int)Texts.EquippableDetailStatsText).text = detailBaiscStatsTextFormat.ToString();
                
                Get<GameObject>((int)GameObjects.EquippableDetail).SetActive(true);
                
                
                break;
            case Item.ItemType.Consumable or Item.ItemType.GoldCoin :  
                // �Ҹ�ǰ ������ Ÿ�� ����â�� Ȱ��ȭ
                Get<GameObject>((int)GameObjects.WeaponDetail).SetActive(false);
                Get<GameObject>((int)GameObjects.EquippableDetail).SetActive(false);

                itemIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.ItemCardIcon,
                    Managers.Data.GetItemName(Item.Itemtype is Item.ItemType.Consumable ? Item.ItemType.Consumable : Item.ItemType.GoldCoin, Item.ItemID), true);
                
                Get<TextMeshProUGUI>((int)Texts.ConsumableItemDetailText).text = Item.ItemDescription;
                
                break;
            case Item.ItemType.Accessory : Item = (Accessory)item;  break;
        }
        
        
        // ������ �ʻ�ȭ ������ ����
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
    /// ��, ���⿡ ���� �� ���� ���� �ؽ�Ʈ ����
    /// </summary>
    /// <param name="equipmentItem">������ ������</param>
    private void FormatArmorStats(Equipment equipmentItem)
    {
        FormatArmorStat(equipmentItem.ModifyEquipmentPhysicalArmorBonus, Equipment.EquipmentStat.PhysicalArmor);
        FormatArmorStat(equipmentItem.ModifyEquipmentResistanceArmorBonus, Equipment.EquipmentStat.Resistance);
        FormatArmorStat(equipmentItem.ModifyEquipmentExtraStatBonus, equipmentItem.EquipmentExtraStatType);
        FormatArmorStat(equipmentItem.ModifyEquipmentPenaltyStatBonus, equipmentItem.EquipmentPenaltyStatType);
        
        // TODO : ��ū �鿪�� �ɷ�ġ �ؽ�Ʈ
    }

    /// <summary>
    /// �� ���� ���� ���� (����� �� ���� ����)
    /// </summary>
    /// <param name="statBonus">���� ��ġ ��</param>
    /// <param name="statType">���� ����</param>
    private void FormatArmorStat(int statBonus, Equipment.EquipmentStat statType)
    {
        // ������ ���
        if (statBonus > 0)
        {
            detailBaiscStatsTextFormat.Append("<color=").Append(_BUFF_TEXT_COLOR).Append(">")
                .Append("+").Append(statBonus).Append(" ").Append(_STAT_FORMAT_TEXT[statType]).Append("</color>\n");
        }
        // ����� �ΰ��
        else if (statBonus < 0)
        {
            /* ����� �ΰ�� ������ ���� ���� "����"�� ���´ٸ� Append("-") �����ڰ� �ʿ���� */
             
           /* detailBaiscStatsTextFormat.Append("<color=").Append(_DEBUFF_TEXT_COLOR).Append(">")
                .Append("-").Append(statBonus).Append(" ").Append(_STAT_FORMAT_TEXT[statType]).Append("</color>\n"); */
              
            detailBaiscStatsTextFormat.Append("<color=").Append(_DEBUFF_TEXT_COLOR).Append(">")
                .Append(statBonus).Append(" ").Append(_STAT_FORMAT_TEXT[statType]).Append("</color>\n");
        }
    }

    protected abstract void SetItemInfo(bool isRewardItem);
}
