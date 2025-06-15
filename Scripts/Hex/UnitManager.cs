using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instacne { get; private set; }

    [SerializeField] private HexGrid _hexGrid;

    public HexGrid HexGrid
    {
        get => _hexGrid;
    }

    [SerializeField] private MovementSystem _movementSystem;
    
    // 플레이어가 도착한 Hex 타입을 확인
    private readonly CheckHexEvent _checkHexEvent = new CheckHexEvent();
    
    public bool PlayersTurn { get; private set; } = true;
    
    public WorldMapPlayerCharacter SelectedWorldMapPlayerCharacter { get; private set; }
    

    private Hex _previouslySelectedHex; // 마지막에 선택된 유닛

    private void Awake()
    {
        if (Instacne == null)
        {
            Instacne = this;
        }
    }
    

    public void HandleUnitSelected(GameObject unit)
    {
        if (PlayersTurn == false)
            return;

        WorldMapPlayerCharacter worldMapPlayerCharacterReference = unit.GetComponent<WorldMapPlayerCharacter>();
        
        /*
        // 선택된 플레이어 중복체크 확인
        if (CheckIfTheSameUnitSelected(worldMapPlayerCharacterReference))
            return;
            
       */

        // 선택된 플레이어 Turn 확인
        if (!CheckPlayerTurn(worldMapPlayerCharacterReference))
        {
            return;
        }


        // 위 조건을 모두 만족했으니 이동 준비
        PrepareUnitForMovement(worldMapPlayerCharacterReference);
    }
    
    /*

    public void HandleTerrainSelected(GameObject hexGo)
    {
        // 예외처리
        if (SelectedWorldMapPlayerCharacter == null || PlayersTurn == false)
            return;
        
        Hex selectedHex = hexGo.GetComponent<Hex>();

        // 이동범위 밖 Hex를 선택했거나 유닛이 있는 현재 Hex 위치를 선택한 경우를 예외처리
        if (HandleHexOutOfRange(selectedHex.HexCoords) || HandleSelectedHexIsUnitHex(selectedHex.HexCoords))
            return;

        HandleTargetHexSelected(selectedHex);
    }
    */
    
    public void HandleTerrainSelected(Hex hexGo, bool isClick)
    {
        // 예외처리
        if (SelectedWorldMapPlayerCharacter == null || PlayersTurn == false)
            return;

        Hex selectedHex = hexGo;

        // 이동범위 밖 Hex를 선택했거나 유닛이 있는 현재 Hex 위치를 선택한 경우를 예외처리
        /*
        if (HandleHexOutOfRange(selectedHex.HexCoords) || HandleSelectedHexIsUnitHex(selectedHex.HexCoords))
            return;
        */

        if (!HandleHexOutOfRange(selectedHex.HexCoords) && !HandleSelectedHexIsUnitHex(selectedHex.HexCoords))
        {
            HandleTargetHexSelected(selectedHex, isClick);
        }
    }

    /// <summary>
    /// 특정 지역(Hex)으로 자동으로 플레이어를 이동하는 함수
    /// </summary>
    /// <param name="requester">이동할 플레이어</param>
    /// <param name="destHex">이동할 지형</param>
    public void AutoMovingHex(WorldMapPlayerCharacter requester, Hex destHex)
    {
        // 보통 전투에서 이기거나, 후퇴할때 이용
        
        SelectedWorldMapPlayerCharacter = requester;
        SelectedWorldMapPlayerCharacter.IsPlayerInEvent = false;
        
        // 전투에 이겼으니 해당 Hex 위치로 강제이동
        if (destHex == SelectedWorldMapPlayerCharacter.CurrentHex)
        {
            Vector3 hexPos = destHex.transform.position;
            Transform playerTransform = SelectedWorldMapPlayerCharacter.transform;
            hexPos.y = playerTransform.position.y;
            playerTransform.position = hexPos;
            ResetTurn(SelectedWorldMapPlayerCharacter);
            return;
        }
        
        // 전투에서 후퇴하는 경우
       // _movementSystem.ShowPath(destHex.HexCoords, this._hexGrid);
       _movementSystem.SetCurrentPath(new List<Vector3Int> { destHex.HexCoords });
        _movementSystem.MoveUnit(SelectedWorldMapPlayerCharacter, this._hexGrid);
        PlayersTurn = false; // 플레이어 턴 비활성화 (이동 하고 있으니깐)
        SelectedWorldMapPlayerCharacter.MovementFinished += ResetTurn;
        ClearOldSelection();
    }
    
    private bool CheckIfTheSameUnitSelected(WorldMapPlayerCharacter worldMapPlayerCharacterReference)
    {
        if (this.SelectedWorldMapPlayerCharacter == worldMapPlayerCharacterReference)
        {
            ClearOldSelection();
            return true;
        }

        return false;
    }
    
    private bool CheckPlayerTurn(WorldMapPlayerCharacter worldMapPlayerCharacterReference)
    {
        // 선택된 플레이어와 현재 턴을 가진 플레이어 정보와 같은경우 
        return worldMapPlayerCharacterReference == Managers.Turn.PlayerTurn;
    }
    
    private void HandleTargetHexSelected(Hex selectedHex, bool isClick)
    {
        // 이동 가능한 경로 보여줌
        if (_previouslySelectedHex == null || _previouslySelectedHex != selectedHex)
        {
            _previouslySelectedHex = selectedHex;
            _movementSystem.ShowPath(selectedHex.HexCoords, this._hexGrid);
        }
        else if (isClick) // 플레이어 이동
        {
            _movementSystem.MoveUnit(SelectedWorldMapPlayerCharacter, this._hexGrid);
            PlayersTurn = false; // 플레이어 턴 비활성화 (이동 하고 있으니깐)
            SelectedWorldMapPlayerCharacter.MovementFinished += ResetTurn;
            SelectedWorldMapPlayerCharacter.CheckHexType += CheckHexType;
            ClearOldSelection();
        }
    }
    
    /*
    private bool HandleSelectedHexIsUnitHex(Vector3Int hexPosition)
    {
        if (hexPosition == _hexGrid.GetClosestHex(SelectedWorldMapPlayerCharacter.transform.position))
        {
            SelectedWorldMapPlayerCharacter.Deselect();
            ClearOldSelection();
            return true;
        }

        return false;
    }
    */
    
    private bool HandleSelectedHexIsUnitHex(Vector3Int hexPosition)
    {
        if (hexPosition == _hexGrid.GetClosestHex(SelectedWorldMapPlayerCharacter.transform.position))
        {
            return true;
        }

        return false;
    }

    private bool HandleHexOutOfRange(Vector3Int hexPosition)
    {
        if (_movementSystem.IsHexInRange(hexPosition) == false)
        {
            // 이동범위를 초과한 Hex 노드를 선택했을 경우
            print("Hex Out Of Range!");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 플레이어 턴 초기화
    /// </summary>
    /// <param name="selectedWorldMapPlayerCharacter">선택된 플레이어</param>
    /// <param name="isEvent">현재 위치에 이벤트가 발생한 경우</param>
    private void ResetTurn(WorldMapPlayerCharacter selectedWorldMapPlayerCharacter)
    {
        selectedWorldMapPlayerCharacter.CheckHexType -= CheckHexType;
        selectedWorldMapPlayerCharacter.MovementFinished -= ResetTurn;
        
        PlayersTurn = true; // 플레이어 턴 활성화
        if (selectedWorldMapPlayerCharacter.IsPlayerInEvent == false)
        {
            HandleUnitSelected(selectedWorldMapPlayerCharacter.gameObject);
        }
        
        // 어떻게 되었든 이동을 완료 했으니 해당 위치로 카메라를 이동함  (전투종료할 경우 텔레포트, 월드맵에서는 타켓팅)
        Managers.Camera.ReTargetToWorldCamera(selectedWorldMapPlayerCharacter.transform.position);
    }
    
    

    /// <summary>
    /// 현재 Hex의 Type을 확인함
    /// </summary>
    /// <param name="selectedWorldMapPlayerCharacter"></param>
    /// <param name="hexInfo"></param>
    /// <param name="battleZoneCheck"></param>
    /// <returns>전투 지역이면 true를 반환</returns>
    private bool CheckHexType(WorldMapPlayerCharacter selectedWorldMapPlayerCharacter, Hex hexInfo, bool isMove)
    {
        return _checkHexEvent.CheckingHexType(isMove, hexInfo, selectedWorldMapPlayerCharacter, _hexGrid, _movementSystem);
    }
    
    private void PrepareUnitForMovement(WorldMapPlayerCharacter worldMapPlayerCharacterReference)
    {
        ClearOldSelection();
        
        this.SelectedWorldMapPlayerCharacter = worldMapPlayerCharacterReference;
        //this.SelectedWorldMapPlayerCharacter.Select(); 
        _movementSystem.ShowRange(this.SelectedWorldMapPlayerCharacter, this._hexGrid);
    }

    /// <summary>
    /// 선택된 플레이어의 쉐이더 및 Hex 경로 쉐이더 초기화
    /// </summary>
    public void ClearOldSelection()
    {
        if (!(bool)SelectedWorldMapPlayerCharacter) return;
        
        _previouslySelectedHex = null;
       // this.SelectedWorldMapPlayerCharacter.Deselect();
        _movementSystem.HideRange(this._hexGrid);
        this.SelectedWorldMapPlayerCharacter = null;
    }
    


}
