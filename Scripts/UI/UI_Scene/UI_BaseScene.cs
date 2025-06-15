using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class UI_BaseScene : UI_Scene
{

    #region 타임라인 Grid 관련
    
    private UI_TimeLine _currentTimeLineObject;

    private readonly Dictionary<Type, UI_TimeLine> _uiTimeLines = new Dictionary<Type, UI_TimeLine>();

    // 월드맵 타임라인 캐싱
    private UI_WorldMapTimeLineGrid _worldMapTimeLine;
    // 전투 타임라인 캐싱
    private UI_BattleTimeLineGrid _battleTimeLine;
    // 광산 타임라인 캐싱
    private UI_CaveTImeLineGird _caveTimeLine;
    #endregion


    #region Player HUD 딕셔너리

    private readonly Dictionary<PlayerStats, UI_PlayerMainHUD> _playerMainHudDictionary = new Dictionary<PlayerStats, UI_PlayerMainHUD>();

    #endregion
    
    private enum Buttons
    {
        TurnOverButton // 턴 종료 버튼
    }

    private enum Panels
    {
        TopPanel,       // 상체 Time Line UI 구간  
        TimeLineSlot,  // Time Line UI가 배치될 위치 ( World Map, 광산, 전투 3가지의 Time Line으로 나뉨 )
        BottomPanel,    // 하체 Player Stat UI 구간
        PlayerHUDSlot   // Player HUD UI 배치 구간
    }

    private enum Texts
    {
        CurrentPlayerTurnText // 현재 어떤 Player 차례인지 알려주는 Text
    }
    
    private enum QuestChart
    {
        UI_QuestChart,
    }
    private enum Option
    {
        UI_Option,
    }
    
    // 디버그 테스트용도 색상
    // 플레이어 HUD 타이틀 색상
    public readonly Color[] PLAYER_TITLE_COLOR = new Color[3]
    {
        new Color(202f / 255f, 83f / 255f, 83f / 255f),
        new Color(56f / 255f, 173f / 255f, 42f / 255f),
        new Color(73f / 255f, 119f / 255f, 204f / 255f),
    };

    /// <summary>
    /// 1회 호출 초기화
    /// </summary>
    public override void Init()
    {

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(Panels));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<UI_QuestChart>(typeof(QuestChart));
        Bind<UI_Option>(typeof(Option));
        Get<UI_Option>(0).Init();
        // 월드 맵 타임라인 캐싱
        _worldMapTimeLine = Get<GameObject>((int)Panels.TimeLineSlot).GetComponentInChildren<UI_WorldMapTimeLineGrid>(true);
        _battleTimeLine =  Get<GameObject>((int)Panels.TimeLineSlot).GetComponentInChildren<UI_BattleTimeLineGrid>(true);
        _caveTimeLine = Get<GameObject>((int)Panels.TimeLineSlot).GetComponentInChildren<UI_CaveTImeLineGird>(true);

        // 월드 맵 타임라인 캐싱
        AddTimeLineGrid<UI_WorldMapTimeLineGrid>(_worldMapTimeLine);
        // 전투 타임라인 캐싱
        AddTimeLineGrid<UI_BattleTimeLineGrid>(_battleTimeLine);
        // 던전 타임라인 캐싱
        AddTimeLineGrid<UI_CaveTImeLineGird>(_caveTimeLine);
        // 광산 타임라인 캐싱
        // AddTimeLineGrid<UI_CaveMapTimeLineGrid>(_caveMapTimeLine);

        _worldMapTimeLine.Init();
      
       // 플레이어 HUD UI 초기화
       for (int i = 0; i < Managers.Instance.GetPlayerCount; i++)
       {
           var player = Managers.Instance.GetPlayer(i);
           _playerMainHudDictionary[player.PlayerStats] = Instantiate(Resources.Load<GameObject>("Prefabs/UI/HUD/UI_PlayerMainHUD"),
                                             Get<GameObject>((int)Panels.PlayerHUDSlot).transform).GetComponent<UI_PlayerMainHUD>();
           
           // 색상 컬러  
         //  _playerMainHudDictionary[player.PlayerStats].PlayerHudInit(Managers.Instance.GetPlayer(i).PlayerStats, GameLogic.Instance.PLAYER_TITLE_COLOR[i]);
           
           // 색상 컬러 (디버깅 모드) * 23/11/09 종설 최종 발표 *
           _playerMainHudDictionary[player.PlayerStats].PlayerHudInit(Managers.Instance.GetPlayer(i).PlayerStats, PLAYER_TITLE_COLOR[i]);
       }
       
       // TODO : 적군 HUD UI 초기화

    }
    
    /// <summary>
    /// 타임라인 Grid를 딕셔너리로 캐싱
    /// </summary>
    /// <param name="timeLine">캐싱할 타임라인 클래스</param>
    /// <typeparam name="T">타임라인 클래스 재너릭</typeparam>
    private void AddTimeLineGrid<T>(UI_TimeLine timeLine) where T : UI_TimeLine
    {
        _uiTimeLines.Add(typeof(T), timeLine);
    }

    /// <summary>
    /// 캐싱된 타임라인 Grid 가져옴
    /// </summary>
    /// <typeparam name="T">가져오고자는 UI_TimeLine 클래스</typeparam>
    /// <returns></returns>
    private UI_TimeLine GetTimeLineGrid<T>() where T : UI_TimeLine
    {
        return _uiTimeLines[typeof(T)];

        //   return _uiTimeLines.TryGetValue(typeof(T), out UI_TimeLine timeLineGrid) ? timeLineGrid : null;
    }


    public UI_PlayerMainHUD GetPlayerHUD(PlayerStats owner)
    {
        if (!_playerMainHudDictionary.ContainsKey(owner)) return null;

        return _playerMainHudDictionary[owner];
    }

    /// <summary>
    /// 상단 타임라인 Grid가 변경될시 턴 종료버튼도 이벤트 변경되어야함 (전투와 월드맵 턴 종료의 개념은 달라서)
    /// </summary>
    private void ChangeTurnOverButtonEvent(UnityAction<WorldMapPlayerCharacter> turnOverButtonEvent)
    {
        // TODO <! 필 수 !> : 버튼 이벤트 등록 중복을 막기위해 이벤트 꼭 제거 
        Get<Button>((int)Buttons.TurnOverButton).onClick.RemoveAllListeners();
        // 턴 종료 버튼 이벤트 등록
        Get<Button>((int)Buttons.TurnOverButton).onClick.AddListener(()=> turnOverButtonEvent(Managers.Turn.PlayerTurn));
    }

    /// <summary>
    /// 상단 타임라인 Grid 설정 (월드맵, 전투, 광산 Time Line 등 주기적으로 변경됨)
    /// </summary>
    public void ChangeTimeLineGrid<T>() where T : UI_TimeLine
    {
        /*
         *  1. 월드맵은 이미 플레이어 초상화가 정해져있음 정적임
         *  2. 광산맵은 Wave가 랜덤임 동적임
         *  3. 전투맵도 플레이어와 적의 초상화가 동적임
         */
        
        // 이미 TimeLineSlot에 다른 TimeLine Grid가 활성화 되어 있다면 비활성화
        if (_currentTimeLineObject != null)
        {
            _currentTimeLineObject.gameObject.SetActive(false);
        }
        
        // 교체할 TimeLine Grid를 불러와서 활성화
        _currentTimeLineObject = GetTimeLineGrid<T>();

        // 오류 예외처리
        if (_currentTimeLineObject == null) 
        {
            Debug.LogError("교체할 Time Line Grid가 없습니다!");
            return;
        }
        
        // Time Line Grid 종류에 맞는 턴 종료 버튼 이벤트 등록
        ChangeTurnOverButtonEvent(_currentTimeLineObject.OnClickedTurnOverButton);
        _currentTimeLineObject.gameObject.SetActive(true);

    }
    
    
    /// <summary>
    /// 턴 종료 버튼 활성화 또는 비활성화
    /// </summary>
    public bool InteractionTurnOverButton
    {
        set =>  Get<Button>((int)Buttons.TurnOverButton).interactable = value;
    }

    /// <summary>
    /// 현재 플레이어 닉네임 Text 설정
    /// </summary>
    public string SetCurrentPlayerTurnText
    {
        set => Get<TextMeshProUGUI>((int)Texts.CurrentPlayerTurnText).text = value +" 의 턴";
    }


    public void SetQuestChart(bool isOn)
    {
       Get<UI_QuestChart>(0).gameObject.SetActive(isOn);
    }
    public UI_QuestChart GetQuestChart()
    {
        return Get<UI_QuestChart>(0);
    }
    
}
