using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckHexEvent
{
    /* <주 목표>
     * 1. Hex 이벤트를 확인한다.
     *  1-1. 전투 이벤트일 경우 전투범위에 맞는 Hex Overlay 활성화와  1 <= 3개 까지 전투 타입 Hex를 가져온다.
     *  1-2. 전투를 제외한 특정 Hex 이벤트는 단일 대상 Hex를 가져온다. (동굴 제외)
     * 2. Hex 이벤트에 배치된 이벤트 오브젝트를 복사한다.
     * 3. 이벤트 팝업 창에 복사한 오브젝트 정보를 UI에게 넘겨준다.
     * 4. 해당 Hex 이벤트를 종료할 경우 평범한 Hex 타입으로 설정한다.
     */

    private readonly int _MAX_BATTLE_UNIT_COUNT = 3;
    
    // 해당 Hex 이벤트에 관련된 모든 이벤트 오브젝트들 (적, 상점, 광산 등)
    private List<IEventInfo> _hexEventInfos = new List<IEventInfo>();
    
    // 전투 이벤트 범위에 포함되어 있는 Hex 딕셔너리 (Key는 Hex, Value는 딱히 사용하지 않음)
    private readonly Dictionary<Hex, int> _battleHexDictionary = new Dictionary<Hex, int>();
    
    // 해당 이벤트를 포함한 Hex 리스트 (ex. 전투 종료 후 Hex 타입을 기본으로 만듬)
    private readonly List<Hex> _eventHexList = new List<Hex>();
    
    // 이벤트 참여할 플레이어 리스트
    private readonly List<WorldMapPlayerCharacter> _playerList = new List<WorldMapPlayerCharacter>();
    
    /// <summary>
    /// 플레이어 위치에 있는 Hex 타입을 확인
    /// </summary>
    /// <param name="isMove">플레이어가 아직 이동여부를 확인</param>
    /// <param name="hex">도착한 Hex</param>
    /// <param name="callWorldMapPlayerCharacter">도착한 플레이어</param>
    /// <param name="hexGrid">도착한 Hex의 정보를 확인</param>
    /// <param name="checkRange">Hex의 범위를 확인하는 BFS</param>
    public bool CheckingHexType(bool isMove, Hex hex, WorldMapPlayerCharacter callWorldMapPlayerCharacter ,HexGrid hexGrid, MovementSystem checkRange)
    {
        /*  1. 이동중인 경우 전투와 특별 이벤트만 감지
         *  2. 최종지점인 경우 기본지형 이벤트를 제외하고 모든 이벤트를 감지
         */
        
        
        bool isEvent = false;

        // 기본 지형 및 물 타일은 무시
        if (hex.HexType is HexType.Default or HexType.Water) return isEvent;
        
        // 이동 중인 경우에는 전투와 특별 이벤트, 퀘스트만 확인
        if (isMove && hex.HexType != HexType.BattleZone && hex.HexType != HexType.QuestZone) return isEvent; //|| hex.HexType != HexType.Special) return; 
        
        
        /*
         * 0. 전에 사용했던 이벤트 기록을 모두 초기화
         * 1. 그냥 일반 지형인 경우 무시
        *  2. 전투 지형인 경우 전투범위 확인
        *  3. 퀘스트 용도
        *  4. 동굴인 경우 해당 Hex 마을 지형범위를 모두 확인하여 Player 존재 확인
        *  4. 그외 다른 이벤트 지형인 경우 해당 Hex만 확인
        */
        
        InitEventList();

        // 첫 이벤트 오브젝트 정보는 무조건 접근한 Hex 기준으로 넣음
        _hexEventInfos.Add(hex.GetEventInfoList()[0]);
        // 가장 처음에 접근한 플레이어를 리스트에 넣음
        _playerList.Add(callWorldMapPlayerCharacter);

        switch (hex.HexType)
        {
            case HexType.BattleZone :
                isEvent = true;
                PrePareUnitForBattleRange(hex, hexGrid, checkRange); 
                break; 
            case HexType.QuestZone:
                
                switch (hex.QuestZoneType)
                {
                    case QuestZoneType.Battle:
                        isEvent = true;
                        PrePareUnitForBattleRange(hex, hexGrid, checkRange);
                        break;
                    case QuestZoneType.Delivery:
                        // 퀘스트 배달을 완료 했으니 보상 수령
                        Managers.Quest.ClearQuest(UnitManager.Instacne.HexGrid.GetQuestHexID(hex).questID);
                        // 퀘스트 완료 했으니 퀘스트 지역 초기화
                        UnitManager.Instacne.HexGrid.RemoveQuestHexInfo(hex);
                        break;
                }
                
                break;
            
            case HexType.Event:
                isEvent = true;
                _eventHexList.Add(hex);
                break;
            
            case HexType.Sanctuary:
                isEvent = true;
                _eventHexList.Add(hex);
                break;
                  
            // case HexType.Cave : break;
            // default: break;
        }
        
        CallEventPopUpUI(hex);
        callWorldMapPlayerCharacter.IsPlayerInEvent = true;
        return isEvent;
    }


    /// <summary>
    /// 이벤트 팝업 UI 호출
    /// </summary>
    /// <param name="hex">플레이어가 도착한 Hex 정보</param>
    private void CallEventPopUpUI(Hex hex)
    {
        // 어떤 이벤트 타입이 오든 가장 첫 번째 Hex 기준으로 UI를 보여주면 된다. 이벤트 함수 2개를 전달해준다. (자세한 내용은 해당 함수참조)
        //_hexEventObjects
        Managers.UIManager.ShowEventPopUpUI(hex, _hexEventInfos, _playerList,  
         () => { ResetHexOverlay(hex.BattleZoneType); }, DestroyHexEventObject);
    }
    
    private void PrePareUnitForBattleRange(Hex hex, HexGrid hexGrid, MovementSystem checkRange)
    {

        bool isSoloEnemy = true;
        
        // 현재 위치 Hex에 포함된 상점 인덱스를 참고
        var storeHexIndex = UnitManager.Instacne.HexGrid.GetStoreHexIndex(hex.HexCoords);
        // 해당 상점 지형의 전투범위
        var battleRange = UnitManager.Instacne.HexGrid.GetStoreHexBattleRange(storeHexIndex);
        /* 해당 Hex에 접근한 Player를 제외한 다른 Player 위치를 찾아서 전투범위에 있는지 확인 */
        var playerMaxCount = Managers.Instance.GetPlayerCount;
        
        //전투 캠프 또는 그룹인 경우 해당 Hex만 참조
        if (hex.BattleZoneType is BattleZoneType.Camp or BattleZoneType.Group )
        {
            isSoloEnemy = false;
            _eventHexList.Add(hex);
            _hexEventInfos = hex.GetEventInfoList();
            // TODO : 전투 범위는 설정하지만 Player만 전투만 참여하게 (적군은 이미 전투캠프에 2~3명 차지하고 있음)
        }
        

        // 전투범위에 포함되는 주변 Hex 리스트
        List<Hex> neighboursHexList = checkRange.GetAroundHexBFS(battleRange, hex, UnitManager.Instacne.HexGrid).Select(UnitManager.Instacne.HexGrid.GetTileAt).ToList();
        hex.EnableBattleOverlay();
        
        foreach (var overlayHex in neighboursHexList)
        {
            // 주변 Hex중에서 장애물 또는 바다 지형이 아닌곳만 확인
            if(overlayHex.HexType is HexType.Obstacle or HexType.Water) continue;
            _battleHexDictionary.Add(overlayHex, overlayHex.GetInHexPlayerCount());
            overlayHex.EnableBattleOverlay();
        }

        // 전투 지역이 캠프나 파티 지역이 아닌경우는 다른 전투범위에서 적군을 확인함
        if (isSoloEnemy)
        {
            // 전투에 참여하는 적군이랑, 아군 최대 허용 범위 (최대 값 3, 1로 초기화는 가장 먼저 배치된 Hex가 있으니 1로 설정)
            int currentListCount = 1;
        
            //( !! 가장 먼저 전투가 일어난 Hex가 있으니 적군 정보를 가져옴)
            _eventHexList.Add(hex);
        
            // 전투범위에서 적 최대 3명과 플레이어 최대 접속한 수만큼 확인함
            // 1. 우선 적부터 Hex 확인
            foreach (var battleHex in _battleHexDictionary)
            {
                // 그 전투지역 범위에서 딱 3명만 가져옴
                if(currentListCount == _MAX_BATTLE_UNIT_COUNT) break;
                if (battleHex.Key.HexType != HexType.BattleZone) continue;
            
                var enemyInfoList = battleHex.Key.GetEventInfoList();
                _hexEventInfos.Add(enemyInfoList[0]);
                _eventHexList.Add(battleHex.Key);
                currentListCount++;
            }
        }
        
   
        // 혼자 플레이할 경우 주변 동료가 없으니 Skip
        //if (playerMaxCount == 1) return;
        
        // 2. 플레이어 Hex 확인
        // 이미 Hex에 도착한 플레이어를 제외하고 나머지 플레이어 수만큼 탐색
        for (int i = 0; i < playerMaxCount; i++)
        {
            // 플레이어 위치와 전투 범위에 있는 플레이어 Hex Key값을 비교해 존재하면 플레이어도 전투 범위에 있다는 의미로 플레이어 리스트에 넣음
            if (_battleHexDictionary.ContainsKey(Managers.Instance.GetPlayer(i).CurrentHex))
            {
                _playerList.Add(Managers.Instance.GetPlayer(i));
            }
        }
        
        // 모든 전투범위 계산이 완료되면 마지막에 현재 진행된 전투지역 Hex를 넣어줌
        _battleHexDictionary.Add(hex, hex.GetInHexPlayerCount());
    }

    /// <summary>
    /// Hex 전투 오버레이 표시 비활성화
    /// </summary>
    private void ResetHexOverlay(BattleZoneType type)
    {
        if (type != BattleZoneType.None)
        {
            foreach (var overlayHex in _battleHexDictionary)
            {
                overlayHex.Key.DisableBattleOverlay();
            }
        }
    }

    /// <summary>
    /// Hex 이벤트를 제거하고 Hex 타입도 기본으로 설정
    /// </summary>
    private void DestroyHexEventObject()
    {
        foreach (var eventHex in _eventHexList)
        {
            eventHex.DestroyAllEventObject();
        }
    }

    /// <summary>
    /// 모든 저장된 리스트에 내용을 초기화 
    /// </summary>
    private void InitEventList()
    {
        // null 체크 후 리스트 초기화
        _battleHexDictionary.Clear();
        _eventHexList?.Clear();
        _hexEventInfos?.Clear();
        _playerList?.Clear();
    }
}
