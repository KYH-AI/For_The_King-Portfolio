using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class WorldMapGenerator : MonoBehaviour
{
    /* <주 목표>
     *  1. 가로 x 세로 크기의 바다 타일으로 배치한다.
     *  2. 랜덤으로 Seed 지형을 생성한다.
     *  3. Seed 지형은 마을의 위치가 된다.
     *  4. Seed 지형 기준으로 마을 컨셉에 알맞는 지형을 생성한다.
     *
     *  5. 마을 지형 주변으로 마을 -> 성소 -> 광산 -> 장애물 -> 몬스터 -> 데코레이션 오브젝트를 생성한다.
     */

    [Header("BFS 계산")]
    [SerializeField] private MovementSystem m;
    
    #region 초기 지형 변수 
    
    private const int HEX_TILE_MAIN_ID = 0;
    
    
    [Header("지형 좌표 정보")]
    [SerializeField] private HexGrid _hexGrid;
    
    [Header("맵 크기 (너비 x 높이)")] // 40, 20 기본 값
    [SerializeField] private int _mapWidth = 40;
    [SerializeField] private int _mapHeight = 20;

    [Header("지형 개수")] // 마을 갯수
    [SerializeField] private int _masses = 15;
    
    [Header("지형 정리용 오브젝트")]
    [SerializeField] private Transform _childHexGround;     // Hex 오브젝트들을 해당 오브젝트 자식으로 설정

    [Header("Seed 설정")] 
    [SerializeField] private Vector3 _grow = new Vector3(4f, 0f, 7f); // 4, 0, 7 기본 값
    [SerializeField] private int _frequency = 3;                            // 3 기본 값
    
    private int searchSize = 4;  // 마을 지형 생성 실패 시 반복횟수 

    #endregion
    
    #region 타일 이벤트 게임오브젝트

    public const int HEX_TILE_PROB_ID = 1;
    private const int HEX_TILE_CLOUD_ID = 2;
    private const int HEX_TILE_DECO_ID = 4;
    
    // 마을로 부터 배치 거리
    private readonly int _BATTLE_RANGE = 10;//7;     // 적군 배치 거리 
    private readonly int _SANCTUARY_RANGE = 10; // 성소 배치 거리
    private readonly int _CAVE_RANGE = 10;      // 동굴 배치 거리
    private readonly int _STORE_RANGE = 11;

    private readonly EnemyGenerator _enemyGenerator = new EnemyGenerator();
    
    // Hex 이벤트 오브젝트 리스트
    private List<IEventInfo> _eventInfos = new List<IEventInfo>();

    [Header("Hex 타일 기본 뼈대")]
    [SerializeField] private GameObject _hexTilePrefab;
    
    [Header("Hex 머티리얼(지형) 오브젝트")]
    [SerializeField] private GameObject[] _hexMainPrefabs;

    [Header("Hex 마우스 커서 연출")] 
    [SerializeField] private Transform _hexCursor;
    
    
    // 도시 이름
    private enum StoreName
    {
        Caramba = 0,
        Arkeon = 1,
        Novacity = 2,
        Neocity = 3,
        Coralis = 4,
        Blueward = 5,
        Skyheight = 6
    }
    
    // Hex 장애물 오브젝트 프리팹 경로
    private readonly string _HEX_OBSTACLE_PREFAB_PATH ="Prefabs/Hex/HexProb/Obstacle/";
    
    // Hex 데코레이션 오브젝트 프리팹 경로
    private readonly string _HEX_DECORATION_PREFAB_PATH ="Prefabs/Hex/HexProb/Decoration/";
    
    #endregion

    #region 월드 맵 초기화 함수
    
    
    /// <summary>
    /// 월드 맵 생성기 초기화 (1회 호출)
    /// </summary>
    public bool MapGeneratorInit()
    {
        // 초기 맵 생성 
        MapGenerator();
        
        //상점 지역 설정
        if (!SeedsGenerator())
        {
            // 상점 지역 생성 실패 시 다시 모든 맵을 재생성
            searchSize += 1;
            DestroyAllHexGround();
            return false;
        }
        
        // 연출 옵션 설정
        HexGlowHighLightInit();
        
        SetStoreHexProb();
        SetSanctuaryHexProb();
        SetCaveHexProb();
        SetObstacleHexProb();
        SetBattleZoneProb();
        
        // 모든 지형 세팅이 종료됨 이제부터 플레이어들을 가장 첫 번째 마을에 배치
        Managers.Instance.InitPlayerPosition(_hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(0)));
        return true;
    }

    public IEnumerator MapGeneratorCoroutine(Action<bool> onComplete)
    {
        bool success = false;
        while (!success)
        {
            MapGenerator();
            yield return new WaitForSeconds(0.1f);
            success = SeedsGenerator();
            yield return new WaitForSeconds(0.1f);
            if (!success)
            {
                searchSize += 1;
                DestroyAllHexGround();
            }
            yield return new WaitForSeconds(0.1f);
        }
        
        // 연출 옵션 설정
        HexGlowHighLightInit();
        
        SetStoreHexProb();
        SetSanctuaryHexProb();
        SetCaveHexProb();
        SetObstacleHexProb();
        SetBattleZoneProb();
        SetDecorationHexProb();
        
        // 모든 지형 세팅이 종료됨 이제부터 플레이어들을 가장 첫 번째 마을에 배치
        Managers.Instance.InitPlayerPosition(_hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(0)));
        
        // 디버깅 모드 (메인 화면 씬 생략) * 23/11/09 종설 최종 발표 *
        //Managers.Instance.DebugModeInitPlayerPosition(_hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(0)));
        
        onComplete?.Invoke(true);
    }
    /// <summary>
    /// 맵 생성에 실패 시 삭제 후 다시 맵 생성
    /// </summary>
    private void DestroyAllHexGround()
    {
        _hexGrid.AllClearHexInfo();
        
        foreach(Transform childHex in _childHexGround)
        {
            Destroy(childHex.gameObject);
        }
    }
    
    /// <summary>
    /// 바다 타일 생성
    /// </summary>
    private void MapGenerator()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int z = 0; z < _mapHeight; z++)
            {
                Vector3 hexTilePosition = z % 2 == 0
                    ? new Vector3(x * HexCoordinates.xOffSet, 0, z * HexCoordinates.zOffSet)
                    : new Vector3(x * HexCoordinates.xOffSet + HexCoordinates.xOffSet / 2, 0, z * HexCoordinates.zOffSet);
                
                // 정리된 좌표를 이용해 바다 타일 오브젝트를 생성한다.
                var hexTile=Instantiate(_hexTilePrefab, hexTilePosition, Quaternion.identity);
                // 모든 Hex 오브젝트 부모설정
                hexTile.transform.SetParent(_childHexGround); 
            }
        }
        // 기본적인 Hex가 배치되었으니 모든 Hex좌표를 할당
        _hexGrid.SetHexCoords();
    }

    /// <summary>
    /// 마을 지형 타일 생성
    /// </summary>
    private bool SeedsGenerator()
    {
        int massTemp = _masses;
        int storeIndex = 0;
        int attempts = 0;
        
        while (massTemp > 0 && attempts < _masses * searchSize)   // while (storeIndex < 5 && attempts < _masses * 2) // 
        {
            attempts += 1;
            Vector3 pos = new Vector3(MathF.Round(Random.value * _mapWidth - 1), 0,
                                       MathF.Round(Random.value * _mapHeight - 1));
            
            if (pos.z % 2 == 0)
            {
                pos.x += HexCoordinates.xOffSet;
            }

            Vector3Int vector3Int = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
            Hex hex = _hexGrid.GetTileAt(vector3Int);
            // Hex 지형이 물 지형인 경우 초원 지형으로 변경
            if (hex)
            {
                if (hex.HexType != HexType.Water)continue;
                
                var waterHex = hex.transform.GetChild(HEX_TILE_MAIN_ID).GetChild(0).gameObject;
        
                if (waterHex != null)
                {
                    // Hex_Tile 비활성화 (1 프레임 때문에 비활성 후 삭제함 삭제는 HexGlowHighLightInit()함수에서 진행 )
                    //  DestroyImmediate() 함수를 이용해서 해결은 가능하지만 Prefab이 삭제되는 위험이 있음
                    waterHex.SetActive(false); 
                }
            
                Instantiate(_hexMainPrefabs[0], hex.transform.GetChild(HEX_TILE_MAIN_ID));
                hex.HexType = HexType.Default;

                // Hex에게 Seed 설정 값 전달
                hex.Grow = Mathf.RoundToInt(_grow.x + Random.value * (_grow.z - _grow.x));
                hex.Frequency = _frequency;
                hex.Width = (int)_mapWidth;
                
                // 마을 Hex 좌표 저장
                _hexGrid.SetStoreHexCoord(hex.HexCoords);
                
                SeedGrow(hex, storeIndex++);
                massTemp--;
            }
        }
        
        // 상점지형을 모두 할당하지 못 한경우는 다시 맵 생성을 진행함
        if (!StoreHexGeneratorCheck(Managers.Store.GetStoreLength()))//Managers.Data.GetDataLength(DataManager.DataType.Store)))
        {
            //Debug.LogError($"맵 제작에 실패함 마지막 상점인덱스 {storeIndex}");
            return false;
        }
        return true;
    }
    
    
    /* 세마포어의 전투 흔적들
    private Semaphore _semaphore = new Semaphore(1, 1);
    private readonly object _lock = new object();
    private List<Vector3Int> GetNeighboursFor(Vector3Int coords)
    {
     
        lock (_lock)
        {
            List<Vector3Int> neighbours = _hexGrid.GetNeighboursFor(coords);
            return neighbours;
        }
    }
    */
    
    
    /// <summary>
    /// 마을 주변에 생성되는 지형 생성
    /// </summary>
    /// <param name="hex">기준이 될 지형</param>
    private void SeedGrow(Hex hex, int storeIndex)
    {
        // 기준 지형에 대한 Seed 정보를 가져온다.
        var frequency = hex.Frequency;
        Vector3Int position = hex.HexCoords;
        
        // 선택된 지형에 6방향 좌표를 가져온다.
        List<Vector3Int> coords = new List<Vector3Int>(); //= HexDirection.GetDirectionList(position.z); //GetNeighboursFor(hex.HexCoords);

        foreach (var direction in HexDirection.GetDirectionList(position.z))
        {
            // 6방향 좌표를 합쳐서 주변에 있는 Hex 좌표로 변환
             coords.Add(direction + position);
        }
        
        // Seed 정보에 맞는 지형 횟수 만큼 반복하여 지형들을 생성한다.
        for (int n = 0; n < frequency; n++)
        {
            // 6방향중 랜덤으로 추가로 생성될 지형방향을 난수로 뽑는다.
            int dir = Random.Range(0, coords.Count);
            
            // 해당 방향에 생성될 지형의 위치(x)값이 맵 밖으로 나올경우 예외처리
            if (coords[dir].x < 0)
            {
                coords[dir] = new Vector3Int((int)(coords[dir].x + _mapWidth), 0, coords[dir].z);
            }
            else if (coords[dir].x >= _mapWidth)
            {
                coords[dir] = new Vector3Int((int)(coords[dir].x - _mapWidth), 0, coords[dir].z);
            }
            
            // 생성될 지형의 위치에 대한 지형 오브젝트를 가져온다 (기본적으로 모든 지형은 바다로 설정되어 있음)
            Hex newHex = _hexGrid.GetTileAt(coords[dir]);

            // Hex 지형이 물 지형인 경우 초원 지형으로 변경
            if (newHex)
            {
                if (newHex.HexType != HexType.Water) continue;
                
                var waterHex = newHex.transform.GetChild(HEX_TILE_MAIN_ID).GetChild(0).gameObject;
        
                if (waterHex != null)
                {
                    waterHex.SetActive(false); // Hex_Tile 비활성화 (프레임 때문에 비활성 후 삭제함)
                }
            
                Instantiate(_hexMainPrefabs[1], newHex.transform.GetChild(HEX_TILE_MAIN_ID));
                newHex.HexType = HexType.Default;
                
                _hexGrid.SetStoreNeighboursHex(storeIndex, newHex.HexCoords);
                
                // 기본 지형을 리스트에 추가
                // _hexGrid.DefaultHexes.Add(newHex);

                // 재귀함수의 종료조건 
                if (hex.Grow - 1 > 0)
                {
                    newHex.Grow = hex.Grow - 1;
                    newHex.Frequency = _frequency;
                    newHex.Width = hex.Width;

                    // 재귀함수로 호출해 현재 지형에서 또 추가적인 지형을 생성한다.
                    SeedGrow(newHex, storeIndex);
                }
            }
        }
    }

    /// <summary>
    /// 마을 지형 생서 유뮤 확인
    /// </summary>
    /// <param name="totalStoreCount">총 마을 횟수</param>
    /// <returns>true : 성공, false : 실패</returns>
    private bool StoreHexGeneratorCheck(int totalStoreCount)
    {
        // 마을의 Key 값이 모두 있는지 확인한다. 만약 해당 마을 인덱스(Key) 값이 없는경우 -1을 반환해서 마을 지형 생성에 실패했다는 의미
        for (int i = 0; i < totalStoreCount; i++)
        {
            // 최소의 마을 범위 보정을 해줘야함 (성소, 광산 등 생성을 위해서)
            if (_hexGrid.GetStoreNeighboursLength(i) <= 8)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 타일 오브젝트 GlowHighLight 초기화
    /// </summary>
    private void HexGlowHighLightInit()
    {
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int z = 0; z < _mapHeight; z++)
            {
                Vector3Int hexPosition = new Vector3Int(x, 0, z);
                var hexTile = _hexGrid.GetTileAt(hexPosition);
                
                if(hexTile == null) continue;
                
                // 타일 선택 광원효과 초기화 설정 
                hexTile.GetComponent<GlowHighlight>().Init();
                
                // 육지 타일인데 바다 타일 오브젝트를 자식으로 가진 경우를 확인해 삭제함
                if(hexTile.transform.GetChild(HEX_TILE_MAIN_ID).childCount > 1) 
                    Destroy(hexTile.transform.GetChild(HEX_TILE_MAIN_ID).GetChild(0).gameObject);
            }
        }
    }
    
    #endregion

    #region 월드 맵 이벤트 오브젝트 생성 함수

    /// <summary>
    /// 마을 오브젝트 생성
    /// </summary>
    private void SetStoreHexProb()
    {

        var hexStoreLength = Managers.Store.GetStoreLength();//Managers.Data.GetDataLength(DataManager.DataType.Store);
        for (int i = 0; i < hexStoreLength; i++)
        {
            var hexInfo = Managers.Store.GetStoreInfo(i);
            Hex hex = _hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(i));
            var storePrefabName = Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.Store, hexInfo.GetEventID() + "_" + (DataManager.StoreID)hexInfo.GetEventID(), false);

            var storeProbPrefab  = Instantiate(storePrefabName, hex.transform.GetChild(HEX_TILE_PROB_ID));
            storeProbPrefab.name = storePrefabName.name;    // 프리팹 오브젝트에 (Clone) 이름 제거
            _eventInfos.Add(hexInfo);                       // 타일 타입 오브젝트 생성
            hex.HexType = HexType.Store;                      // 타일 타입 설정
            hex.SetEventList(_eventInfos);                   // 타일에게 생성된 이벤트 오브젝트 리스트 전달
            _eventInfos.Clear();                             // 다시 리스트를 재활용하기 위해 초기화
        }
    }

    /// <summary>
    /// 동굴 오브젝트 생성
    /// </summary>
    private void SetCaveHexProb()
    {
        
        /*  마을 지형의 위치 정보를 찾자
         *  위치 정보를 이용해 BFS을 이용해 반경 _CAVE_RANGE까지 주변 위치 정보를 배열로 담자
          *  랜덤으로 배열의 인덱스를 결정해 해당 위치 지형에 던전을 생성하자
         */
        

        var hexStoreLength = _hexGrid.StoreHexLength;
        for (int i = 0; i < hexStoreLength; i++)
        {
            var aroundHex = m.GetAroundHexBFS(_CAVE_RANGE, _hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(i)), _hexGrid);
            var aroundHexLength = aroundHex.Count;
            
            int randomPosition = Random.Range(5, aroundHexLength);
            Hex caveHex = _hexGrid.GetTileAt(aroundHex[randomPosition]);
            
            if(caveHex.HexType != HexType.Default) continue;

            var sanctuaryPrefabName = Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.Cave, "0_Cave", false);
            var sanctuaryPrefab = Instantiate(sanctuaryPrefabName, caveHex.transform.GetChild(HEX_TILE_PROB_ID));
            sanctuaryPrefab.name = sanctuaryPrefabName.name;         // 프리팹 오브젝트에 (Clone) 이름 제거
            _eventInfos.Add(Managers.Data.GetCaveInfo(0));       // 타일 타입 오브젝트 생성
            caveHex.HexType = HexType.Cave;                         // 타일 타입 설정
            caveHex.SetEventList(_eventInfos);                       // 타일에게 생성된 이벤트 오브젝트 리스트 전달
            _eventInfos.Clear();                                     // 다시 리스트를 재활용하기 위해 초기화
        }
    }

    /// <summary>
    /// 성소 오브젝트 생성
    /// </summary>
    private void SetSanctuaryHexProb()
    {
        
        var sanctuaryHexCount = Managers.Data.GetDataLength(DataManager.DataType.Sanctuary);
        int currentSanctuaryCount = 0;
        while(currentSanctuaryCount < sanctuaryHexCount)  // 성소 갯수 만큼 진행
        {
            var aroundHex = m.GetAroundHexBFS(_SANCTUARY_RANGE, _hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(currentSanctuaryCount)), _hexGrid);
            var aroundHexLength = aroundHex.Count;
            
            int randomPosition = Random.Range(8, aroundHexLength);
            Hex sanctuaryHex = _hexGrid.GetTileAt(aroundHex[randomPosition]);
            
            // 랜덤으로 정해진 Hex 타입이 기본 지형이 아니면 성소 생성에 Skip
            if(sanctuaryHex.HexType != HexType.Default) continue;

            var sanctuaryInfo = Managers.Data.GetSanctuaryInfo(currentSanctuaryCount);
            var sanctuaryPrefabName = Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.Sanctuary, sanctuaryInfo.GetEventID() + "_" + (DataManager.SanctuaryID)sanctuaryInfo.GetEventID(), false);
 
            
            var sanctuaryPrefab = Instantiate(sanctuaryPrefabName, sanctuaryHex.transform.GetChild(HEX_TILE_PROB_ID));
            sanctuaryPrefab.name = sanctuaryPrefabName.name;         // 프리팹 오브젝트에 (Clone) 이름 제거
            _eventInfos.Add(sanctuaryInfo);                         // 타일 성소 이벤트 등록
            sanctuaryHex.HexType = HexType.Sanctuary;              // 타일 타입 설정
            sanctuaryHex.SetEventList(_eventInfos);                 // 타일에게 생성된 이벤트 오브젝트 리스트 전달
            _eventInfos.Clear();;                                  // 다시 리스트를 재활용하기 위해 초기화
            currentSanctuaryCount++;
        }
    }

    /// <summary>
    /// 장애물 오브젝트 생성
    /// </summary>
    private void SetObstacleHexProb()
    {
        /*  마을 지형의 위치 정보를 찾자
         *  마을의 경계선까지 장애물을 배치하자 (랜덤으로 할까?)
        */
        
        var hexStoreLength = _hexGrid.StoreHexLength;
        string storeName;

        for (int i = 0; i < hexStoreLength; i++)
        {
            
          //  storeName = Enum.GetName(typeof(StoreName), 0);
            // 다른 지형 장애물 배치되면 0을 i로 바꿈
            if (i == 5)
            {
                storeName = "Coralis";
            }
            else
            {
                storeName = Enum.GetName(typeof(StoreName), i);
            }

            
            // 마을 경계선 개수 와 경계선을 구성한 Hex 배열 정보 가져오기
            var aroundHexLength = _hexGrid.GetStoreNeighboursLength(i); 
            var aroundHexArray = _hexGrid.GetStoreNeighboursHex(i);

           for (int k = 0; k < aroundHexLength; k++)
            {               
             //   int randomPosition = Random.Range(1, aroundHexLength);
                Hex obstacleHex = aroundHexArray[k];

                // 랜덤으로 정해진 Hex 타입이 기본 지형이 아니거나 또는 25% 확률에 도달하지 못했으면 장애물 생성 Skip
                if(obstacleHex.HexType != HexType.Default || Random.value < 0.75f) continue;
                
                // Hex Tile Prob 오브젝트 위에 배치
               // Instantiate(Resources.Load<GameObject>(_HEX_OBSTACLE_PREFAB_PATH+ (i+1) +"_Obstacle"), obstacleHex.transform.GetChild(HEX_TILE_PROB_ID));
               
               // Assets/Resources/Prefabs/Hex/HexProb/Obstacle/Caramba/CarambaObstacle0.prefab
               Instantiate(Resources.Load<GameObject>(_HEX_OBSTACLE_PREFAB_PATH+ storeName +"/" +storeName + "Obstacle" + Random.Range(0, 3)), obstacleHex.transform.GetChild(HEX_TILE_PROB_ID));
                // Hex 타입을 장애물 지역으로 설정
                obstacleHex.HexType = HexType.Obstacle;
            }
        }
    }
    
    /// <summary>
    /// 환경 오브젝트 생성
    /// </summary>
    private void SetDecorationHexProb()
    {
        var hexStoreLength = _hexGrid.StoreHexLength;
        string storeName;

        for (int i = 0; i < hexStoreLength; i++)
        {
            if (i == 5)
            {
                storeName = "Coralis";
            }
            else
            {
                storeName = Enum.GetName(typeof(StoreName), i);
            }
            
            // 마을 경계선 개수 와 경계선을 구성한 Hex 배열 정보 가져오기
            var aroundHexLength = _hexGrid.GetStoreNeighboursLength(i); 
            var aroundHexArray = _hexGrid.GetStoreNeighboursHex(i);

            for (int k = 0; k < aroundHexLength; k++)
            {
                Hex obstacleHex = aroundHexArray[k];

                // 랜덤으로 정해진 Hex 타입이 기본 지형이 아니거나 또는 35% 확률에 도달하지 못했으면 데코레이션 오브젝트에 Skip
                if(obstacleHex.HexType != HexType.Default || Random.value < 0.75f) continue;
                
                //Assets/Resources/Prefabs/Hex/HexProb/Decoration/Caramba/CarambaDecoration0.prefab
                Instantiate(Resources.Load<GameObject>(_HEX_DECORATION_PREFAB_PATH+ storeName +"/" +storeName + "Decoration" + Random.Range(0, 2)), obstacleHex.transform.GetChild(HEX_TILE_DECO_ID));
            }
        }
    }

    /// <summary>
    /// 적 오브젝트 생성
    /// </summary>
    private void SetBattleZoneProb()
    {
        /*  마을 지형의 위치 정보를 찾자
         *  위치 정보를 이용해 BFS을 이용해 반경 _BATTLE_RANGE까지 주변 위치 정보를 배열로 담자
          *  랜덤으로 배열의 인덱스를 결정해 해당 위치 지형에 몬스터를 생성하자
         */
        

        var hexStoreLength = _hexGrid.StoreHexLength;
        for (int i = 0; i < hexStoreLength; i++)
        {
            var aroundHex = m.GetAroundHexBFS(_BATTLE_RANGE, _hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(i)), _hexGrid);
            var aroundHexLength = aroundHex.Count;
            for (int k = 0; k < aroundHexLength; k++)
            {               
                int randomPosition = Random.Range(0, aroundHexLength);
                Hex battleHex = _hexGrid.GetTileAt(aroundHex[randomPosition]);
                
                // 랜덤으로 정해진 Hex 타입이 기본 지형이 아니거나 또는 10% 확률에 도달하지 못했으면 몬스터 생성에 Skip
                if(battleHex.HexType != HexType.Default || Random.Range(0, 10) < 9) continue;
                
                int testID = RandomEnemyID();

                if (Random.value < 0.15f)// 15% 확률 Group
                {
                    SetEnemyGroup((DataManager.EnemyCharacter)testID, battleHex);
                }
                else if (Random.value < 0.1f) // 10% 확률 Camp
                {
                    SetEnemyCamp((DataManager.EnemyCharacter)testID, battleHex);
                }
                else // Solo 
                {
                    SetEnemySolo((DataManager.EnemyCharacter)testID, battleHex);
                }
            }
        }
    }

    public void SetQuestZone(int questID, int storeIndex, bool isBattleQuest = false, BattleZoneType battleZoneType = BattleZoneType.None)
    {
        Hex targetQuestZoneHex = null;
        
        if (!isBattleQuest)
        {
            // 마을 Hex를 찾음
            targetQuestZoneHex = _hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(storeIndex));

            // 기존 마을 HexType은 유지하면서 퀘스트 타입을 "전달" 타입으로 지정하
            targetQuestZoneHex.QuestZoneType = QuestZoneType.Delivery;
        }
        else
        {
            do
            {
                var aroundHex = m.GetAroundHexBFS(_BATTLE_RANGE, _hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition(storeIndex)), _hexGrid);
                int randomPosition = Random.Range(0, aroundHex.Count);
                targetQuestZoneHex = _hexGrid.GetTileAt(aroundHex[randomPosition]);
            } while (targetQuestZoneHex.HexType != HexType.Default);

            switch (battleZoneType)
            {
                case BattleZoneType.Solo :
                    SetEnemySolo((DataManager.EnemyCharacter)RandomEnemyID(), targetQuestZoneHex);
                    break;
                
                case BattleZoneType.Group : 
                    SetEnemyGroup((DataManager.EnemyCharacter)RandomEnemyID(), targetQuestZoneHex);
                    break;
                
                case BattleZoneType.Camp : 
                    SetEnemyCamp((DataManager.EnemyCharacter)RandomEnemyID(), targetQuestZoneHex);
                    break;
            }
            targetQuestZoneHex.HexType = HexType.QuestZone;
            targetQuestZoneHex.QuestZoneType = QuestZoneType.Battle;
        }
        
        _hexGrid.SetQuestHexInfo(questID, targetQuestZoneHex);
        Managers.Camera.ReTargetToWorldCamera(targetQuestZoneHex.transform.position);
        RemoveHexCloud(targetQuestZoneHex);
    }

    int RandomEnemyID()
    {
        int[] enemyID = new int[] { 0, 4, 5, 6, 7, 9 };

        return enemyID[Random.Range(0, enemyID.Length)];;
    }

    public void SetEnemySolo(DataManager.EnemyCharacter enemyID, Hex battleHex)
    {
        // 투입할 적 레벨 범위 Random.Range(i, i+2)
        var enemyInfo = Managers.Data.GetEnemyStatsInfo((int)enemyID);
        // 전투에 투입할 적 오브젝트 리스트 할당 (일단 1명만)
        var enemyList = _enemyGenerator.CreateEnemyPrefab(enemyInfo.EnemyID);
        
        // 첫 번쨰 적 오브젝트만 생성 (대표적으로 Hex Tile 위에 표시)
        // Hex Tile Prob 오브젝트 위에 배치 하고 이벤트 오브젝트 리스트에 넣음
        var enemyPrefab = Instantiate(enemyList, battleHex.transform.GetChild(HEX_TILE_PROB_ID));
        var enemyComponent = enemyPrefab.GetComponent<WorldMapEnemyCharacter>();
        enemyComponent.InitEventInfo(enemyInfo);
        enemyPrefab.name = enemyList.name;                    // 프리팹 오브젝트에 (Clone) 이름 제거
        _eventInfos.Add(enemyComponent);
        // Hex 타입을 전투지역으로 설정
        battleHex.BattleZoneType = BattleZoneType.Solo;
        battleHex.HexType = HexType.BattleZone;
        // Hex 클래스에 적 리스트를 설정
        battleHex.SetEventList(_eventInfos);
        _eventInfos.Clear();
    }
    
    /// <summary>
    /// 랜덤 적 캠프 생성
    /// </summary>
    /// <param name="enemyID">적 대장 ID</param>
    /// <param name="spawnHexPos">스폰할 위치</param>
    private void SetEnemyCamp(DataManager.EnemyCharacter enemyID, Hex spawnHexPos)
    {
        SetEnemyCamp(enemyID, DataManager.EnemyCharacter.None, DataManager.EnemyCharacter.None,
                     false, -1, spawnHexPos);
        
        /*특정 상점 주변에 랜덤으로 생성하고 싶을경우 */
       // SetEnemyCamp(enemyID, DataManager.EnemyCharacter.None, DataManager.EnemyCharacter.None, false, 3);
    }
    
    /// <summary>
    /// 랜덤 적 그룹 생성
    /// </summary>
    /// <param name="enemyID">적 객체 ID</param>
    /// <param name="spawnHexPos">스폰할 위치</param>
    private void SetEnemyGroup(DataManager.EnemyCharacter enemyID, Hex spawnHexPos)
    {
        SetEnemyGroup(enemyID, -1, spawnHexPos);
        
        /*특정 상점 주변에 생성하고 싶을경우 */
        // SetEnemyGroup(enemyID, 3);
    }
    
    
    /// <summary>
    /// 적 캠프 생성 
    /// </summary>
    /// <param name="mainEnemyType">캠프 대장</param>
    /// <param name="subEnemyType1">캠프 부하1 (null 기본)</param>
    /// <param name="subEnemyType2">캠프 부하2 (null 기본)</param>
    /// <param name="isBoss">스커지 확인 (false 기본)</param>
    /// <param name="storeIndex">생성할 상점 위치 기준 (-1 기본 값)</param>
    /// <param name="spawnHexPos">생성할 특정 Hex 위치 (null 기본)</param>
    public void SetEnemyCamp(DataManager.EnemyCharacter mainEnemyType, 
                            DataManager.EnemyCharacter subEnemyType1 = DataManager.EnemyCharacter.None, 
                            DataManager.EnemyCharacter subEnemyType2 = DataManager.EnemyCharacter.None,
                            bool isBoss = false,  int storeIndex = -1,   Hex spawnHexPos = null)
    {
        /* <적군 그룹 생성>
         *  1. 캠프 대장 적 객체 생성
         *  2. 부하 적 객체 생성 (1 ~ 2명, 동일한 객체가 될수도 있음)
         */
        
        if (!spawnHexPos)
        {
            spawnHexPos = GetStoreNeighborBattleZone((DataManager.StoreID)storeIndex);
        }
        

        int[] enemyCampList = { (int)mainEnemyType, (int)subEnemyType1, (int)subEnemyType2, };
        int maxCampSize = 3;
        bool isRandomSubEnemy = true;

        // 서브 적군이 설정하지 않은 경우는 랜덤
        if (subEnemyType1 == DataManager.EnemyCharacter.None && subEnemyType2 == DataManager.EnemyCharacter.None)
        {
            // 2명 ~ 3명까지 (보스포함)
            maxCampSize = Random.Range(2, 4);
            isRandomSubEnemy = true;
        }
        // 둘중 하나라도 서브 적군이 설정되어 있으면 
        else if (subEnemyType1 != DataManager.EnemyCharacter.None)
        {
            // 최대 2명까지 (보스포함)
            maxCampSize = 2;
            isRandomSubEnemy = false;
        }
        

        if (isRandomSubEnemy)
        {
            // 보스를 제외한 나머지 서브적군 설정
            for (int i = 1; i<maxCampSize; i++)
            {
                enemyCampList[i] = RandomEnemyID();
            }
        }
        
        /* 메인 보스 할당 */
        var enemyInfo = Managers.Data.GetEnemyStatsInfo(enemyCampList[0]);
        var enemyPrefab = _enemyGenerator.CreateEnemyPrefab(enemyInfo.EnemyID);
        var enemy = Instantiate(enemyPrefab, spawnHexPos.transform.GetChild(HEX_TILE_PROB_ID));
        var enemyComponent = enemy.GetComponent<WorldMapEnemyCharacter>();
        enemyComponent.InitEventInfo(enemyInfo);
        enemyComponent.SetEventName("캠프");
        _eventInfos.Add(enemyComponent);

        // 캠프 오브젝트 할당
        if (isBoss)
        {
            // TODO : 스커지 전용캠프 할당
        }
        else
        {
            // 해당 적 종족에 맞는 캠프설정
            Instantiate(Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.Camp,
                1 + "_" + (DataManager.CampID)1), spawnHexPos.transform.GetChild(HEX_TILE_PROB_ID));
        }
        
        // 메인 보스는 할당했으므로 제외하고 진행 (/* 서브 적군 할당 */)
        for (int i = 1; i < maxCampSize; i++)
        {
            enemyInfo = Managers.Data.GetEnemyStatsInfo(enemyCampList[i]);
            enemyPrefab = _enemyGenerator.CreateEnemyPrefab(enemyInfo.EnemyID);
            enemyComponent = enemyPrefab.GetComponent<WorldMapEnemyCharacter>();
            enemyComponent.InitEventInfo(enemyInfo);
            _eventInfos.Add(enemyComponent);
        }
        
        // Hex 타입을 전투지역으로 설정
        spawnHexPos.BattleZoneType = BattleZoneType.Camp;
        spawnHexPos.HexType = HexType.BattleZone;
        // Hex 클래스에 적 리스트를 설정
        spawnHexPos.SetEventList(_eventInfos);
        _eventInfos.Clear();
    }

    /// <summary>
    /// 적 그룹 생성
    /// </summary>
    /// <param name="enemyType">그룹 적 객체</param>
    /// <param name="storeIndex">생성할 상점 위치 기준 (-1 기본 값)</param>
    /// <param name="spawnHexPos">생성할 특정 Hex 위치 (null 기본)</param>
    public void SetEnemyGroup(DataManager.EnemyCharacter enemyType, int storeIndex = -1,   Hex spawnHexPos = null)
    {
        /* <적군 그룹 생성>
         *  1. 동일한 적 객체 생성
         */
        
        int maxGroupSize = 3;

        if (!spawnHexPos)
        {
            spawnHexPos = GetStoreNeighborBattleZone((DataManager.StoreID)storeIndex);
        }
        
        var enemyInfo = Managers.Data.GetEnemyStatsInfo((int)enemyType);
        var enemyList = _enemyGenerator.CreateEnemyPrefab(maxGroupSize, enemyInfo.EnemyID);
        
        var enemyPrefab = Instantiate(enemyList[0], spawnHexPos.transform.GetChild(HEX_TILE_PROB_ID));

        for (int i = 0; i < maxGroupSize; i++)
        {
            var enemyComponent = enemyList[i].GetComponent<WorldMapEnemyCharacter>();
            enemyComponent.InitEventInfo(enemyInfo);
            enemyPrefab.name = enemyList[i].name;
            _eventInfos.Add(enemyComponent);
        }
        
        enemyList[0].GetComponent<WorldMapEnemyCharacter>().SetEventName("무리");

        // Hex 타입을 전투지역으로 설정
        spawnHexPos.BattleZoneType = BattleZoneType.Group;
        spawnHexPos.HexType = HexType.BattleZone;
        // Hex 클래스에 적 리스트를 설정
        spawnHexPos.SetEventList(_eventInfos);
        _eventInfos.Clear();
    }


    /// <summary>
    /// 상점 경계선 Hex에서 전투 지역이 가능한 지역을 가져옴
    /// </summary>
    /// <param name="storeID"> 상점 ID </param>
    /// <returns>HexType.Default 값을 가진 Hex</returns>
    private Hex GetStoreNeighborBattleZone(DataManager.StoreID storeID)
    {
        var aroundHex = m.GetAroundHexBFS(_BATTLE_RANGE, _hexGrid.GetTileAt(_hexGrid.GetStoreHexPosition((int)storeID)), _hexGrid);
        var aroundHexLength = aroundHex.Count;

        while (true)
        {
            int randomPosition = Random.Range(0, aroundHexLength);
            var battleHex = _hexGrid.GetTileAt(aroundHex[randomPosition]);
            if (battleHex.HexType is HexType.Default)
            {
                return battleHex;
            }
        }
    }
    
 
    
    
    /// <summary>
    /// 현재 Hex 지형의 기준으로 전투범위 만큼 모든 구름 오브젝트를 비활성화
    /// </summary>
    /// <param name="targetHex">기준이 되는 Hex 지형</param>
    public void RemoveHexCloud(Hex targetHex)
    {
        // 해당 Hex 지형 기준으로 마을의 정보를 가져옴
        var storeHexIndex = UnitManager.Instacne.HexGrid.GetStoreHexIndex(targetHex.HexCoords);
        
        // 해당 상점 지형의 전투범위 (전투범위가 너무 작어서 2배 크게함)
        var battleRange = UnitManager.Instacne.HexGrid.GetStoreHexBattleRange(storeHexIndex) * 2;

        // 기준점 Hex로 주변에 있는 지형들 Get
        var targetHexList = m.GetAroundHexBFS(battleRange, _hexGrid.GetTileAt(targetHex.HexCoords), _hexGrid, false);
        
        foreach (var hexPosition in targetHexList)
        {
            GameObject cloud = _hexGrid.GetTileAt(hexPosition).transform.GetChild(HEX_TILE_CLOUD_ID).GetChild(0).gameObject;
            
            if (!cloud.activeSelf) continue;

            cloud.transform.DOScale(Vector3.zero, 1.5f).OnComplete( () => cloud.SetActive(false) );
        }
    }

    public void SetHexCursor(Vector3 targetHex)
    {
        targetHex.y = 0.95f;
        _hexCursor.position = targetHex;
    }


    #endregion
}
