using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;

public class UI_Inventory : UI_Base, I_Inventory
{
    // �κ��丮 �ؽ�Ʈ
    private enum Texts
    {
        BackPackAllFilterText,
        BackPackWeaponFilterText,
        BackPackArmorFilterText,
        BackPackAccessoryFilterText,
        BackPackConsumableFilterText,
        BackPackQuestFilterText,
        
        GoldText,
        
        WeaponEquipSlot,
        ShieldEquipSlot,
        ArmorEquipSlot,
        HelmetEquipSlot,
        BootsEquipSlot,
        RingEquipSlot,
        AmuletEquipSlot,
    }


    // �κ��丮 ���� ��ư
    private enum BackPackFilterButtons
    {
        AllBackPackFilterButton = 0,
        WeaponsBackPackFilterButton = 1,
        ArmorsBackPackFilterButton = 2,
        AccessoryBackPackFilterButton = 3,
        ConsumableBackPackFilterButton = 4,
        QuestBackPackFilterButton = 5,
        
        WeaponEquipSlot,
        ShieldEquipSlot,
        ArmorEquipSlot,
        HelmetEquipSlot,
        BootsEquipSlot,
        RingEquipSlot,
        AmuletEquipSlot
    }

    private enum SanctuaryIcon
    {
        // TODO : ���� ������ �̹��� 
    }
    

    // �κ��丮 ������ ����Ʈ 
    private enum InventoryItemListRoot
    {
        BackPackContents
    }

    private PlayerInventory _requestPlayerInventory;

    // ���� ������ ����Ʈ ������ ����
    private UI_InventoryItemIconList _uiInventoryItemIconPrefab;

    // ���� ������ ����Ʈ ������Ʈ Ǯ�� ����Ʈ
    private List<UI_InventoryItemIconList> _uiInventoryItemIconList = new List<UI_InventoryItemIconList>();

    private Dictionary<Equipment.EquipmentSlotType, TextMeshProUGUI> equipmentTexts;
    
    private Dictionary<Equipment.EquipmentSlotType, Button> equipmentButtons;
    
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Transform>(typeof(InventoryItemListRoot));
        Bind<Button>(typeof(BackPackFilterButtons));

        _uiInventoryItemIconPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemIconList/UI_InventoryItemIconList").GetOrAddComponent<UI_InventoryItemIconList>();


        // �κ��丮 ������ ���� ��ư���� ���ĵǴ� �̺�Ʈ ���
        Get<Button>((int)BackPackFilterButtons.AllBackPackFilterButton).onClick
            .AddListener(() => UpdateInventoryCategoryFilter(Item.ItemType.NONE));
        
        Get<Button>((int)BackPackFilterButtons.WeaponsBackPackFilterButton).onClick
            .AddListener(() => UpdateInventoryCategoryFilter(Item.ItemType.Weapon));
        
        Get<Button>((int)BackPackFilterButtons.ArmorsBackPackFilterButton).onClick
            .AddListener(() => UpdateInventoryCategoryFilter(Item.ItemType.Armor));
        
        Get<Button>((int)BackPackFilterButtons.AccessoryBackPackFilterButton).onClick
            .AddListener(() => UpdateInventoryCategoryFilter(Item.ItemType.Accessory));
        
        Get<Button>((int)BackPackFilterButtons.ConsumableBackPackFilterButton).onClick
            .AddListener(() => UpdateInventoryCategoryFilter(Item.ItemType.Consumable));
        
        Get<Button>((int)BackPackFilterButtons.QuestBackPackFilterButton).onClick
            .AddListener(() => UpdateInventoryCategoryFilter(Item.ItemType.QuestItem));
        
        
  

        // ������ ���뽽�Կ� ���� ������ ���� Text ĳ��
        equipmentTexts = new Dictionary<Equipment.EquipmentSlotType, TextMeshProUGUI>
        {
            {Equipment.EquipmentSlotType.PrimaryWeapon, Get<TextMeshProUGUI>((int)Texts.WeaponEquipSlot)},
            {Equipment.EquipmentSlotType.Shield, Get<TextMeshProUGUI>((int)Texts.ShieldEquipSlot)},
            {Equipment.EquipmentSlotType.Clothes, Get<TextMeshProUGUI>((int)Texts.ArmorEquipSlot)},
            {Equipment.EquipmentSlotType.Helmet, Get<TextMeshProUGUI>((int)Texts.HelmetEquipSlot)},
            {Equipment.EquipmentSlotType.Boots, Get<TextMeshProUGUI>((int)Texts.BootsEquipSlot)},
            {Equipment.EquipmentSlotType.Ring, Get<TextMeshProUGUI>((int)Texts.RingEquipSlot)},
            {Equipment.EquipmentSlotType.Amulet, Get<TextMeshProUGUI>((int)Texts.AmuletEquipSlot)},
        };

        // ������ ���뽽�Կ� ���� ������ ���� Button ĳ��
        equipmentButtons = new Dictionary<Equipment.EquipmentSlotType, Button>
        {
            { Equipment.EquipmentSlotType.PrimaryWeapon, Get<Button>((int)BackPackFilterButtons.WeaponEquipSlot) },
            { Equipment.EquipmentSlotType.Shield, Get<Button>((int)BackPackFilterButtons.ShieldEquipSlot) },
            { Equipment.EquipmentSlotType.Clothes, Get<Button>((int)BackPackFilterButtons.ArmorEquipSlot) },
            { Equipment.EquipmentSlotType.Helmet, Get<Button>((int)BackPackFilterButtons.HelmetEquipSlot) },
            { Equipment.EquipmentSlotType.Boots, Get<Button>((int)BackPackFilterButtons.BootsEquipSlot) },
            { Equipment.EquipmentSlotType.Ring, Get<Button>((int)BackPackFilterButtons.RingEquipSlot) },
            { Equipment.EquipmentSlotType.Amulet, Get<Button>((int)BackPackFilterButtons.AmuletEquipSlot) },
        };
        
          // ������ ���� ���� �� ��ư�� ���콺 Ŭ�� �׸��� ���콺 ȣ�� �̺�Ʈ ���
        foreach (var equipmentButton in equipmentButtons)
        {
            equipmentButton.Value.onClick.AddListener( () => Managers.UIManager.ShowItemMenuButtonUI(_requestPlayerInventory.Owner, 
                                                                                                            Managers.Inventory.SlotEquipmentsItem(_requestPlayerInventory)[(int)equipmentButton.Key],
                                                                                                            UI_ItemMenuButton.ItemMenuType.LoadOut,
                                                                                                            equipmentButton.Value.transform.position));
            equipmentButton.Value.gameObject.BindEvent(_ => Managers.UIManager.ShowMainItemCardUI(
                    Managers.Inventory.SlotEquipmentsItem(_requestPlayerInventory)[(int)equipmentButton.Key], 
                    UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, true), 
                    MouseUIEvent.Enter);
            equipmentButton.Value.gameObject.BindEvent(_ => Managers.UIManager.ShowMainItemCardUI(
                    Managers.Inventory.SlotEquipmentsItem(_requestPlayerInventory)[(int)equipmentButton.Key],
                    UI_PlayerEquippedItemCard.EquippedTarget.EquipInventoryTarget, false),
                    MouseUIEvent.Exit);
        }
    }

    /// <summary>
    /// �÷��̾� �κ��丮 UI ������Ʈ
    /// </summary>
    /// <param name="requestPlayer">������Ʈ ����� �κ��丮</param>
    public void InventoryUiUpdate(PlayerStats requestPlayer)
    {
        
        /*  1. ��� (OK)
         *  2. �÷��̾� �ʻ�ȭ
         *  3. ȹ���� ����
         *  4. ����, ����, ����, ���, ��ȭ, ����, ���� ����
         *  5. ��� ������ ���� ����, ���� ���� ����, ���� ���� ����, �Ǽ����� ���� ����, �Ҹ�ǰ ���� ����, ����Ʈ ������ ���� ���� (OK)
         *  6. ���ͺ� ������ ����Ʈ (OK)
         */

        _requestPlayerInventory = requestPlayer.PlayerInventory;
        
        // ��� Text ������Ʈ
        Get<TextMeshProUGUI>((int)Texts.GoldText).text = requestPlayer.CurrentGold.ToString();

        // ������ ���� ���� ���� Text ������Ʈ
        int[] itemList = Managers.Inventory.FilterInventoryItemCount(requestPlayer.PlayerInventory);
        
        for (int i = 0; i < Enum.GetValues(typeof(Texts)).Length-8; i++)
        {
            // ��� ���� Text ~ ����Ʈ ������ ���� Text���� �ݺ�
            Get<TextMeshProUGUI>(i).text = itemList[i].ToString();
        }
        
        // ������ ���� ��� ������ �ʱ�ȭ
        UpdateUiEquipSlot(Equipment.EquipmentSlotType.NONE, null);

         // �κ��丮 ������ ����Ʈ �ʱ�ȭ
        UpdateInventoryCategoryFilter(Item.ItemType.NONE);


        // �������� �ٸ� ĳ���Ϳ��� ������ ��� �κ��丮 UI Ȱ��ȭ�� ����
        if (_requestPlayerInventory != requestPlayer.PlayerInventory) return;
    }

    /// <summary>
    /// �÷��̾� ������ ���뽽�� UI ������Ʈ
    /// </summary>
    /// <param name="equipmentSlotType">������ ���� ����</param>
    /// <param name="equipment">���� ������ ������ ����</param>
    private void UpdateUiEquipSlot(Equipment.EquipmentSlotType equipmentSlotType, Equipment equipment)
    {
        /* 1. ������ �̸�
         * 2. ��ư Ȱ��ȭ
         * 3. ��ư Ŭ�� �� ������ ������ ���õ� ��ư UI ����Ʈ ���
         * 4. ������ ���콺 �ø� �� �������� ������ ī�� ���
         * 5. ��� ������ü �� ��� ������ �̸��� String.Empty, ��ư�� ��Ȱ��ȭ
         */

        
        // ��� ������ ���� ���� ������Ʈ
        if (equipmentSlotType is Equipment.EquipmentSlotType.NONE)
        {
            // �÷��̾ ���� �������� ��� ������ ���Կ� ���� ������ �迭�� ������
            Equipment[] equipmentArray = Managers.Inventory.SlotEquipmentsItem(_requestPlayerInventory);
            
            // ������ ���� ���� ���� ��ŭ �ݺ�����
            for (int i = 0; i < equipmentTexts.Count; i++)
            {
                // ���� Ư�� ������ ���Կ� ������ �������� ���� ���  ������ ���Կ� �̸��� ����, ������ ���� ��ư�� ��Ȱ��ȭ
                equipmentTexts[(Equipment.EquipmentSlotType)i].text = equipmentArray[i] is null ? string.Empty : equipmentArray[i].ItemName;
                equipmentButtons[(Equipment.EquipmentSlotType)i].interactable = equipmentArray[i] != null;
                equipmentButtons[(Equipment.EquipmentSlotType)i].gameObject.SetActive(equipmentArray[i] != null);
            }
            return;
        }
        
        // Ư�� ������ ���� ���� ������Ʈ
        equipmentTexts[equipmentSlotType].text = equipment is null ? string.Empty : equipment.ItemName;
        equipmentButtons[equipmentSlotType].interactable = equipment != null;
        equipmentButtons[equipmentSlotType].gameObject.SetActive(equipment != null);
    }

    /// <summary>
    /// �κ��丮 ������ ����Ʈ ���Ϳ� �°� ����
    /// </summary>
    /// <param name="itemType">������ ���� Ÿ��</param>
    private void UpdateInventoryCategoryFilter(Item.ItemType itemType)
    {
        List<Item> itemList = Managers.Inventory.FilterInventoryItems(_requestPlayerInventory, itemType);

        if (itemList.Count > _uiInventoryItemIconList.Count)
        {
            CreateItemListUI(itemList.Count - _uiInventoryItemIconList.Count);
        }
        
        // ���� ������ ����Ʈ ǥ��
        int itemListIndex; // ������ ����Ʈ�� ǥ���ؾ��� �ִ� ����
        for (itemListIndex = 0; itemListIndex < itemList.Count; itemListIndex++)
        {
            _uiInventoryItemIconList[itemListIndex].SetItemInfo(itemList[itemListIndex]);
            _uiInventoryItemIconList[itemListIndex].gameObject.SetActive(true);
        }
        
        //  ���� ������ ����Ʈ ��ǥ�� (i = ���� ǥ���� ������ �ε��� ������ ��ġ, ��������? ������Ʈ Ǯ�� ����Ʈ�� �����ִ� ��� ����Ʈ�� ��Ȱ��ȭ����)
        for (int i = itemList.Count; i < _uiInventoryItemIconList.Count; i++)
        {
            // ������ ����Ʈ ������Ʈ ��Ȱ��ȭ
            _uiInventoryItemIconList[i].gameObject.SetActive(false);
        }
    }
    
    
    /// <summary>
    /// ������ ����Ʈ ������ �����Ҵ�
    /// </summary>
    /// <param name="count">������ ���� ����</param>
    private void CreateItemListUI(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UI_InventoryItemIconList uiItemList = Instantiate(_uiInventoryItemIconPrefab, Get<Transform>((int)InventoryItemListRoot.BackPackContents).transform);
            uiItemList.Init();
            uiItemList.gameObject.SetActive(false);
            _uiInventoryItemIconList.Add(uiItemList);
            
        }
    }
}
