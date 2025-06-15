using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    private BFSResult _playerMovementRangeBFS = new BFSResult();

    private BFSResult _hexRangeBFS = new BFSResult();
    
    private List<Vector3Int> _currentPath = new List<Vector3Int>();

    /*
     * _movementRange 변수는 BFSResult 객체를 저장하고 있습니다. BFSResult 객체는 BFSGetRange 메소드를 사용하여 계산된 유닛의 이동 가능 범위 정보를 포함합니다.
        _currentPath 변수는 List<Vector3Int> 객체를 저장하고 있습니다. 이 리스트에는 현재 선택된 타일까지 이동하기 위한 경로 정보가 저장됩니다.
        HideRange 메소드는 _movementRange 변수에 저장된 이동 가능 범위 정보를 사용하여, 이동 가능한 모든 타일의 하이라이트를 비활성화 합니다.
        ShowRange 메소드는 CalculationRange 메소드를 사용하여 유닛의 이동 가능 범위 정보를 계산하고, 계산된 정보를 사용하여 이동 가능한 모든 타일의 하이라이트를 활성화 합니다.
        CalculationRange 메소드는 GraphSearch.BFSGetRange 메소드를 사용하여 유닛의 이동 가능 범위 정보를 계산합니다. 이 메소드는 selectedUnit 매개변수로 전달된 유닛의 위치에서, hexGrid 매개변수로 전달된 HexGrid 객체 내에서 이동 가능한 범위를 계산합니다.
        ShowPath 메소드는 _movementRange 변수에 저장된 이동 가능 범위 정보를 사용하여, 선택된 타일까지의 경로 정보를 계산하고, 계산된 경로 정보를 사용하여 경로를 하이라이트 표시합니다. 이 때, _currentPath 변수를 사용하여 이전에 표시된 경로 하이라이트를 제거합니다.
     */
    
    public void HideRange(HexGrid hexGrid)
    {
        // 만약 숨길 경로가 없다면 스킵
        if (_playerMovementRangeBFS.VisitedNodesDictionary == null) return;
        
        /*  23-11-11 플레이어 이동 최대 범위 하이라이트 머리티리얼 비활성화는 필요가 없음
        foreach (var hexPosition in _playerMovementRangeBFS.GetRangePositions())
        {
            hexGrid.GetTileAt(hexPosition).DisableHighlight();
        }
        */
        
        //  23-11-11 플레이어 이동 경로 범위를 표시한 하이라이트 머리티리얼만 비활성화
        foreach (var hexPosition in _currentPath)
        {
            hexGrid.GetTileAt(hexPosition).ResetHighlight();
        }

        _playerMovementRangeBFS = new BFSResult();
    }

    public void ShowRange(WorldMapPlayerCharacter selectedWorldMapPlayerCharacter, HexGrid hexGrid)
    {
        //플레이어의 위치 계산은 좌표가 아닌 CurrentHex의 변수로 진행한다 (05/15)
        CalculationRange(selectedWorldMapPlayerCharacter, hexGrid);

        /*    23-11-11 플레이어 이동 최대 범위 하이라이트 머리티리얼 활성화는 필요가 없음
        Vector3Int unitPos = selectedWorldMapPlayerCharacter.CurrentHex.HexCoords;//hexGrid.GetClosestHex(selectedWorldMapPlayerCharacter.transform.position); // 현재 위치 Hex의 쉐이더를 비활성화
        
        // 플레이어가 이동할 수 있는 최대 범위 머티리얼을 활성화
        foreach (var hexPosition in _playerMovementRangeBFS.GetRangePositions())
        {
            if(unitPos == hexPosition) continue; // 현재 위치 Hex의 위치가 같으면 넘김
            
           
            hexGrid.GetTileAt(hexPosition).EnableHighlight();
        }
        */
    }

    private void CalculationRange(WorldMapPlayerCharacter selectedWorldMapPlayerCharacter, HexGrid hexGrid)
    {
        _playerMovementRangeBFS = GraphSearch.BFSGetRange(hexGrid, hexGrid.GetClosestHex(selectedWorldMapPlayerCharacter.CurrentHex.transform.position), selectedWorldMapPlayerCharacter.MovementPoints);
    }

    private void CalculationRange(int range, Vector3 standardHexPosition, HexGrid hexGrid, bool skipObstacle = true)
    {
        _hexRangeBFS = GraphSearch.BFSGetRange(hexGrid, hexGrid.GetClosestHex(standardHexPosition), range, skipObstacle);
    }

    public void SetCurrentPath(List<Vector3Int> pathList)
    {
        _currentPath = pathList;
    }

    public void ShowPath(Vector3Int selectedHexPosition, HexGrid hexGrid)
    {
        // 플레이어가 이동만 할 경로 머리티얼 활성화 및 비활성화
        
        if (_playerMovementRangeBFS.GetRangePositions().Contains(selectedHexPosition))
        {
            foreach (var hexPosition in _currentPath)
            {
                hexGrid.GetTileAt(hexPosition).ResetHighlight();
            }

            SetCurrentPath(_playerMovementRangeBFS.GetPathTo(selectedHexPosition));

            foreach (var hexPosition in _currentPath)
            {
                hexGrid.GetTileAt(hexPosition).HighlightPath();
            }
        }
    }


    public List<Vector3Int> GetAroundHexBFS(int maxRange, Hex standardHex, HexGrid hexGrid, bool skipObstacle = true)
    {
        List<Vector3Int> aroundHexPositions = new List<Vector3Int>();
        var startPosition = standardHex.transform.position;
        
        CalculationRange(maxRange, startPosition, hexGrid, skipObstacle);

        Vector3Int startHex = hexGrid.GetClosestHex(startPosition); // 현재 위치 Hex의 쉐이더를 비활성화
        
        foreach (var hexPosition in _hexRangeBFS.GetRangePositions())
        {
            if(startHex == hexPosition) continue; // 현재 위치 Hex의 위치가 같으면 넘김
            aroundHexPositions.Add(hexPosition);
        }

        return aroundHexPositions;
    }

    public void MoveUnit(WorldMapPlayerCharacter selectedWorldMapPlayerCharacter, HexGrid hexGrid)
    {
        selectedWorldMapPlayerCharacter.MoveThroughPath(_currentPath.Select(pos => hexGrid.GetTileAt(pos).transform.position).ToList(),
                                                _currentPath.Select(pos => hexGrid.GetTileAt(pos)).ToList());
        
    }

    public bool IsHexInRange(Vector3Int hexPosition)
    {
        return _playerMovementRangeBFS.IsHexPositionInRange(hexPosition);
    }
}
