using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Store : WorldMapEventInfo
{
    public int StoreID { get; private set; }
    
    /*  1. 무기 리스트
    *   2. 방어구 리스트
    *   3. 소모품 리스트
    */
    
    private List<Item[]> _itemsList = new List<Item[]>()
    {
        new Item[5],
        new Item[5],
        new Item[5],
    };
    
    public Store(int storeID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null) 
        : base(storeID, eventName, eventDetailText, eventDescriptionText, eventLevel, eventPortrait)
    {
        StoreID = storeID;
    }
    
    
    /// <summary>
    /// 새로운 아이템으로 교체
    /// </summary>
    /// <param name="itemType">아이템 타입</param>
    /// <param name="itemList">교체될 아이템 배열</param>
    public void SetNewItemList(Item.ItemType itemType, Item[] itemList)
    {
        _itemsList[(int)itemType] = itemList;
    }

    /// <summary>
    /// 새로운 아이템 추가 등록
    /// </summary>
    /// <param name="itemType">아이템 타입</param>
    /// <param name="itemList">추가될 아이템 배열</param>
    public void SetAddItemList(Item.ItemType itemType, Item[] itemList)
    {
        _itemsList[(int)itemType] = (Item[])_itemsList[(int)itemType].Concat(itemList);
    }

    /// <summary>
    /// 상점 UI에 표시할 아이템 목록 ID
    /// </summary>
    /// <param name="storeType">상점 특징</param>
    /// <returns>아이템 목록 ID</returns>
    public Item[] GetItemInfo(UI_StorePopUp.StoreType storeType)
    {
        return _itemsList[(int)storeType];
    }

}
