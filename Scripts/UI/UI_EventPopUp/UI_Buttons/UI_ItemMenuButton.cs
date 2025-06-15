using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemMenuButton : UI_PopUp
{
    /*          <아이템 상호작용 UI>
        1. 상점에서 아이템 클릭 시 버튼 내역
            1. 구매
            2. 구매 및 착용
            3. 닫기

        2. 상점에서 인벤토리 아이템 클릭 시 버튼 내역
            1. 사용
            2. 장착
            3. 판매 (골드 가격 text) 골드 아이콘
            4. 주기 플레이어 초상화 (현재 플레이어 최대 명수 만큼)
            5. 닫기

        3. 비전투에서 인벤토리 아이템 클릭 시 버튼 내역
            1. 사용
            2. 장착
            3. 판매 (비활성화)
            4. 주기 플레이어 초상화 (현재 플레이어 최대 명수 만큼)
            5. 닫기
     */

    private enum Buttons
    {
        UseButton,   // 아이템 사용 버튼
        UnEquipButton, // 장착 해체 버튼
        EquipButton, // 아이템 착용 버튼
        ShellButton, // 판매 버튼
        SendButton1, // 플레이어 1에게 아이템 전달 버튼
        SendButton2, // 플레이어 2에게 아이템 전달 버튼
        BuyButton,   // 구매 버튼
        BuyAndEquipButton,  // 구매 및 착용 버튼
        CloseButton         // 닫기 버튼
    }

    private enum ButtonText
    {
        ShellButtonText
    }


    public enum ItemMenuType
    {
        NONE = -1,
        Default = 0,     // 평상시
        Shop = 1,       // 플레이어 측(상점 이용시)
        Shopping = 2,   // 상점 측
        LoadOut = 3,    // 플레이어 로드아웃 (장비 슬롯)
    }

    private ItemMenuType _lastType = ItemMenuType.NONE;

    // 플레이어가 선택한 아이템 정보
    public Item SelectItem { get; private set; }

    // 장착한 아이템인 경우
    private Equipment _equipmentItem;

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(ButtonText));

        // 닫기 버튼 이벤트
        Get<Button>((int)Buttons.CloseButton).onClick.AddListener(base.ClosedPopUpUI);

        // 사용 버튼 이벤트
        //Get<Button>((int)Buttons.UseButton).onClick.AddListener();

        // 장비 해체 버튼 이벤트
        Get<Button>((int)Buttons.UnEquipButton).onClick.AddListener(() =>
        {
            Managers.Inventory.UnEquipmentItem(Managers.Inventory.RequestPlayer, _equipmentItem.EquipmentSlottype);
            base.ClosedPopUpUI();
        });
        
        // 구매 버튼 이벤트
        Get<Button>((int)Buttons.BuyButton).onClick.AddListener(() => 
        { 
            Managers.Store.BuyItem(Managers.Store.CurrentStore, Managers.Store.Customer, SelectItem);
            base.ClosedPopUpUI();
        });

        // 판매 버튼 이벤트
        Get<Button>((int)Buttons.ShellButton).onClick.AddListener(() =>
        {
            Managers.Store.SellItem(Managers.Store.Customer, SelectItem);
            base.ClosedPopUpUI();
        });
        
         // 장비 착용 버튼 이벤트
        Get<Button>((int)Buttons.EquipButton).onClick.AddListener( ()=>
        {
            Managers.Inventory.EquipmentItem(Managers.Inventory.RequestPlayer, (Equipment)SelectItem);
            base.ClosedPopUpUI();
        });
        
        // 구매 후 착용 버튼 이벤트
        Get<Button>((int)Buttons.BuyAndEquipButton).onClick.AddListener(() =>
        {
            Managers.Store.BuyItem(Managers.Store.CurrentStore, Managers.Store.Customer, SelectItem);
            Managers.Inventory.EquipmentItem(Managers.Store.Customer, (Equipment)SelectItem);
            base.ClosedPopUpUI();
        });
    }

    public void ActiveItemMenuUI(PlayerStats requestPlayer, Item item, ItemMenuType itemMenuType)
    {
        // 이미 아이템 상호작용 버튼 UI가 활성화 된 경우는 비활성화하고 진행
        if(this.gameObject.activeSelf) base.ClosedPopUpUI();
        
        // 선택한 아이템 정보 할당
        SelectItem = item;
        
        // 이미 착용한 아이템인 경우를 위해 확인
        if (item is Equipment equipment)
        {
            _equipmentItem = equipment;
        }
        
        // 현재 상점을 이용중인 플레이어 != 인벤토리 UI를 요청한 플레이어 = 아이템 판매버튼 상호작용 비활성화
        if (itemMenuType is ItemMenuType.Shop && Managers.Store.Customer != requestPlayer) 
            Get<Button>((int)Buttons.ShellButton).interactable = false;
        else if(itemMenuType is ItemMenuType.Shop)
            Get<Button>((int)Buttons.ShellButton).interactable = true;
        
        // 상점 아이템 구매버튼 확인
        if (itemMenuType is ItemMenuType.Shopping)
        {
            // 현재 플레이어 재화로 아이템이 구매가능한 경우 구매 버튼 활성화
            Get<Button>((int)Buttons.BuyAndEquipButton).interactable = Get<Button>((int)Buttons.BuyButton).interactable = Managers.Store.Customer.CurrentGold >= item.BuyPrice;
        }


        // 판매 가격 UI Text 표시
        Get<TextMeshProUGUI>((int)ButtonText.ShellButtonText).text = "판매 (" + SelectItem.SellPrice + ")";
        
        if (itemMenuType == _lastType)
        {
            // 인벤토리 상태에 맞는 버튼 활성화 동작 스킵
            base.Init();
            return;
        }
        
        switch (itemMenuType)
        {
            case ItemMenuType.Default :
                DisableButtons(Buttons.ShellButton, Buttons.BuyButton, Buttons.BuyAndEquipButton, Buttons.UnEquipButton);
                EnableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.SendButton1, Buttons.SendButton2);
                break;
            
            case ItemMenuType.LoadOut :
                DisableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.ShellButton, 
                                Buttons.SendButton1, Buttons.SendButton2, Buttons.BuyButton, Buttons.BuyAndEquipButton);
                EnableButtons(Buttons.UnEquipButton);
                break;
            
            case ItemMenuType.Shop :
                DisableButtons(Buttons.BuyButton, Buttons.BuyAndEquipButton, Buttons.UnEquipButton);
                EnableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.ShellButton,  
                               Buttons.SendButton1, Buttons.SendButton2);
                break;
            
            case ItemMenuType.Shopping :
                DisableButtons(Buttons.UseButton, Buttons.EquipButton, Buttons.ShellButton, 
                               Buttons.SendButton1, Buttons.SendButton2, Buttons.UnEquipButton);
                EnableButtons(Buttons.BuyButton, Buttons.BuyAndEquipButton);
                break;
            
        }

        _lastType = itemMenuType;
        
        // 버튼 UI 활성화
        base.Init();
      
    }

    // 버튼 비활성화
    private void DisableButtons(params Buttons[] disableButtons)
    {
        foreach (var button in disableButtons)
        {
            Get<Button>((int)button).gameObject.SetActive(false);
        }
    }

    // 버튼 활성화
    private void EnableButtons(params Buttons[] enableButtons)
    {
        foreach (var button in enableButtons)
        {
            Get<Button>((int)button).gameObject.SetActive(true);
        }
    }
    

}
