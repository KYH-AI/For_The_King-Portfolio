using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UI_EventPopUp : UI_Scene
{
    
    public UI_MainEventPopUp UI_MainEventPopUp => Get<UI_MainEventPopUp>(0);
    
    public UI_Quest UI_Quest => Get<UI_Quest>(0);

    public UI_StorePopUp UI_StorePopUp => Get<UI_StorePopUp>(0);
    
    private enum MainEventPanel
    {
        UI_MainEventPopUp
    }
    
    private enum Store
    {
        UI_Store
    }
    
    private enum RestService
    {
        UI_RestService
    }
    
    private enum Quest
    {
        UI_Quest
    }
    

    public override void Init()
    {

        Bind<UI_MainEventPopUp>(typeof(MainEventPanel));
        
        Bind<UI_Quest>(typeof(Quest));

        Bind<UI_StorePopUp>(typeof(Store));

        Get<UI_MainEventPopUp>(0).Init(this);

        Get<UI_Quest>(0).Init();
        
        Get<UI_StorePopUp>(0).Init();
    }


    /// <summary>
    /// 변경되는 UI 요소에 알맞게 변경 ( 1회 호출이 아닌 매번 변경됨 )
    /// </summary>
    /// <param name="hexInfo">확인된 첫 번째 Hex 지형</param>
    /// <param name="eventObjects">참여할 이벤트 오브젝트 리스드</param>
    /// <param name="playerList">참여할 플레이어 리스트</param>
    public void GridInit(Hex hexInfo, List<IEventInfo> eventObjectList, List<WorldMapPlayerCharacter> playerList)
    {
        Get<UI_MainEventPopUp>((int)MainEventPanel.UI_MainEventPopUp).GridInit(hexInfo, eventObjectList, playerList);
    }

    
}


