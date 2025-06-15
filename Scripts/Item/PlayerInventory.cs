using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{

    public PlayerStats Owner { get; private set; }

    
    #region 플레이어 아이템 착용 파트

    private Dictionary<Equipment.EquipmentSlotType, Equipment> _playerEquipmentSlots = new Dictionary<Equipment.EquipmentSlotType, Equipment>();

    #endregion

    
    #region 플레이어 아이템 인벤토리

    // 아이템 타입 기준으로 아이템 리스트를 이용해서 저장
    // (아이템 ID가 DB에서 int형이 아닌 string으로 변경될 수 있으니)
    private Dictionary<Item.ItemType, Dictionary<string, Item>> _playerItemDictionary;

    #endregion

    
    // 착용 장비 초기화 함수
    public PlayerInventory(PlayerStats owner, Weapon weapon = null, Armor shield = null, 
                            Armor armor = null, Armor helmet = null, Armor boots = null, 
                            Accessory amulet = null, Accessory ring = null)
    {
        // 소유자 초기화
        this.Owner = owner;
        
        /* 1. 무기
        * 2. 방패
        * 3. 방어구
        * 4. 헬멧
        * 5. 신발
        * 6. 반지
        * 7. 목걸이
        */
        

        /* 인벤토리 딕셔너리 초기화 */
        _playerItemDictionary = new Dictionary<Item.ItemType, Dictionary<string, Item>>
        {
            [Item.ItemType.Weapon] = new Dictionary<string, Item>(),
            [Item.ItemType.Armor] = new Dictionary<string, Item>(),
            [Item.ItemType.Accessory] = new Dictionary<string, Item>(),
            [Item.ItemType.Consumable] = new Dictionary<string, Item>(),
            [Item.ItemType.QuestItem] = new Dictionary<string, Item>()
        };
        


        /* 장비 파츠 초기화 */
        _playerEquipmentSlots = new Dictionary<Equipment.EquipmentSlotType, Equipment>()
        {
            [Equipment.EquipmentSlotType.PrimaryWeapon] = weapon,
            [Equipment.EquipmentSlotType.Shield] = shield,
            [Equipment.EquipmentSlotType.Clothes] = armor,
            [Equipment.EquipmentSlotType.Helmet] = helmet,
            [Equipment.EquipmentSlotType.Boots] = boots,
            [Equipment.EquipmentSlotType.Ring] = ring,
            [Equipment.EquipmentSlotType.Amulet] = amulet,
        };
    }
    
    #region 인벤토리 확인 함수

    
    /// <summary>
    /// 플레이어 인벤토리에 아이템 타입으로 확인
    /// </summary>
    /// <param name="itemType">찾고자하는 아이템 타입</param>
    /// <returns>해당 아이템 타입이 모여있는 딕셔너리</returns>
    public Dictionary<string, Item> FilterItemsByType(Item.ItemType itemType)
    {
        if (_playerItemDictionary.ContainsKey(itemType))
        {
            return _playerItemDictionary[itemType];
        }

        return new Dictionary<string, Item>();
    }

    /// <summary>
    /// 플레이어 인벤토리에 필터링 없이 모든 아이템 확인
    /// </summary>
    /// <returns>모든 아이템이 모여있는 리스트 반환</returns>
    public List<Item> NoneFilterItemsByType()
    {
        List<Item> allItems = new List<Item>();

        foreach (var itemDictionary in _playerItemDictionary.Values)
        {
            allItems.AddRange(itemDictionary.Values);
        }

        return allItems;
    }

    /// <summary>
    /// 플레이어 인벤토리에 특정 아이템 존재여부 확인
    /// </summary>
    /// <param name="item">찾고자하는 아이템</param>
    /// <returns>아이템 존재 여부</returns>
    public bool ContainsInventoryItemCheck(Item item)
    {
        // 3가지 경우를 통과해야함
        
        /* 1. 해당 아이템 타입의 딕셔너리가 있는겨우
         * 2. 해당 딕셔너리에 찾고자하는 아이템이 존재하는지?
         * 3. 해당 아이템의 재고가 1개 이상인지?
         */
        string itemID = item.ItemID.ToString();
        
        return _playerItemDictionary.ContainsKey(item.Itemtype) &&
               _playerItemDictionary[item.Itemtype].ContainsKey(itemID) && 
               _playerItemDictionary[item.Itemtype][itemID].ItemStock > 0;
    }

    
    /// <summary>
    /// 플레이어가 착용하고 있는 파츠에 대한 아이템 반환
    /// </summary>
    /// <param name="itemSlotType">해체하고자는 아이템 파츠(부위) 타입</param>
    /// <returns>현재 착용중인 아이템 반환</returns>
    public Equipment FilterEquipmentSlotByItem(Equipment.EquipmentSlotType itemSlotType)
    {
        if (_playerEquipmentSlots.ContainsKey(itemSlotType))
        {
            // 착용해체할 아이템 획득 후 반환
            Equipment unEquipmentItem = _playerEquipmentSlots[itemSlotType];
            return unEquipmentItem;
        }
        Debug.LogError($"{itemSlotType} 아이템 슬롯을 찾을 수 없습니다!");
        return null;
    }

    /// <summary>
    /// 플레이어 아이템 장착 해체
    /// </summary>
    /// <param name="unEquipmentItem">장착 해체할 아이템</param>
    public void UnEquipmentItemSlot(Equipment unEquipmentItem)
    {
        if (_playerEquipmentSlots.ContainsValue(unEquipmentItem))
        {
            // 해당 파츠는 null값으로 할당
            _playerEquipmentSlots[unEquipmentItem.EquipmentSlottype] = null;

            return;
        }
        
        Debug.LogError($"{unEquipmentItem.ItemName}의 아이템은 플레이어가 착용하지 않고 있습니다!");
    }
    
    /// <summary>
    /// 플레이어 장비 슬롯에 대한 장비장착
    /// </summary>
    /// <param name="item">장착할 장비</param>
    public void SetEquipmentSlotItem(Equipment item)
    {
        _playerEquipmentSlots[item.EquipmentSlottype] = item;
    }
    
    /// <summary>
    /// 플레이어 장비 슬롯에 대한 장비 착용유무 확인
    /// </summary>
    /// <param name="itemSlotType">착용 가능한 장비</param>
    /// <returns>해당 장비 슬롯에 장비 여부확인</returns>
    public bool ContainsEquipmentItemCheck(Equipment itemSlotType)
    {
        Equipment equipment = _playerEquipmentSlots[itemSlotType.EquipmentSlottype];
        // 해당 장비 슬롯에 장비가 있으면 true, 없으면 false 반환
        return equipment != null;
    }
    
    #endregion

}
