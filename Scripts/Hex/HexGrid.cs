using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    #region 모든 지형 정보

    // 모든 Hex Tile에 대한 좌표값을 저장하는 자료구조
    private readonly Dictionary<Vector3Int, Hex> _hexTileDictionary = new Dictionary<Vector3Int, Hex>();
    
    private readonly Dictionary<Vector3Int, List<Vector3Int>> _hexTileNeighboursDictionary = new Dictionary<Vector3Int, List<Vector3Int>>();

    #endregion
    
    #region 마을 지형 정보

    // 마을 지형 정보
    private readonly List<Vector3Int> _storeHexTileList= new List<Vector3Int>();
    
    // 마을 총 갯수
    public int StoreHexLength { get; private set; } = 0;
    
    // 마을 지형 Hex 경계선  정보 (int : 마을 인덱스, List : 인덱스에 해당하는 마을 주변 경계선 Hex 좌표들)
    private readonly Dictionary<int, List<Vector3Int>> _storeNeighboursHexDictionary = new Dictionary<int, List<Vector3Int>>();
    
    // 마일 지형 Hex 경계선  개수 (int : 마을 인덱스, int : 인덱스에 해당하는 마을 주변 경계선 Hex 총 개수)
    private readonly Dictionary<int, int> _storeNeighboursLength = new Dictionary<int, int>();
    
    // 마을 지형 Hex 전투범위 설정 
    private readonly Dictionary<int, int> _storeHexBattleRange = new Dictionary<int, int>();

    #endregion

    #region 퀘스트 지형 정보

    // 퀘스트 지형 정보 저장
    private readonly Dictionary<Hex, int> _questHexDictionary = new Dictionary<Hex, int>();

    #endregion
    
    
    // 임계영역 (세마포어) 설정
    private static readonly object Lock = new object();

    
    /// <summary>
    /// 모든 Hex 좌표 초기화 (마을 재생서시 사용)
    /// </summary>
    public void AllClearHexInfo()
    {
        _hexTileDictionary.Clear();
        _storeHexTileList.Clear();
        StoreHexLength = 0;
        _storeNeighboursHexDictionary.Clear();
        _storeNeighboursLength.Clear();
        _storeHexBattleRange.Clear();
    }

    /// <summary>
    /// 모든 Hex 좌표 저장
    /// </summary>
    public void SetHexCoords()
    {
        foreach (Hex hex in FindObjectsOfType<Hex>())
        {
            _hexTileDictionary[hex.HexCoords] = hex;
              //print($"Hex 좌표 및 상태 : {hex.HexCoords} , {_hexTileDictionary[hex.HexCoords].HexType.ToString()}");
        }

        // 2, 0, 2 타일 기준에서 주변 Hex Tile 탐색
       // List<Vector3Int> neighbours = GetNeighboursFor(new Vector3Int(40, 0, 7));
    }


    #region 마을 관련된 Hex 정보 함수

    /// <summary>
    /// 마을 Hex 위치 저장
    /// </summary>
    public void SetStoreHexCoord(Vector3Int storeHex)
    {
        _storeHexTileList.Add(storeHex);
        StoreHexLength++;
        //_storeHexTileDictionary[storeHex.HexCoords] = storeHex;
    }
    
    /// <summary>
    /// 마을 Hex 위치 정보 가져옴
    /// </summary>
    /// <param name="findIndex">찾는 마을 Index</param>
    /// <returns>마을 좌표</returns>
    public Vector3Int GetStoreHexPosition(int findIndex)
    {
        return _storeHexTileList[findIndex];
    }

    /// <summary>
    /// Hex 위치를 이용해 소속된 마을 지형 인덱스를 가져옴
    /// </summary>
    /// <param name="hexCoordinates">확인하고 자는 Hex 위치</param>
    /// <returns>마을 지형 인덱스</returns>
    public int GetStoreHexIndex(Vector3Int hexCoordinates)
    {
        // FirstOrDefault LINQ 메서드를 이용해 (선형검색)을 통해 Value 값에 해당하는 key(상점 인덱스)를 얻음
        var storeHexIndex = _storeNeighboursHexDictionary.FirstOrDefault(x => x.Value.Contains(hexCoordinates)).Key;
        return storeHexIndex;
    }

    /// <summary>
    /// 마을 위치에서 부터 마을 경계선 Hex 저장
    /// </summary>
    /// <param name="storeIndex">마을 인덱스</param>
    /// <param name="hexCoordinates">저장할 Hex 위치</param>
    public void SetStoreNeighboursHex(int storeIndex, Vector3Int hexCoordinates)
    {
        // 임계영역 설정
        lock (Lock)
        {
            // 해당 마을 Index Key가 없는 경우 생로 Key, Value를 생성함
            if (_storeNeighboursHexDictionary.ContainsKey(storeIndex) == false)
            {
                _storeNeighboursHexDictionary.Add(storeIndex, new List<Vector3Int>());
                // 마을 Index Key로 접근해 Hex의 개수를 0로 초기설정함
                _storeNeighboursLength.Add(storeIndex, 0);
                //마을 지형 전투범위 설정
                _storeHexBattleRange.Add(storeIndex, 3);
            }
            
            // 마을 Index Key로 접근해 해당 Hex 위치를 저장함
            _storeNeighboursHexDictionary[storeIndex].Add(hexCoordinates);
            // 마을 Index Key로 접근해 Hex의 개수를 1증가 함
            _storeNeighboursLength[storeIndex]++;
        }
    }
    
    /// <summary>
    /// 마을로 부터 경계선 Hex 정보 획득
    /// </summary>
    /// <param name="storeIndex">마을 인덱스</param>
    /// <returns>마을 경계선 Hex 배열</returns>
    public Hex[] GetStoreNeighboursHex(int storeIndex)
    {
        // 인접 Hex 길이
        var length = GetStoreNeighboursLength(storeIndex);
        // Hex 배열 (반환 용도)
        Hex[] storeNeighboursHexArray = new Hex[length];
        // Hex 좌표 배열 (Hex 타입으로 캐스팅 용도)
        var neighbours = _storeNeighboursHexDictionary[storeIndex];
        
        for(int i = 0; i < length; i++)
        {
            storeNeighboursHexArray[i] = _hexTileDictionary[neighbours[i]];
        }
        
        return storeNeighboursHexArray;
    }

    /// <summary>
    /// 마을 경계선 Hex 길이 정보 획득
    /// </summary>
    /// <param name="storeIndex">마을 인덱스</param>
    /// <returns>경계선 Hex 총 길이</returns>
    public int GetStoreNeighboursLength(int storeIndex)
    {
        if (!_storeNeighboursLength.ContainsKey(storeIndex))
        {
            return -1; // 해당 마을 경계선 Hex가 없다는 의미 (오류처리 용도)
        }
        
      //  print($"{storeIndex} 마을 경계선 길이 : {_storeNeighboursLength[storeIndex]}");
        return _storeNeighboursLength[storeIndex];
    }

    public int GetStoreHexBattleRange(int storeIndex)
    {
        return _storeHexBattleRange[storeIndex];
    }

    #endregion

    #region 퀘스트 관련된 Hex 정보 함수

    /// <summary>
    /// 해당 Hex랑 퀘스트 ID정보를 저장함
    /// </summary>
    /// <param name="questID">퀘스트 ID</param>
    /// <param name="targetQuestHex">퀘스트로 등록된 Hex</param>
    public void SetQuestHexInfo(int questID, Hex targetQuestHex)
    {
        if (!_questHexDictionary.ContainsKey(targetQuestHex))
        {
            _questHexDictionary.Add(targetQuestHex, questID);
        }
        else
        {
            Debug.LogError("이미 할당된 퀘스트 입니다!");
        }
    }

    public struct QuestHexInfo
    {
        public Hex questHex;
        public int questID;
    }
    

    /// <summary>
    /// 해당 Hex위치에 저장된 퀘스트 ID값 확인
    /// </summary>
    /// <param name="targetQuestHex">확인할 Hex</param>
    /// <returns>퀘스트 ID (퀘스트가 존재 하지 않으면 -1을 반환)</returns>
    public QuestHexInfo GetQuestHexID(Hex targetQuestHex)
    {
        QuestHexInfo questHexInfo;

        if (!_questHexDictionary.ContainsKey(targetQuestHex))
        {
            // 퀘스트가 존재하지 않을 경우
            questHexInfo.questID = -1;
            questHexInfo.questHex = null;
            return questHexInfo;
            //Debug.LogError("존재하지 않는 퀘스트 지역 입니다!");
        }

        questHexInfo.questID = _questHexDictionary[targetQuestHex];
        questHexInfo.questHex = targetQuestHex;

        return questHexInfo;
    }


    /// <summary>
    /// 해당 Hex위치에 저장돤 퀘스트 ID값 삭제
    /// </summary>
    /// <param name="targetQuestHex">삭제할 퀘스트 Hex</param>
    public void RemoveQuestHexInfo(Hex targetQuestHex)
    {
        if (!_questHexDictionary.ContainsKey(targetQuestHex))
        {
            Debug.LogError("제거할 수 없는 퀘스트 지역 입니다!");
        }
        else
        {
            // Hex 타입을 기본 값으로 초기화
            if (targetQuestHex.HexType is HexType.QuestZone)
            {
                targetQuestHex.HexType = HexType.Default;
            }

            // 퀘스트 타입을 기본 값으로 초기화
            targetQuestHex.QuestZoneType = QuestZoneType.None;
            _questHexDictionary.Remove(targetQuestHex);
        }
        
    }
    
    #endregion
    

    /// <summary>
    /// 타일 정보 가져오기
    /// </summary>
    /// <param name="hexCoordinates">가져오고자는 타일 좌표</param>
    /// <returns>타일 정보</returns>
    public Hex GetTileAt(Vector3Int hexCoordinates)
    {
        _hexTileDictionary.TryGetValue(hexCoordinates, out Hex result);
        return result;
    }



    public List<Vector3Int> GetNeighboursFor(Vector3Int hexCoordinates)
    {
        // Hex의 주변좌표를 가져오는 부분 임계영역을 이용해 하나의 쓰레드만 접근 가능하게 함
        lock (Lock)
        {
            if (_hexTileDictionary.ContainsKey(hexCoordinates) == false)
                return new List<Vector3Int>();

            if (_hexTileNeighboursDictionary.ContainsKey(hexCoordinates))
                return _hexTileNeighboursDictionary[hexCoordinates];
            
            _hexTileNeighboursDictionary.Add(hexCoordinates, new List<Vector3Int>());
        
            foreach (var direction in HexDirection.GetDirectionList(hexCoordinates.z))
            {
                if (_hexTileDictionary.ContainsKey(hexCoordinates + direction))
                {
                    _hexTileNeighboursDictionary[hexCoordinates].Add(hexCoordinates + direction);
                }
            }
            
            return _hexTileNeighboursDictionary[hexCoordinates];
        }
    }

    public Vector3Int GetClosestHex(Vector3 worldPosition)
    {
        worldPosition.y = 0;
        return HexCoordinates.ConvertPositionToOffSet(worldPosition);
    }
}

public static class HexDirection
{
    /* 짝수 Hex, 홀수 Hex 방향이 다르므로 2개의 타입으로 방향을 구분해야함 */

    // 홀수 방향 오프셋
    public static List<Vector3Int> directionsOffSetOdd = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 1), // N1
        new Vector3Int(0, 0, 1), // N2
        new Vector3Int(1, 0, 0), // E
        new Vector3Int(0, 0, -1), // S2
        new Vector3Int(-1, 0, -1), // S1
        new Vector3Int(-1, 0, 0), // W
    };
    
    // 짝수 방향 오프셋
    public static List<Vector3Int> directionsOffSetEven = new List<Vector3Int>
    {
        new Vector3Int(0, 0, 1), // N1
        new Vector3Int(1, 0, 1), // N2
        new Vector3Int(1, 0, 0), // E
        new Vector3Int(1, 0, -1), // S2
        new Vector3Int(0, 0, -1), // S1
        new Vector3Int(-1, 0, 0), // W
    };

    /// <summary>
    /// 홀, 짝 방향 오프셋 구하는 함수
    /// </summary>
    /// <param name="z">현재 Hex 타일의 Z 좌표 값</param>
    /// <returns>방향 오프셋 데이터 구조 반환</returns>
    public static List<Vector3Int> GetDirectionList(int z)
    {
        return z % 2 == 0 ? directionsOffSetEven : directionsOffSetOdd;
    }
}
