using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager 
{

    /*
    *  1. World Map 기준 Unit 최대 3명
    *  2. Battle 기준 Unit 최대 6명
    *  3. Cave 기준 최대 6 Wave
    */

   
   #region  World Map 턴 관리

   public WorldMapPlayerCharacter PlayerTurn { get; private set; }  // 현재 턴을 가진 플레이어
   
   private LinkedList<WorldMapPlayerCharacter> _playerLinkedList;   //  플레이어들의 턴을 관리하는 LinkedList 
   
   private LinkedListNode<WorldMapPlayerCharacter> _lastPlayerTurnNode;  // 마지막 차례였던 플레이어 Node
   
   private int _playerCount; // 최대 플레이어 길이 캐싱
   public int CurrentPlayerIndex { get; private set; } = 0; // 현재 플레이어 턴 Index
   public int PreviouslyPlayerIndex { get; private set; }  // 마지막 플레이어 턴 Index
   
 
   #endregion

   #region World Map 관련 함수
    
   /// <summary>
   /// 월드맵 플레이어 턴 초기화 (1회 호출)
   /// </summary>
   /// <param name="playerList">최대 플레이어 길이</param>
   public void WorldMapPlayerTurnInit(List<WorldMapPlayerCharacter> playerList)
   {
       _playerCount = playerList.Count;
       // 플레이어 길이만큼 LinkedList 생성
       _playerLinkedList = new LinkedList<WorldMapPlayerCharacter>(playerList);
       // 가장 앞에 있는 Player Node를 가져옴
       _lastPlayerTurnNode = _playerLinkedList.First;
       PlayerTurn = _lastPlayerTurnNode.Value;
       
       
       // 플레이어 이동력 부여
       PlayerTurn.SetPlayerMovementPoints();
       // 카메라 위치 즉시 순간이동
       Managers.Camera.TeleportToWorldCamera(PlayerTurn.transform.position);
       // 이동할 플레이어 설정
       UnitManager.Instacne.HandleUnitSelected(PlayerTurn.gameObject);

       // 현재 플레이어 턴 강조 타이틀 켜기
       Managers.UIManager.GetPlayerHUD(PlayerTurn.PlayerStats).UpdatePlayerHudTitleGrow(true);
       
       // 마지막 플레이어 인덱스 초기화
       PreviouslyPlayerIndex = _playerCount - 1;
   }
   
    /// <summary>
    /// 다음 플레이어 차례 확인 (월드 맵 전용)
    /// </summary>
    /// <param name="updateTimeLine">월드 맵 타임라인 업데이트</param>
    /// <returns>다음 차례인 플레이어</returns>
   public WorldMapPlayerCharacter GetNextWorldMapPlayerTurn(out bool updateTimeLine)
   {
       updateTimeLine = false;
       
       // 전 플레이어 턴 강조 타이틀 끄기
       Managers.UIManager.GetPlayerHUD(_lastPlayerTurnNode.Value.PlayerStats).UpdatePlayerHudTitleGrow(false);
       
       // 만약 다음 Player Node가 null 이면 다시 처음 Player Node를 참조
       if (_lastPlayerTurnNode.Next == null)
       {
           updateTimeLine = true; // 모든 플레이어가 턴을 종료 했으니 World Map TimeLine 업데이트
           _lastPlayerTurnNode = _playerLinkedList.First;
       }
       else
       {
           // Player Node의 다음 노드를 참조함
           _lastPlayerTurnNode = _lastPlayerTurnNode.Next;
       }
       
       // Player Node Value를 Player에 참조
       PlayerTurn = _lastPlayerTurnNode.Value;
       
       // 플레이어 턴의 인덱스를 업데이트
       CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _playerCount;
       PreviouslyPlayerIndex = (CurrentPlayerIndex - 1 + _playerCount) % _playerCount;
       
      // 플레이어 이동력 부여
       PlayerTurn.SetPlayerMovementPoints();
       Managers.Camera.ReTargetToWorldCamera(PlayerTurn.transform.position);
       UnitManager.Instacne.HandleUnitSelected(PlayerTurn.gameObject);

       // 현재 플레이어 턴 강조 타이틀 켜기
       Managers.UIManager.GetPlayerHUD(PlayerTurn.PlayerStats).UpdatePlayerHudTitleGrow(true);

       // 다음 차례인 Player를 반환
       return PlayerTurn;
   }
   

   #endregion

   
}
