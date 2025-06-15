using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI_StoreButtonGrid : UI_EventPopUpButtonGrid
{
    private enum Buttons
    {
        RestButton,
        WeaponStoreButton,
        ArmorStoreButton,
        UsedItemStoreButton,
        QuestButton,
        LeaveButton
    }
    
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        
        Get<Button>((int)Buttons.RestButton).gameObject.BindEvent(data => { RestButtonEvent(); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.WeaponStoreButton).gameObject.BindEvent(data => { StoreButtonEvent(UI_StorePopUp.StoreType.WeaponStore); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.ArmorStoreButton).gameObject.BindEvent(data => { StoreButtonEvent(UI_StorePopUp.StoreType.ArmorStore); }, MouseUIEvent.Click);
      //  Get<Button>((int)Buttons.UsedItemStoreButton).gameObject.BindEvent(data => { StoreButtonEvent(UI_StorePopUp.StoreType.ConsumableStore); }, MouseUIEvent.Click);
        Get<Button>((int)Buttons.QuestButton).gameObject.BindEvent(data => { QuestButtonEvent(); }, MouseUIEvent.Click);
        
        
        /*
        Get<Button>((int)Buttons.RestButton).onClick.AddListener(RestButtonEvent);
        Get<Button>((int)Buttons.WeaponStoreButton).onClick.AddListener(() => StoreButtonEvent(UI_StorePopUp.StoreType.WeaponStore));
        Get<Button>((int)Buttons.ArmorStoreButton).onClick.AddListener(() => StoreButtonEvent(UI_StorePopUp.StoreType.ArmorStore));
        Get<Button>((int)Buttons.UsedItemStoreButton).onClick.AddListener(() => StoreButtonEvent(UI_StorePopUp.StoreType.ConsumableStore));
        Get<Button>((int)Buttons.QuestButton).onClick.AddListener(QuestButtonEvent);
        */
        
        BindEventLeaveButton((int)Buttons.LeaveButton);
    }

    private void StoreButtonEvent(UI_StorePopUp.StoreType storeType)
    {
        ParentUIPopUp.UI_MainEventPopUp.gameObject.SetActive(false);
        Managers.UIManager.SetQuestChart(false);
        // 상점 버튼에 맞는 카메라 연출을 진행함 그리고 상점 UI 출력을 위한 이벤트 함수도 전달
        Managers.Camera.SetStoreCamera(storeType, () => ParentUIPopUp.UI_StorePopUp.SetStoreInfo(ParentUIPopUp.UI_MainEventPopUp.GetInteractionEventList()[0], 
                                                                                                        ParentUIPopUp.UI_MainEventPopUp.GetInteractionPlayerList()[0], 
                                                                                                        storeType));
    }

    private void RestButtonEvent()
    {
        
    }

    private void QuestButtonEvent()
    {
        ParentUIPopUp.UI_Quest.ShowQuestBoard();
        Managers.Quest.player = Managers.Turn.PlayerTurn.PlayerStats;
    }

    protected override void LeaveButtonEvent()
    {
        ParentUIPopUp.UI_MainEventPopUp.ClosedPopUpUI();
    }
}
