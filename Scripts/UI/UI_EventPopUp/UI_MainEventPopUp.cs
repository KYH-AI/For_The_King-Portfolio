using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UI_MainEventPopUp : UI_PopUp
{
    public UI_EventPopUp ParentEventPopUpUI { get; private set; }

    // 현재 이벤트가 작동되는 Hex
    private Hex _hexInfo;
    
    // 이벤트 오브젝트 리스트
    private List<IEventInfo> _eventList;
    
    // 이벤트 오브젝트를 상호작용하는 플레이어 리스트
    private List<WorldMapPlayerCharacter> _playerList;
    
    // 마지막에 사용된 이벤트 오브젝트
    private GameObject _rendererObject = null;
    
    // 마지막에 사용된 버튼 그리드 오브젝트
    private UI_EventPopUpButtonGrid _lastButtonGrid = null;
    
    
    private readonly string EVENT_UI_PREFAB_PATH = "Prefabs/UI/EventObject/";
    
    
    private readonly string MAIN_EVENT_PREFAB_PATH = "Character/WorldMapEnemyCharacter/";

    #region  Hex에 대한 이벤트 핸들러
    
    public event Action CompleteHexEventHandler;        // 해당 Hex 이벤트를 클리어 시 (해당 이벤트를 완료할 경우 호출)
    
    public event Action DisableBattleOverEventHandler; // 떠나기 버튼을 눌렀을 Hex에 대한 Overlay 효과 삭제
    
    #endregion

    private enum Panels
    {
        PlayerPortraitRoot,      // 현재 이벤트를 이용하는 유저 슬롯 위치
        EnemyPortraitRoot,       // 현재 이벤트를 상대해야하는 적군 슬롯 위치
        SlotPanel,               // 이벤트에 필요하는 능력치 아이콘 위치
        ButtonPanel,             // 이벤트 상호작용하는 버튼 위치
        EventDetailIconRoot,     // 이벤트 특수사항 아이콘 위치
        EventLevelBackGround,
        EnemySwarmIcon,
        EnemyCampIcon,
        EventNoFocusIcon,
    }

    private enum Texts
    {
        EventNameText, // 이벤트 제목
        EventDetailText, // 이벤트 상세 설명
        EventDescriptionText, // 이벤트 스토리 
        EventLevelText // 이벤트 레벨 
    }

    // 플레이어와 이벤트 오브젝트 초상화 오브젝트
    private enum ObjectPortraits
    {
        UI_PlayerObjectPortrait_1,
        UI_PlayerObjectPortrait_2,
        UI_PlayerObjectPortrait_3,
        UI_EventObjectPortrait_1,
        UI_EventObjectPortrait_2,
        UI_EventObjectPortrait_3,
    }
    
    private enum BattleButtonGird
    {
        UI_BattleButtonGrid
    }

    private enum StoreButtonGird
    {
        UI_StoreButtonGrid
    }
    
    private enum CaveButtonGird
    {
        UI_CaveButtonGrid
    }
    
    private enum SanctuaryButtonGird
    {
        UI_SanctuaryButtonGrid
    }

    public void Init(UI_EventPopUp parentUIEventPopUp)
    {
        ParentEventPopUpUI = parentUIEventPopUp;
        Init();
    }
   
    public override void Init()
    {
        // 패널 설정
        Bind<GameObject>(typeof(Panels));
        // 텍스트 설정
        Bind<TextMeshProUGUI>(typeof(Texts));
        // 초상화 설정
        Bind<UI_EventUnitSlot>(typeof(ObjectPortraits));

        Bind<UI_BattleButtonGrid>(typeof(BattleButtonGird));
        
        Bind<UI_StoreButtonGrid>(typeof(StoreButtonGird));
        
        Bind<UI_CaveButtonGrid>(typeof(CaveButtonGird));
        
        Bind<UI_SanctuaryButtonGrid>(typeof(SanctuaryButtonGird));
        

        for (int i = 0; i < Enum.GetValues(typeof(ObjectPortraits)).Length; i++)
        {
            Get<UI_EventUnitSlot>(i).Init();
        }
        
        Get<UI_BattleButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);

        Get<UI_StoreButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);
            
        Get<UI_CaveButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);
        
        Get<UI_SanctuaryButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);
        
    }

    
    /// <summary>
    /// 변경되는 UI 요소에 알맞게 변경 ( 1회 호출이 아닌 매번 변경됨 )
    /// </summary>
    /// <param name="hexInfo">확인된 첫 번째 Hex 지형</param>
    /// <param name="eventObjects">참여할 이벤트 오브젝트 리스드</param>
    /// <param name="playerList">참여할 플레이어 리스트</param>
    public void GridInit(Hex hexInfo, List<IEventInfo> eventObjectList, List<WorldMapPlayerCharacter> playerList)
    {
        _hexInfo = hexInfo;
        _eventList = eventObjectList;
        _playerList = playerList;
        
        InitEventTexts(_eventList[0].GetEventName(), _eventList[0].GetEventDetailText(), _eventList[0].GetEventDescriptionText());
        InitUnitSlot();
        InitSlot();
        InitDetailSlot();
        InitButtons();
        SetEventMainObject(_eventList[0].GetEventID());
        
        // 현재 EventPopUp 게임오브젝트 활성화
        base.Init();
        
        
        /* <이벤트 UI 초기화 순서>
         *  1. 이벤트 이름
         *  2. 이벤트 대상 초상화 슬롯
         *  3. 이벤트 요구되는 능력치 슬롯
         *  4. 이벤트 버튼
         *  5. 이벤트 3D 오브젝트 Renderer Texture
         *  6. 현재 오브젝트 활성화
         */
    }
    

    private void InitEventTexts(string eventNameText, string eventDetailText, string eventStoryText)
    {
        Get<TextMeshProUGUI>((int)Texts.EventNameText).text = eventNameText;
        Get<TextMeshProUGUI>((int)Texts.EventDetailText).text = eventDetailText;
        Get<TextMeshProUGUI>((int)Texts.EventDescriptionText).text = eventStoryText;
    }

    private void InitUnitSlot()
    {
        int count = Enum.GetValues(typeof(ObjectPortraits)).Length;

        // 초상화 슬롯 모두 초기화
        for (int i = 0; i < count; i++)
        {
            Get<UI_EventUnitSlot>(i).gameObject.SetActive(false);
        }

        // 플레이어 초상화 할당
        for (int playerIndex = 0; playerIndex < _playerList.Count; playerIndex++)
        {
            Get<UI_EventUnitSlot>(playerIndex).SetEventObjectInformation(false, _playerList[playerIndex].PlayerStats.PlayerPortrait, _playerList[playerIndex].PlayerStats.BaseLevel);
            Get<UI_EventUnitSlot>(playerIndex).gameObject.SetActive(true);
        }

        // 전투를 제외한 이벤트는 적군 슬롯에 할당하지 않음 (해당 이벤트에 적군이 없다는 뜻)
        if (_hexInfo.BattleZoneType == BattleZoneType.None) return;
        
        // TODO : 히든 맴버일 경우 '?'로 표시

        bool isHidden = _hexInfo.BattleZoneType == BattleZoneType.Camp;

        int maxEnemyCount = Mathf.Min(3, _eventList.Count);
        // 적군 슬롯 할당
        for (int enemyIndex = 3; enemyIndex < maxEnemyCount + 3; enemyIndex++)
        {
            var portrait = !isHidden ? _eventList[enemyIndex - 3].GetEventPortrait() : null; 
            Get<UI_EventUnitSlot>(enemyIndex).SetEventObjectInformation(isHidden, portrait, _eventList[enemyIndex - 3].GetEventLevel());
            Get<UI_EventUnitSlot>(enemyIndex).gameObject.SetActive(true);
        }
        
    }

    private void InitSlot() 
    {

    }

    private void InitDetailSlot()
    {
        bool isNeedDetail = true;
        
        if (_hexInfo.HexType is HexType.Store or HexType.Sanctuary)
        {
            isNeedDetail = false; 
        }

        if (isNeedDetail)
        {
            Get<TextMeshProUGUI>((int)Texts.EventLevelText).text = _eventList[0].GetEventLevel().ToString();
        }
        else
        {
            foreach (Transform childDetailIcon in Get<GameObject>((int)Panels.EventDetailIconRoot).transform)
            {
                childDetailIcon.gameObject.SetActive(false);
            }
        }
        
  
        /*  전투, 광산 이벤트는 레벨 표시
         *  랜덤 발생 이벤트는 집중력 미사용 표시
         *  마을, 성소 이벤트는 아무것도 표시하지 않음
         *  전투캠프나 적군무리는 캠프 또는 무리 아이콘 표시
         */ 
        
        
    }
    private void InitButtons()
    {
        switch (_hexInfo.HexType)
        {
            case HexType.BattleZone : InitButtons<UI_BattleButtonGrid>(); break;
            case HexType.Store : InitButtons<UI_StoreButtonGrid>(); break;
            case HexType.Cave : InitButtons<UI_CaveButtonGrid>(); break;
            case HexType.Sanctuary : InitButtons<UI_SanctuaryButtonGrid>(); break;
            case HexType.QuestZone:
                if (_hexInfo.QuestZoneType is QuestZoneType.Battle) InitButtons<UI_BattleButtonGrid>();
                break;
            default: Debug.LogError($"{_hexInfo.HexType.ToString()}은 존재하지 않는 HexType 입니다!"); break;;
        }

    }
    
    private void InitButtons<T>() where T : UI_EventPopUpButtonGrid
    {
        // 마지막에 사용된 버튼 Grid가 있으면 비활성화
        if (_lastButtonGrid)
        {
            _lastButtonGrid.gameObject.SetActive(false);
        }

        _lastButtonGrid = Get<T>(0);
        _lastButtonGrid.SetRequesterWorldMapPlayCharacter(_playerList[0]);
        _lastButtonGrid.gameObject.SetActive(true);
        
    }

    // 이벤트 오브젝트 RendererTexture 진행
    private void SetEventMainObject(int mainObjectID)
    {
        
        // 기존에 있던 이벤트 오브젝트는 삭제
        ClearEventMainObject();
        

        string path = string.Empty;
        ResourceManager.ResourcePath resourcePath = 0;

        switch (_hexInfo.HexType)
        {
            case HexType.BattleZone :
                path = mainObjectID + "_World" + (DataManager.EnemyCharacter)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.WorldMapEnemy;
                break;
            
            case HexType.Store :
                path = mainObjectID + "_" + (DataManager.StoreID)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.Store;
                break;
            
            case HexType.Sanctuary :
                path = mainObjectID + "_" + (DataManager.SanctuaryID)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.Sanctuary;
                break;
            
            case HexType.Cave :
                path = mainObjectID + "_" + (DataManager.CaveID)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.Cave;
                break;
            
            case HexType.QuestZone:
                if (_hexInfo.QuestZoneType is QuestZoneType.Battle)
                {
                    path = mainObjectID + "_World" + (DataManager.EnemyCharacter)mainObjectID;
                    resourcePath = ResourceManager.ResourcePath.WorldMapEnemy;
                }
                break;

            default: Debug.LogError("적용할 수 없는 Renderer Texture 오브젝트 입니다!!");
                return;
   
        }

        GameObject newEventObject = Instantiate(Managers.Resource.LoadResource<GameObject>(resourcePath, path, false), 
            Managers.UIManager.UIEventObjectRendererTexturePos);

        newEventObject.GetComponentInChildren<Camera>().enabled = true;

        // 혹시 적군 오브젝트인 경우 "도발 애니메이션" 진행
        if (newEventObject.TryGetComponent(out WorldMapEnemyCharacter introTrigger))
        {
            introTrigger.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Taunt);
        }

           // 새로운 이벤트 오브젝트로 설정
        _rendererObject = newEventObject;
    }

    private void ClearEventMainObject()
    {
        if (!_rendererObject) return;
        Destroy(_rendererObject);
    }

    public void CompleteHexEvent()
    {
        CompleteHexEventHandler?.Invoke();
    }
    
    public void DisableHexBattleOver()
    {
        DisableBattleOverEventHandler?.Invoke();
    }

    /// <summary>
    /// 플레이어 오브젝트 데이터를 리스트에 넣음
    /// </summary>
    /// <returns>플레이어 오브젝트 리스트</returns>
    public List<PlayerStats> GetInteractionPlayerList()
    {
        List<PlayerStats> playerObjectList = new List<PlayerStats>();

        foreach (var playerList in _playerList)
        {
            playerObjectList.Add(playerList.PlayerStats);
        }
        return playerObjectList;
    }

    /// <summary>
    /// 이벤트 오브젝트 데이터를 리스트에 넣음
    /// </summary>
    /// <returns>이벤트 오브젝트 리스트</returns>
    public List<int> GetInteractionEventList()
    {
        List<int> eventObjectList = new List<int>();

        foreach (var eventList in _eventList)
        {
           eventObjectList.Add(eventList.GetEventID());
        }
        return eventObjectList;
    }

    /// <summary>
    /// 해당 Hex 지역에 퀘스트가 있는지 확인
    /// </summary>
    /// <returns></returns>
    public HexGrid.QuestHexInfo GetHexQuestID()
    {
        return UnitManager.Instacne.HexGrid.GetQuestHexID(_hexInfo);
    }


    public override void ClosedPopUpUI()
    {
        base.ClosedPopUpUI();
        
        // 전투를 제외한 경우는 제자리에 있으면 됨
        if (_hexInfo.BattleZoneType == BattleZoneType.None)
        {
            // 현재 이벤트를 취소한 플레이어의 상태를 취소 상태로 변경
            Managers.Turn.PlayerTurn.IsPlayerInEvent = false;
            // 현재 이벤트를 취소한 플레이어에게 다시 월드맵 이동기능을 부여
            UnitManager.Instacne.HandleUnitSelected(Managers.Turn.PlayerTurn.gameObject);
            return;
        }
        
        // 전투지역인 경우 전투범위 모두 제거
        DisableHexBattleOver();
    }
}


