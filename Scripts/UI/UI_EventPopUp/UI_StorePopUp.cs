using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI_StorePopUp : UI_PopUp
{
    public enum StoreType
    {
        WeaponStore = 0,
        ArmorStore = 1,
        ConsumableStore = 2,
        GoblinStore = 3,
        WorldMapEventStore = 4,
    }
    
    // 상점 아이템 목록 프리팹 원본
    private UI_StoreItemIconList _uiItemListPrefab;

    // UI에 표시할 아이템 목록 리스트(오브젝트 풀링)
    private List<UI_StoreItemIconList> _uiItemListGameObject = new List<UI_StoreItemIconList>(5);

    // 상점 컨셉에 맞는 상점 이름
    private readonly Dictionary<StoreType, string> _storeTitle = new Dictionary<StoreType, string>()
    {
        { StoreType.WeaponStore, "무기 상점" },
        { StoreType.ArmorStore, "방어구 상점" },
        { StoreType.ConsumableStore,"소모품 상점" },
        { StoreType.GoblinStore, "고블린 상점"},
        { StoreType.WorldMapEventStore, "떠돌이 상점"}
    };

    private enum Root
    {
        // 아이템 목록 리스트
        ItemGridRoot,
    }

    private enum Texts
    {
        // 현재 도시 이름
        CityNameText,
        // 현재 상점 이름
        StoreHeaderText,
        // 플레이어 소지 재화
        PlayerCurrentGoldText,
    }

    private enum Buttons
    {
        // 상점 닫기 버튼
        CloseButton
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Root));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        // 닫기 버튼 이벤트 등록
        Get<Button>((int)Buttons.CloseButton).onClick.AddListener(() =>
        {
            Managers.UIManager.SetQuestChart(true);
            // 쇼핑을 끝냈으니 연출에 사용된 3D 오브젝트  아이템 캐싱을 모두 초기화
            UI_MainItemCard.ItemObjectDictionary.Clear();

            // 상점 이용자 정보 해체
            Managers.Store.Customer = null;
            Managers.Inventory.SetInventoryItemTypeMenu(UI_ItemMenuButton.ItemMenuType.Default);
            
            Managers.Camera.SetWorldCamera();
            Managers.UIManager.RemoveForTargetClosePopUpUI(this, true);
        });
        
        _uiItemListPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemIconList/UI_StoreItemIconList")
            .GetOrAddComponent<UI_StoreItemIconList>();

    }

    /// <summary>
    /// 상점 UI 할당
    /// </summary>
    /// <param name="storeID">상점 ID</param>
    /// <param name="customer">상점 이용자</param>
    /// <param name="storeType">상점 타이틀</param>
    public void SetStoreInfo(int storeID, PlayerStats customer, StoreType storeType)
    {

        Store store = Managers.Store.GetStoreInfo(storeID);

        // 상점 정보 설정
        Managers.Store.CurrentStore = store;
        
        SetCityTitle(store.GetEventName());
        SetStoreTitle(storeType);
        UpdateItemList(store.GetItemInfo(storeType));
        UpdateStoreGoldText(customer.CurrentGold.ToString());
        
        // 상점 이용자 정보 설정
        Managers.Store.Customer = customer;
        Managers.Inventory.SetRequestPlayer(customer);
        Managers.Inventory.SetInventoryItemTypeMenu(UI_ItemMenuButton.ItemMenuType.Shop);

        /* 상점에 대한 모든 UI 설정
         * 1. 현재 도시 이름
         * 2. 현재 상점 이름
         * 3. 아이템 리스트 목록
         * 4. 플레이어 보유 골드 (유동적으로 변경 필요)
         */
        base.Init();
    }

    
    /// <summary>
    /// 현재 상점 도시 이름 설정
    /// </summary>
    /// <param name="cityTitle">도시 이름</param>
    private void SetCityTitle(string cityTitle)
    {
        Get<TextMeshProUGUI>((int)Texts.CityNameText).text = cityTitle;
    }

    /// <summary>
    /// 현재 상점 타이틀 이름 설정
    /// </summary>
    /// <param name="storeType">상점 컨셉</param>
    private void SetStoreTitle(StoreType storeType)
    {
        Get<TextMeshProUGUI>((int)Texts.StoreHeaderText).text = _storeTitle[storeType];
    }
    
    // 상점 아이템 목록 설정 (1회 호출이 아님)
    private void UpdateItemList(Item[] itemList)
    {
        if (itemList.Length > _uiItemListGameObject.Count)
        {
            CreateItemListUI(itemList.Length - _uiItemListGameObject.Count);
        }

        // 상점 아이템 리스트 표시
        int itemListIndex; // 아이템 리스트를 표시해야할 최대 개수
        for (itemListIndex = 0; itemListIndex < itemList.Length; itemListIndex++)
        {
            _uiItemListGameObject[itemListIndex].SetItemInfo(itemList[itemListIndex]);
            _uiItemListGameObject[itemListIndex].gameObject.SetActive(true);
        }

        //  상점 아이템 리스트 미표시 (i = 현재 표시할 아이템 인덱스 마지막 위치, 언제까지? 오브젝트 풀링 리스트에 남아있는 모든 리스트를 비활성화까지)
        for (int i = itemList.Length; i < _uiItemListGameObject.Count; i++)
        {
            // 아이템 리스트 오브젝트 비활성화
            _uiItemListGameObject[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 아이템 재고 업데이트 (플레이어가 구매 시 변경)
    /// </summary>
    /// <param name="targetItem">구매한 아이템</param>
    public void UpdateItemStock(Item targetItem)
    {
        foreach (var itemSlot in _uiItemListGameObject)
        {
            // 동일한 아이템이면서 아이템 리스트가 활성화 된 경우에만 재고 업데이트 진행
            if (itemSlot.Item == targetItem && itemSlot.gameObject.activeSelf)
            {
                // 상점 아이템 재고 감소
                itemSlot.UpdateItemStock(itemSlot.Item.ItemStock-1);
            }
        }
    }

    // 플레이어 현재 재화 상점애 표시
    public void UpdateStoreGoldText(string playerCurrentGold)
    {
        Get<TextMeshProUGUI>((int)Texts.PlayerCurrentGoldText).text = playerCurrentGold;
    }
    
    private void CreateItemListUI(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UI_StoreItemIconList uiItemList = Instantiate(_uiItemListPrefab, Get<GameObject>((int)Root.ItemGridRoot).transform);
            uiItemList.Init();
            uiItemList.gameObject.SetActive(false);
            _uiItemListGameObject.Add(uiItemList);
            
        }
    }

}
