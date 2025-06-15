using UnityEngine.UI;
using Utils;

public abstract class UI_EventPopUpButtonGrid : UI_Base
{
    protected UI_EventPopUp ParentUIPopUp;
    
    protected WorldMapPlayerCharacter WorldMapPlayerCharacter;
    
    /// <summary>
    /// 버튼 UI 컴포넌트 초기화 (1회 호출)
    /// </summary>
    /// <param name="uiEventPop">UI_EventPopUp (부모)</param>
    public void ButtonGridInit(UI_EventPopUp uiEventPop)
    {
        ParentUIPopUp = uiEventPop;
        Init();
    }

    /// <summary>
    /// 해당 이벤트 팝업을 뛰운 플레이어 캐릭터 저장함수
    /// </summary>
    /// <param name="requester">팝업을 뛰운 플레이어 캐릭터</param>
    public void SetRequesterWorldMapPlayCharacter(WorldMapPlayerCharacter requester)
    {
        this.WorldMapPlayerCharacter = requester;
    }

    protected void BindEventLeaveButton(int index)
    {
        Get<Button>(index).gameObject.BindEvent(data =>
        {
            // 이벤트 데이터 삭제와 전투지역 범위 Overlay 모두 비활성화
           // ParentUIPopUp.DisableHexBattleOver(); (! UI_EventPopUp.cs OnDisable()에서 호출함)
           // 종료 기능 (자식에서 오버라이딩한 함수를 호출)
            LeaveButtonEvent();
        }, MouseUIEvent.Click);
    }

    protected abstract void LeaveButtonEvent();
}
