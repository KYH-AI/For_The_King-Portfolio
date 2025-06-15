using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{

    // 현재 상점 또는 인벤토리 이용자 정보
    public PlayerStats RequestPlayer { get; private set; }
    
    // 현재 플레이어 인벤토리 상태 (상점, 평상시 구분용도)
    public UI_ItemMenuButton.ItemMenuType InventoryItemMenuType { get; private set; }

    /// <summary>
    /// 현재 인벤토리 이용자 정보 할당
    /// </summary>
    /// <param name="requestPlayer">요청한 플레이어</param>
    public void SetRequestPlayer(PlayerStats requestPlayer)
    {
        this.RequestPlayer = requestPlayer;
    }
    
    /// <summary>
    /// 현재 인벤토리 요구 버튼 UI 정보 할당
    /// </summary>
    /// <param name="itemMenuType">현재 인벤토리 상태</param>
    public void SetInventoryItemTypeMenu(UI_ItemMenuButton.ItemMenuType itemMenuType)
    {
        this.InventoryItemMenuType = itemMenuType;
    }
    
    
    /// <summary>
    /// 플레이어 인벤토리에 아이템 추가
    /// </summary>
    /// <param name="requesterPlayer">요청한 플레이어</param>
    /// <param name="item">추가할 아이템</param>
    public void AddInventoryItem(PlayerStats requesterPlayer, Item item)
    {
        Dictionary<string, Item> itemList = requesterPlayer.PlayerInventory.FilterItemsByType(item.Itemtype);
        string itemID = item.ItemID.ToString();
        
        // 해당 인벤토리에 중복되는 아이템이 없는경우 새로추가
        if (!itemList.ContainsKey(itemID))
        {
            itemList.Add(itemID, item);
        }
        // 중복되는 아이템이 있는경우 아이템의 재고만 +1 증가
        else
        {
            if (itemList.TryGetValue(itemID, out Item existingItem))
            {
                // 인벤토리를 확인해 기존아이템에 재고를 +1 증가
                existingItem.ItemStock++;
            }
        }
        
        // 아이템이 추가되었으니 UI_Inventory에서 업데이트 필요
        Managers.UIManager.PlayerInventoryUiUpdate<UI_Inventory>(requesterPlayer);
    }

    /// <summary>
    /// 플레이어 인벤토리에 아이템 제거
    /// </summary>
    /// <param name="requesterPlayer">요청한 플레이어</param>
    /// <param name="item">제거할 아이템</param>
    public void RemoveInventoryItem(PlayerStats requesterPlayer, Item item)
    {
   
        if (requesterPlayer.PlayerInventory.ContainsInventoryItemCheck(item))
        {
            Dictionary<string, Item> itemList = requesterPlayer.PlayerInventory.FilterItemsByType(item.Itemtype);
            string itemID = item.ItemID.ToString();

            // 아이템 재고가 1개 초과인경우 해당 아이템 1개만 제거
            if (itemList[itemID].ItemStock > 1)
            {
                itemList[itemID].ItemStock--;
            }
            // 아이템 재고가 1개인 경우 해당 아이템 모두 제거
            else
            {
                itemList.Remove(itemID);
            }

        }
        // 예외처리
        // 지우고자는 아이템이 없는 경우 Error Log 출력
        else
        {
            Debug.LogError($"해당 아이템(이름 : {item.ItemName}, 코드 : {item.ItemID})이 존재하지 않아 인벤토리에서 아이템을 제거할 수 없음!");
        }
        
        // 아이템이 제거되었으니 UI_Inventory에서 업데이트 필요
        Managers.UIManager.PlayerInventoryUiUpdate<UI_Inventory>(requesterPlayer);
    }

    /// <summary>
    /// 플레이어 아이템 장착
    /// </summary>
    /// <param name="requesterPlayer">요청한 플레이어</param>
    /// <param name="item">장착할 아이템</param>
    public void EquipmentItem(PlayerStats requesterPlayer, Equipment item)
    {
        /* 1. 무기
        * 2. 방패
        * 3. 방어구
        * 4. 헬멧
        * 5. 신발
        * 6. 반지
        * 7. 목걸이
        */

        // 이미 해당 파츠에 장비가 장착되어 있는 경우
        if (requesterPlayer.PlayerInventory.ContainsEquipmentItemCheck(item))
        {
            // 장비교체 (기존에 착용중인 장비를 먼저 해체하고 인벤토리로 이동)
            UnEquipmentItem(requesterPlayer, item.EquipmentSlottype);
        }
        
        // 인벤토리에 있는 장비를 해당 파츠에 올림
        RemoveInventoryItem(requesterPlayer, item);
        
        // 장비착용
        requesterPlayer.PlayerInventory.SetEquipmentSlotItem(item);
        
        // 아이템에 의해서 플레이어 스텟 변경
        requesterPlayer.ItemForUpdateModifyStat(item, true);

        // 아이템을 장착했으니 UI_Inventory에서 업데이트 필요
        Managers.UIManager.PlayerInventoryUiUpdate<UI_Inventory>(requesterPlayer);
    }

    /// <summary>
    /// 플레이어 아이템 장착해체
    /// </summary>
    /// <param name="requesterPlayer">요청한 플레이어</param>
    /// <param name="unEquipmentItemSlotType">장착해체할 아이템 파츠(부위) 타입</param>
    public void UnEquipmentItem(PlayerStats requesterPlayer, Equipment.EquipmentSlotType unEquipmentItemSlotType)
    {
        // 먼저 장비해체 진행
        Equipment unEquipmentItem = requesterPlayer.PlayerInventory.FilterEquipmentSlotByItem(unEquipmentItemSlotType);
        
        requesterPlayer.PlayerInventory.UnEquipmentItemSlot(unEquipmentItem);
        
        // 해채된 장비는 다시 인벤토리로 이동
        AddInventoryItem(requesterPlayer, unEquipmentItem);
        
        // 아이템의 의해서 플레이어 스텟 변경
        requesterPlayer.ItemForUpdateModifyStat(unEquipmentItem, false);
        
        // 아이템을 장착해체했으니 UI_Inventory에서 업데이트 필요
        Managers.UIManager.PlayerInventoryUiUpdate<UI_Inventory>(requesterPlayer);
    }

    /// <summary>
    /// 인벤토리 아이템 리스트 반환
    /// </summary>
    /// <param name="requesterPlayer">요청한 플레이어</param>
    /// <param name="itemFilter">아이템 필터링 타입</param>
    /// <returns>해당 필터링에 맞는 아이템 리스트 반환</returns>
    public List<Item> FilterInventoryItems(PlayerInventory requesterPlayer, Item.ItemType itemFilter)
    {
        // 필터링 타입을 NONE일 경우 모든 아이템 리스트를 반환
        if (itemFilter is Item.ItemType.NONE)
        {
            return requesterPlayer.NoneFilterItemsByType();
        }
        
        // 필터링 타입이 있으면 해당 아이템 리스트만 반환
        Dictionary<string, Item> itemList = requesterPlayer.FilterItemsByType(itemFilter);

        List<Item> filteredItems = new List<Item>(itemList.Values);

         return filteredItems;
    }

    /// <summary>
    /// 인벤토라 아이템 필터별로 개수 반환
    /// </summary>
    /// <param name="requesterPlayer">요청한 플레이어</param>
    /// <returns>필터별 아이템 개수가 들어있는 배열반환</returns>
    public int[] FilterInventoryItemCount(PlayerInventory requesterPlayer)
    {
         /* (0)        (1)   (2)    (3)    (4)     (5)
        /* 모든 아이템, 무기, 방어구, 악세서리, 소모품, 퀘스트템 (골드는 제외 -1) */
        int[] itemFilterCount = new int[Enum.GetValues(typeof(Item.ItemType)).Length - 1];

        // (1) 인덱스부터 시작 
        for (int i = 1; i < itemFilterCount.Length; i++)
        {

            var itemList = requesterPlayer.FilterItemsByType((Item.ItemType)(i - 1));
            foreach (var item in itemList.Values)
            {
                itemFilterCount[i] += item.ItemStock;
            }
            
            // (0) 인덱스에 1~5 인덱스까지의 합을 저장함
            itemFilterCount[0] += itemFilterCount[i];
        }
        
        return itemFilterCount;
    }

    /// <summary>
    /// 플레이어가 착용중인 아이템에 대한 정보를 반환
    /// </summary>
    /// <param name="requestPlayer">요청한 플레이어</param>
    /// <returns>[0] ~ [6] 까지의 플레이어 부위별로 장착된 아이템 반환</returns>
    public Equipment[] SlotEquipmentsItem(PlayerInventory requestPlayer)
    {
        Equipment[] equipments = new Equipment[Enum.GetValues(typeof(Equipment.EquipmentSlotType)).Length - 1];

        for (int i = 0; i < equipments.Length; i++)
        {
            equipments[i] = requestPlayer.FilterEquipmentSlotByItem((Equipment.EquipmentSlotType)i);
        }

        return equipments;

    }

    
}
