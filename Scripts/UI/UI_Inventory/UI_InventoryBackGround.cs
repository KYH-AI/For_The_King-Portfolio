using System;
using TMPro;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_InventoryBackGround : UI_Base
{
    private enum Texts
    {
        PlayerNameDisplay,
        StrengthText,
        VitalityText,
        IntelligenceText,
        AwarenessText,
        QuicknessText 
    }
    
    // 인벤토리 배경색상
    private enum Images
    {
        InventoryHeaderBackGround
    }

    // 인벤토리 닫기 버튼
    private enum Buttons
    {
        InventoryCloseButton
    }

    private enum UiInventory
    {
        Inventory
    }

    private enum UiStats
    {
        Stats
    }

    private WorldMapPlayerCharacter _lastInventoryDummy;
    
    // UIManager에서 초기화 실행
    public override void Init()
    {
        Bind<UI_Inventory>(typeof(UiInventory));
        Bind<UI_Stats>(typeof(UiStats));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        // 인벤토리 닫기 버튼 이벤트 등록
        Get<Button>((int)Buttons.InventoryCloseButton).onClick.AddListener(() => this.gameObject.SetActive(false));
        Get<UI_Inventory>((int)UiInventory.Inventory).Init();
        Get<UI_Stats>((int)UiStats.Stats).Init();
    }

    /// <summary>
    /// 인벤토리 또는 스텟상세 창 데이터 UI 업데이트
    /// </summary>
    /// <param name="requestPlayer">요청한 플레이어</param>
    /// <param name="isShow">UI 활성화 여부</param>
    public void InventoryUiUpdate<T>(PlayerStats requestPlayer, bool isShow)where T : Object, I_Inventory
    {
        /* 
         * 1. 플레이어 닉네임
         * 2. 플레이어 배너 색상
         * 3. 플레이어 능력치
         * 4. 플레이어 인벤토리 업데이트
         */
        if (requestPlayer != Managers.Inventory.RequestPlayer) return;

        Get<TextMeshProUGUI>((int)Texts.PlayerNameDisplay).text = requestPlayer.BaseClassName;

        Get<Image>((int)Images.InventoryHeaderBackGround).color =
            Managers.UIManager.GetPlayerHUD(requestPlayer).PlayerTitleColor;

        PlayerStats.PlayerMainHudDisplay playerInventoryStatDisplay = requestPlayer.GetStatDisplay(PlayerStats.StatType.Strength);
        Get<TextMeshProUGUI>((int)Texts.StrengthText).text = playerInventoryStatDisplay.Text;
        Get<TextMeshProUGUI>((int)Texts.StrengthText).color = playerInventoryStatDisplay.Color;
               
        playerInventoryStatDisplay = requestPlayer.GetStatDisplay(PlayerStats.StatType.Vitality);
        Get<TextMeshProUGUI>((int)Texts.VitalityText).text = playerInventoryStatDisplay.Text;
        Get<TextMeshProUGUI>((int)Texts.VitalityText).color = playerInventoryStatDisplay.Color;
               
        playerInventoryStatDisplay = requestPlayer.GetStatDisplay(PlayerStats.StatType.Intelligence);
        Get<TextMeshProUGUI>((int)Texts.IntelligenceText).text = playerInventoryStatDisplay.Text;
        Get<TextMeshProUGUI>((int)Texts.IntelligenceText).color = playerInventoryStatDisplay.Color;
               
        playerInventoryStatDisplay = requestPlayer.GetStatDisplay(PlayerStats.StatType.Awareness);
        Get<TextMeshProUGUI>((int)Texts.AwarenessText).text = playerInventoryStatDisplay.Text;
        Get<TextMeshProUGUI>((int)Texts.AwarenessText).color = playerInventoryStatDisplay.Color;
               
        playerInventoryStatDisplay = requestPlayer.GetStatDisplay(PlayerStats.StatType.Quickness);
        Get<TextMeshProUGUI>((int)Texts.QuicknessText).text = playerInventoryStatDisplay.Text;
        Get<TextMeshProUGUI>((int)Texts.QuicknessText).color = playerInventoryStatDisplay.Color;
        
        // 플레이어 인벤토리 정보 업데이트
        Get<T>(0).InventoryUiUpdate(requestPlayer);

        if (!isShow) return;
        
        // 인벤토리 Dummy 및 카메라 활성화
        if (requestPlayer.WorldMapCharacterDummy is WorldMapPlayerCharacter inventoryDummy)
        {
            inventoryDummy.ToggleDummyCharacter();
            _lastInventoryDummy = inventoryDummy;
        }
        
        // 인벤토리 UI 활성화 또는 비활성화
        ShowInventoryUI<T>(true);
        
    }

    /// <summary>
    /// 인벤토리 및 스텟창 UI 숨기기
    /// </summary>
    private void HideInventoryUI()
    {
        // 아이템 상호작용 팝업창 비활성화
        Managers.UIManager.SearchForClosePopUpUI(UIManager.PopUpUI.UI_ItemMenuButton);
        
        Get<UI_Inventory>((int)UiInventory.Inventory).gameObject.SetActive(false);
        Get<UI_Stats>((int)UiStats.Stats).gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        
        _lastInventoryDummy.ToggleDummyCharacter();
    }

    /// <summary>
    /// 인벤토리 및 스테창 UI 활성화 또는 비활성화
    /// </summary>
    private void ShowInventoryUI<T>(bool isShow)where T : Object, I_Inventory
    {
        // UI 인벤토리 백 그라운드 활성화
        this.gameObject.SetActive(isShow);

        // 재너릭 타입에 맞는 UI 활성화
        if (typeof(T) == typeof(UI_Inventory))
        {
            Get<UI_Inventory>(0).gameObject.SetActive(isShow);
        }
        else
        {
            Get<UI_Stats>(0).gameObject.SetActive(isShow);
        }
    }
    
    private void OnDisable()
    {
        HideInventoryUI();
    }
}
