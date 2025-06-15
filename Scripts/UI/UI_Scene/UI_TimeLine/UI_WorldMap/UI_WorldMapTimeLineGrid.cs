using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI_WorldMapTimeLineGrid : UI_TimeLine
{
    
    
    /*  << World Map Time Line 목표 >>
     *  1. 플레이어 초상화 Time Line Sprite => 완료
     *  2. 현재 World Map Time Line Sprite => 낮(4), 밤(4)의 총 8개의 이미지로 구성됨
     *   2-1. N명의 플레이어가 모두 턴을 종료하면 현재 이미지가 가장 뒤로 감
     *  3. 현재 D-Day Text => 낮(4), 밤(4)의 1 Cycle을 모두 돌면 1일 추가
     */

    #region 플레이어 초상화 타임라인

    private const string _PLAYER_TURN_ICON_PERFAB_PATH = "Prefabs/UI/TimeLine/UI_WorldMapTimeLine/UI_WorldMapTimeLinePlayerIcon";
    
    private UI_WorldMapPlayerTimeLineIcon[] _playerTimeLineIcons;
    
    // 플레이어 턴 활성화 시 초상화 배경색상
    private readonly Color _enableBackGroundColor = Color.white;
    
    // 플레이어 턴 비활성화 시 초상화 배경색상
    private readonly Color _disableBackGroundColor = Color.gray;
    
    #endregion

    #region 월드 맵 타임라인

    private readonly WorldMapDayNightCycle _worldMapDayNightCycle = new WorldMapDayNightCycle();
    
    #endregion
    

    private enum Panels
    {
        PlayerTimeLineSlot,
        TimeLinePanel,
    }

    private enum Text
    {
        DateText
    }

    private enum PlayerTurnAlarmObject
    {
        UI_PlayerTurnAlarm
    }

    /// <summary>
    /// 현재 플레이어 수 만큼 초상화 생성 및 플레이어 턴 알람 UI 비활성화 (1회 호출)
    /// </summary>
    public override void Init()
    {
        Bind<GameObject>(typeof(Panels));
        Bind<TextMeshProUGUI>(typeof(Text));
        Bind<UI_PlayerTurnAlarm>(typeof(PlayerTurnAlarmObject));
        
        // 플레이어 턴 알람 UI 초기화 후 비활성화
        Get<UI_PlayerTurnAlarm>((int)PlayerTurnAlarmObject.UI_PlayerTurnAlarm).Init();

        InitWorldMapPlayerTimeLine();
        WorldMapPlayerCharacter.OnPlayerTurnAlarmEvent += UpdatePlayerTurnUI;
    }

    /// <summary>
    /// 플레이어 초상화 설정 (1회 호출)
    /// </summary>
    private void InitWorldMapPlayerTimeLine()
    {
        // 총 플레이어 확인
        var playerCount = Managers.Instance.GetPlayerCount;
        // 총 플레이어 숫자만큼 배열 정의
        _playerTimeLineIcons = new UI_WorldMapPlayerTimeLineIcon[playerCount];
        var playerTimeLineSlot = Get<GameObject>((int)Panels.PlayerTimeLineSlot).transform;

        for (int i = 0; i < playerCount; i++)
        {
            // 플레이어 초상화 UI 생성과 동시에 playerTimeLineSlot 자식으로 설정
            var playerIconObject = Managers.UIManager
                .MakeUiPrefab<UI_WorldMapPlayerTimeLineIcon>(playerTimeLineSlot, _PLAYER_TURN_ICON_PERFAB_PATH);
            
            
            // 플레이어 초상화 정보를 불러와 Player Icon에 초상화 초기화
            playerIconObject.PlayerIconInit(Managers.Instance.GetPlayer(i).PlayerStats.PlayerPortrait);
            // 플레이어 초상화 객체를 배열에 참조
            _playerTimeLineIcons[i] = playerIconObject;
        }
        
        SwitchPlayerTimeLineIcon(Managers.Turn.CurrentPlayerIndex, Managers.Turn.PreviouslyPlayerIndex);
    }

    /// <summary>
    /// 플레이어 타임라인 초상화 중에서 현재 턴을 가진 플레이어 초상화만 활성화 그외 비활성화
    /// </summary>
    /// <param name="currentPlayerIndex">현재 플레이어 순서</param>
    /// <param name="lastPlayerIndex">마지막 플레이어 순서</param>
    private void SwitchPlayerTimeLineIcon(int currentPlayerIndex, int lastPlayerIndex)
    {
        _playerTimeLineIcons[lastPlayerIndex].SetPlayerIconBackGroundColor = _disableBackGroundColor;
        _playerTimeLineIcons[currentPlayerIndex].SetPlayerIconBackGroundColor = _enableBackGroundColor;
    }

    /// <summary>
    /// 월드 맵에서 경과 된 Day Text 업데이트
    /// </summary>
    /// <param name="currentDay">최종 Day</param>
    private void UpdateDateText(int currentDay)
    {
        Get<TextMeshProUGUI>((int)Text.DateText).text = currentDay.ToString();
    }

    /// <summary>
    /// 월드 맵 타임라인 이미지 순서 업데이트
    /// </summary>
    private void UpdateWorldMapTimeLine()
    {
        Get<GameObject>((int)Panels.TimeLinePanel).transform.GetChild(0).SetAsLastSibling();
    }

    /// <summary>
    /// 플레이어 턴 알림 UI 업데이트
    /// </summary>
    private void UpdatePlayerTurnUI(WorldMapPlayerCharacter currentPlayerCharacter)
    {
        var playerTurnAlarm = Get<UI_PlayerTurnAlarm>((int)PlayerTurnAlarmObject.UI_PlayerTurnAlarm);

        playerTurnAlarm.transform.DOScale(Vector3.one, 0.5f).From(new Vector3(0f, 1f, 1f));
        playerTurnAlarm.gameObject.SetActive(true);
        
        playerTurnAlarm.SetPlayerInfo(currentPlayerCharacter.PlayerStats.PlayerPortrait, 
                                        currentPlayerCharacter.PlayerStats.BaseClassName, 
                                        currentPlayerCharacter.MovementPoints, 
                                        currentPlayerCharacter.PlayerStats.MaxMovements, 
                                        currentPlayerCharacter.ResultMovementSlotsIcon);
    }



    /// <summary>
    /// 턴 종료 버튼 클릭 이벤트 (월드맵 전용 이벤트)
    /// </summary>
    /// <param name="previouslyPlayer">현재 턴 종료하는 플레이어</param>
    public override void OnClickedTurnOverButton(WorldMapPlayerCharacter previouslyPlayer)
    {
        // 턴을 종료하는 플레이어 이동력을 0으로 설정
        previouslyPlayer.EndOfTurnPlayer();
        
        // 다음 턴을 대기중인 플레이어 설정 
        var playerCharacter = Managers.Turn.GetNextWorldMapPlayerTurn(out bool updateTimeLine);
        //string playerNickName = playerNickNameClone.Substring(playerNickNameClone.IndexOf("_")+1, playerNickNameClone.IndexOf("(")- playerNickNameClone.IndexOf("_") - 1);
        // 플레이어 닉네임 설정 그리고 모든 플레이어 종료 Cycle 확인 (updateTimeLine)
        Managers.UIManager.ChangePlayerTurnText(playerCharacter.PlayerStats.BaseClassName);

        _worldMapDayNightCycle.AdvanceTime(out bool isFirstDay, out bool isFirstNight); 

        // 만약 월드 맵 타임라인 업데이트가 필요하면
        if (updateTimeLine) UpdateWorldMapTimeLine();

     
        if (isFirstDay)           // 첫날 낮일 경우
        {
            // TODO : 낮 몬스터로 Tile 교체
            
            // 첫 낮일 경우 무조건 D-Day 1일 증가함
            UpdateDateText(_worldMapDayNightCycle.DaysElapsed);
        }
        else if (isFirstNight)    // 첫날 밤일 경우
        {
            // TODO : 밤 몬스터로 Tile 교체
        }
        
        // 플레이어 월드 맵 타임라인 초상화 배경화면 설정
        SwitchPlayerTimeLineIcon(Managers.Turn.CurrentPlayerIndex, Managers.Turn.PreviouslyPlayerIndex);
    }
}
