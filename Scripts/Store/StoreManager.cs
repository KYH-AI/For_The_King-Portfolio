using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoreManager
{
    //마을 상점 배열
    private Store[] _stores; 
    
    //고블린 상점 (1회성)
    private Store _goblinStore;

    //현재 활성화된 상점
    public Store CurrentStore { get; set; }
    
    //상점 이용자
    public PlayerStats Customer { get; set; }
        
    /// <summary>
    /// 상점 초기화
    /// </summary>
    public StoreManager()
    {
        // 상점 객체 생성
        
        _stores = new[]
        {
            new Store(0, "카람바", "마을에서 휴식과 장비를 재구성하세요", "기본에 충실한 마을입니다"),
            new Store(1, "아르케온", "마을에서 휴식과 장비를 재구성하세요", "보잘것 없는 초심자의 마을로 보인다"),
            new Store(2, "노바시티", "마을에서 휴식과 장비를 재구성하세요", "산업이 시작된 첫 번째 마을"), 
            new Store(3, "네오시티", "마을에서 휴식과 장비를 재구성하세요", "마법공학의 시작된 첫 번째 마을"), 
            new Store(4, "코랄리스", "마을에서 휴식과 장비를 재구성하세요", "장의사가 매우 필요해 보인다"),
            new Store(5, "블루워드", "마을에서 휴식과 장비를 재구성하세요", "주민들의 성향이 온화하며 많은 인원들이 휴양지로서도 찾기도 한다"),
            new Store(6, "스카이하이트", "마을에서 휴식과 장비를 재구성하세요", "주민들의 성향이 온화하며 많은 인원들이 휴양지로서도 찾기도 한다")
        };

        _goblinStore = new Store(7, "고블린 상점", "신기한 장비 판매점", "쉽게 찾을 수 없는 상점이다.");
    }
    
    public Store GetStoreInfo(int storeID)
    {
        return _stores[storeID];
    }

    public int GetStoreLength()
    {
        return _stores.Length;
    }
    
    
    #region 상점에 아이템 Setter
    
    /// <summary>
    /// 상점 리스트에 Item을 뿌려줌
    /// </summary>
    public void SetNewItemToStoreList()
    {
        /* 1. 무기 리스트
         * 2. 방어구 리스트
         * 3. 소모품 리스트
         */

        Item.ItemType itemType;
        
        // 모든 상점에 새로운 아이템을 정보를 전달함
        for (int storeID = 0; storeID < _stores.Length; storeID++)
        {
            //itemType = Item.ItemType.Weapon;
           // _stores[storeID].SetNewItemList(itemType, GetItemList(itemType));
           
            /*
            for (int itemListIndex = 0; itemListIndex < (int)Item.ItemType.Count; itemListIndex++)
            {
                itemType = (Item.ItemType)itemListIndex;
                _stores[storeID].SetNewItemList(itemType, GetItemList(itemType));
            }
            */
            
            //  (23/06/34 현재 무기 데이터랑 방어구데 이터만 준비된 관계로 두개의 상점만 아이템 셋팅
            _stores[storeID].SetNewItemList(Item.ItemType.Weapon, SetItemList(Item.ItemType.Weapon));
            _stores[storeID].SetNewItemList(Item.ItemType.Armor, SetItemList(Item.ItemType.Armor));
          //  _stores[storeID].SetAddItemList(Item.ItemType.Armor, SetItemList(Item.ItemType.Accessory));
           // _stores[storeID].SetNewItemList(Item.ItemType.Consumable, SetItemList(Item.ItemType.Consumable));
            
        }
        
    }

    private Item[] SetItemList(Item.ItemType itemType)
    {
        int maxStock = 1;
        
        // 아이템 재고 설정을 위해 갑옷 그리고 무기, 악세서리는 최대 재고를 1로 고정
        if (itemType != Item.ItemType.Armor && itemType != Item.ItemType.Weapon && itemType != Item.ItemType.Accessory)
        {
            maxStock = Random.Range(1, 6);
        }
        
        // 아이템 총 품목 (재고를 의미하는게 아님)
        int totalItemCount = Random.Range(1, 4);
        
        // 각 아이템 재고 설정
        int itemStock = Random.Range(1, maxStock);
        
        // 아이템 리스트
        Item[] itemList = new Item[totalItemCount];
        
        // 아이템 ID 랜덤 중복 값 확인
        HashSet<int> selectedIDs = new HashSet<int>();

        // 프로트타입 Test 용도
        int minID = itemType == Item.ItemType.Weapon ? 1 : 0;
        int maxID = itemType == Item.ItemType.Weapon ? 4 : 5;    
            
         int index = 0;
        while (totalItemCount > index)
        {
            // 아이템 ID
            int id;
            do
            {
                // TODO : TEST 용도로 아이템 ID 랜덤으로 가져옴 (마을 ID에 맞게 범위를 정해야함)
                id = Random.Range(minID, maxID);
                // 동일한 ID인 경우 다시 반복
            } while (selectedIDs.Contains(id));

            // ID 중복값을 막기 위해 ID 추가
            selectedIDs.Add(id);

            // 아이템 정보를 가져옴
            Item item =  Managers.Data.GetItemInfo(itemType, id);
            item.ItemStock = itemStock;

            itemList[index] = item;
            index++;
        }

        // 아이템 리스트 반환
        return itemList; 
    }

    /// <summary>
    /// 특정 상점에 아이템 유무 확인
    /// </summary>
    /// <param name="targetItem"></param>
    private bool CheckStoreItem(Store store, Item targetItem, ref Item shopItem)
    {
        shopItem = null;
        Item[] findItems = null;
        
        switch (targetItem.Itemtype)
        {
            case Item.ItemType.Weapon:
                findItems = store.GetItemInfo(UI_StorePopUp.StoreType.WeaponStore);
                break;
            
            case Item.ItemType.Armor:
            case Item.ItemType.Accessory:
                findItems = store.GetItemInfo(UI_StorePopUp.StoreType.ArmorStore);
                break;

            case Item.ItemType.Consumable:
                findItems = store.GetItemInfo(UI_StorePopUp.StoreType.ConsumableStore);
                break;
        }

        if (findItems == null) return false;

        foreach (var item in findItems)
        {
            // 찾은 아이템이 존재하면 상점 아이템 정보를 반환
            if (targetItem == item)
            {
                shopItem = targetItem;
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// 아이템 구매
    /// </summary>
    /// <param name="requestPlayer">구매자</param>
    /// <param name="buyItem">구매 아이템</param>
    public void BuyItem(Store store, PlayerStats requestPlayer, Item buyItem)
    {
        // 상점 아이템 정보
        Item shopItem = null;
        
        // 3가지 판단 (1. 플레이어 재화, 2. 상점에 아이템 확인, 3. 상점 아이템 재고 확인)
        if (buyItem.BuyPrice <= requestPlayer.CurrentGold && !CheckStoreItem(store, buyItem, ref shopItem) && shopItem.ItemStock >= 1)
        {
            return;
        }
        
        requestPlayer.UpdateGold(buyItem.BuyPrice, false);
        // 상점 플레이어 재화 UI 업데이트
        Managers.UIManager.EventPopUp.UI_StorePopUp.UpdateStoreGoldText(requestPlayer.CurrentGold.ToString());
        Managers.Inventory.AddInventoryItem(requestPlayer, buyItem);
        // 상점 아이템 재고 정보 업데이트
        Managers.UIManager.EventPopUp.UI_StorePopUp.UpdateItemStock(shopItem);
    }
    
    
    /// <summary>
    /// 아이템 판매
    /// </summary>
    /// <param name="requestPlayer">판매자</param>
    /// <param name="sellItem">판매 아이템</param>
    public void SellItem(PlayerStats requestPlayer, Item sellItem)
    {
        if (sellItem == null) return;

        requestPlayer.UpdateGold(sellItem.SellPrice, true);
        // 상점 플레이어 재화 UI 업데이트
        Managers.UIManager.EventPopUp.UI_StorePopUp.UpdateStoreGoldText(requestPlayer.CurrentGold.ToString());
        Managers.Inventory.RemoveInventoryItem(requestPlayer, sellItem);

    }

    #endregion

}
