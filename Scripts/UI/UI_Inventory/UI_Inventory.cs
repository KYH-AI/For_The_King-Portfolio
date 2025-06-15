using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;

public class UI_Inventory : UI_Base, I_Inventory
{
    // 인벤토리 텍스트
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


    // 인벤토리 필터 버튼
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
        // TODO : 성소 아이콘 이미지 
    }
    

    // 인벤토리 아이템 리스트 
    private enum InventoryItemListRoot
    {
        BackPackContents
    }

    private PlayerInventory _requestPlayerInventory;

    // 백팩 아이템 리스트 프리팹 원본
    private UI_InventoryItemIconList _uiInventoryItemIconPrefab;

    // 백팩 아이템 리스트 오브젝트 풀링 리스트
    private List<UI_InventoryItemIconList> _uiInventoryItemIconList = new List<UI_InventoryItemIconList>();

    private Dictionary<Equipment.EquipmentSlotType, TextMeshProUGUI> equipmentTexts;
    
    private Dictionary<Equipment.EquipmentSlotType, Button> equipmentButtons;
    
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Transform>(typeof(InventoryItemListRoot));
        Bind<Button>(typeof(BackPackFilterButtons));

        _uiInventoryItemIconPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemIconList/UI_InventoryItemIconList").GetOrAddComponent<UI_InventoryItemIconList>();


        // 인벤토리 아이템 필터 버튼마다 정렬되는 이벤트 등록
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
        
        
  

        // 아이템 착용슬롯에 빠른 접근을 위한 Text 캐싱
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

        // 아이템 착용슬롯에 빠른 접근을 위한 Button 캐싱
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
        
          // 아이템 착용 슬롯 각 버튼에 마우스 클릭 그리고 마우스 호버 이벤트 등록
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
    /// 플레이어 인벤토리 UI 업데이트
    /// </summary>
    /// <param name="requestPlayer">업데이트 대상의 인벤토리</param>
    public void InventoryUiUpdate(PlayerStats requestPlayer)
    {
        
        /*  1. 골드 (OK)
         *  2. 플레이어 초상화
         *  3. 획득한 성소
         *  4. 무기, 방패, 갑옷, 헬멧, 장화, 반지, 부적 파츠
         *  5. 모든 아이템 필터 개수, 무기 필터 개수, 갑옷 필터 개수, 악세서리 필터 개수, 소모품 필터 개수, 퀘스트 아이템 필터 개수 (OK)
         *  6. 필터별 아이템 리스트 (OK)
         */

        _requestPlayerInventory = requestPlayer.PlayerInventory;
        
        // 골드 Text 업데이트
        Get<TextMeshProUGUI>((int)Texts.GoldText).text = requestPlayer.CurrentGold.ToString();

        // 아이템 필터 별로 개수 Text 업데이트
        int[] itemList = Managers.Inventory.FilterInventoryItemCount(requestPlayer.PlayerInventory);
        
        for (int i = 0; i < Enum.GetValues(typeof(Texts)).Length-8; i++)
        {
            // 모든 필터 Text ~ 퀘스트 아이템 필터 Text까지 반복
            Get<TextMeshProUGUI>(i).text = itemList[i].ToString();
        }
        
        // 아이템 파츠 모든 부위별 초기화
        UpdateUiEquipSlot(Equipment.EquipmentSlotType.NONE, null);

         // 인벤토리 아이템 리스트 초기화
        UpdateInventoryCategoryFilter(Item.ItemType.NONE);


        // 아이템을 다른 캐릭터에게 전달한 경우 인벤토리 UI 활성화는 무시
        if (_requestPlayerInventory != requestPlayer.PlayerInventory) return;
    }

    /// <summary>
    /// 플레이어 아이템 착용슬롯 UI 업데이트
    /// </summary>
    /// <param name="equipmentSlotType">아이템 슬롯 부위</param>
    /// <param name="equipment">새로 장착된 아이템 정보</param>
    private void UpdateUiEquipSlot(Equipment.EquipmentSlotType equipmentSlotType, Equipment equipment)
    {
        /* 1. 아이템 이름
         * 2. 버튼 활성화
         * 3. 버튼 클릭 시 아이템 장착에 관련된 버튼 UI 리스트 출력
         * 4. 아이템 마우스 올리 시 장착중인 아이템 카드 출력
         * 5. 장비를 장착해체 할 경우 아이템 이름은 String.Empty, 버튼도 비활성화
         */

        
        // 모든 아이템 착용 슬롯 업데이트
        if (equipmentSlotType is Equipment.EquipmentSlotType.NONE)
        {
            // 플레이어가 현재 착용중인 모든 아이템 슬롯에 대한 정보를 배열로 가져옴
            Equipment[] equipmentArray = Managers.Inventory.SlotEquipmentsItem(_requestPlayerInventory);
            
            // 아이템 착용 슬롯 개수 만큼 반복진행
            for (int i = 0; i < equipmentTexts.Count; i++)
            {
                // 현재 특정 아이템 슬롯에 장착된 아이템이 없는 경우  아이템 슬롯에 이름을 비우고, 아이템 슬롯 버튼도 비활성화
                equipmentTexts[(Equipment.EquipmentSlotType)i].text = equipmentArray[i] is null ? string.Empty : equipmentArray[i].ItemName;
                equipmentButtons[(Equipment.EquipmentSlotType)i].interactable = equipmentArray[i] != null;
                equipmentButtons[(Equipment.EquipmentSlotType)i].gameObject.SetActive(equipmentArray[i] != null);
            }
            return;
        }
        
        // 특정 아이템 착용 슬롯 업데이트
        equipmentTexts[equipmentSlotType].text = equipment is null ? string.Empty : equipment.ItemName;
        equipmentButtons[equipmentSlotType].interactable = equipment != null;
        equipmentButtons[equipmentSlotType].gameObject.SetActive(equipment != null);
    }

    /// <summary>
    /// 인벤토리 아이템 리스트 필터에 맞게 나열
    /// </summary>
    /// <param name="itemType">아이템 필터 타입</param>
    private void UpdateInventoryCategoryFilter(Item.ItemType itemType)
    {
        List<Item> itemList = Managers.Inventory.FilterInventoryItems(_requestPlayerInventory, itemType);

        if (itemList.Count > _uiInventoryItemIconList.Count)
        {
            CreateItemListUI(itemList.Count - _uiInventoryItemIconList.Count);
        }
        
        // 상점 아이템 리스트 표시
        int itemListIndex; // 아이템 리스트를 표시해야할 최대 개수
        for (itemListIndex = 0; itemListIndex < itemList.Count; itemListIndex++)
        {
            _uiInventoryItemIconList[itemListIndex].SetItemInfo(itemList[itemListIndex]);
            _uiInventoryItemIconList[itemListIndex].gameObject.SetActive(true);
        }
        
        //  상점 아이템 리스트 미표시 (i = 현재 표시할 아이템 인덱스 마지막 위치, 언제까지? 오브젝트 풀링 리스트에 남아있는 모든 리스트를 비활성화까지)
        for (int i = itemList.Count; i < _uiInventoryItemIconList.Count; i++)
        {
            // 아이템 리스트 오브젝트 비활성화
            _uiInventoryItemIconList[i].gameObject.SetActive(false);
        }
    }
    
    
    /// <summary>
    /// 아이템 리스트 프리팹 동적할당
    /// </summary>
    /// <param name="count">프리팹 생성 개수</param>
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
