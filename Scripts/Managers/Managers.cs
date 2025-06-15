using System;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;
using Random = UnityEngine.Random;

public class Managers : MonoBehaviour
{
    /* <주 목표>
     * 1. 게임에 라이프 사이클을 관리함
     * 2, 게임에 필요한 매니저 기능들은 모두 참조하고 있어야함
     * 3. 매니저 초기화가 필요할 경우 해당 클래스를 이용해서 초기화 진행
     */
    
    #region 생성부
    
    private static Managers _instance;

    private DataManager _dataManager = new DataManager();
    
    private WorldMapGenerator _worldMapGenerator;
    
    private readonly TurnManager _turnManager = new TurnManager();

    private UIManager _uiManager;
    
    private BattleManager _battleManager;

    private ResourceManager _resourceManager;// = new ResourceManager();
    
    private CameraManager _cameraManager;
    
    private SoundManager _soundManager;

    private CaveManager _caveManager;

    private StoreManager _storeManager = new StoreManager();
    
    private QuestManager _questManager = new QuestManager();

    private FloatingManager _floatingManager = new FloatingManager();

    private InventoryManager _inventoryManager = new InventoryManager();

    #endregion

    #region 참조부

    public static Managers Instance => _instance;

    public static DataManager Data => _instance._dataManager;
    
    public static WorldMapGenerator WorldMapGenerator => _instance._worldMapGenerator;
    
    public static TurnManager Turn => _instance._turnManager;

    public static UIManager UIManager => _instance._uiManager;
    
    public static BattleManager Battle => _instance._battleManager;
    
    public static ResourceManager Resource => _instance._resourceManager;
    
    public static CameraManager Camera => _instance._cameraManager;
    
    public static SoundManager Sound => _instance._soundManager;
    
    public static StoreManager Store => _instance._storeManager;
    
    public static QuestManager Quest => _instance._questManager;
    
    public static FloatingManager Floating => _instance._floatingManager;

    public static InventoryManager Inventory => _instance._inventoryManager;

    public static CaveManager Cave => _instance._caveManager;

    #endregion

    #region 플레이어 정보

    private List<WorldMapPlayerCharacter> _playerList;
    public int GetPlayerCount => _playerList.Count;
    
    public WorldMapPlayerCharacter GetPlayer(int index)
    {
        return _playerList[index];
    }

    /// <summary>
    /// 현재 캐릭터 인덱스 반환
    /// </summary>
    /// <param name="worldMapPlayerCharacter"></param>
    /// <returns></returns>
    public int GetPlayerIndex(WorldMapPlayerCharacter worldMapPlayerCharacter)
    {
        for (int i = 0; i < _playerList.Count; i++)
        {
            if (_playerList[i].Equals(worldMapPlayerCharacter))
            {
                return i;
            }
        }

        Debug.LogError($"{worldMapPlayerCharacter} 캐릭터 정보는 존재하지 않는 정보입니다!");
        return -1; // 캐릭터를 찾을 수 없는 경우 -1 반환
    }

    #endregion

    #region 디버깅 옵션 (테스트 용도)

    [SerializeField] private DebugOption debugOption;

    #endregion
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        InitializeManagers();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// 매니저 참조 함수 (1회 호출)
    /// </summary>
    private void InitializeManagers()
    {
        _resourceManager = new ResourceManager();
        // 월드맵 생성기 싱글톤 캐싱
        _worldMapGenerator = GetComponentInChildren<WorldMapGenerator>();

        // UI 매니저 싱글톤 캐싱
        _uiManager = GetComponentInChildren<UIManager>();
        
        // 배틀 매니저 싱글톤 캐싱
        _battleManager = GetComponentInChildren<BattleManager>();

        // 던전 매니저 싱글톤 캐싱
        _caveManager = GetComponentInChildren<CaveManager>();

        // 카메라 매니저 싱글톤 캐싱
        _cameraManager = GetComponentInChildren<CameraManager>();
        
        // 카메라 매니저 초기화
        _cameraManager.Init();

        // 사운드 매니저 싱글톤 캐싱
        _soundManager = GetComponentInChildren<SoundManager>();
        
        // 사운드 매니저 초기화
        _soundManager.Init();

        //플로팅매니저 초기화
        _floatingManager.Init();
        
        StartCoroutine(_worldMapGenerator.MapGeneratorCoroutine((successMapGenerator) =>
       {
           if (successMapGenerator)
           {
               // UI 모두 초기화
               _uiManager.UIInit();

               // 플레이어 턴 시작
               _turnManager.WorldMapPlayerTurnInit(_playerList);

               // 상점 매니저 초기화
               _storeManager.SetNewItemToStoreList();
               
               // 퀘스트 매니저 초기화
               _questManager.Init();
               
               _cameraManager.FadeOut.FadeIn(3.5f, Color.black);
               _soundManager.PlayBGM("VillageBGM");
               _uiManager.ChangePlayerTurnText(_playerList[0].PlayerStats.BaseClassName);
               
               Instance._worldMapGenerator.RemoveHexCloud(_playerList[0].LastHex);
               
               // 플레이어 인벤토리 초기화
                for(int i=0; i<GetPlayerCount; i++) GetPlayer(i).PlayerInventoryInit();
                
                // 인벤토리 Dummy 카메라 초기화
                _cameraManager.InitInventoryDummyCamera(_playerList.ToArray());

                // 디버깅 옵션 (테스트 용도)
               debugOption.Init();
           }
       }));
    }
    /// <summary>
    /// 플레이어를 가장 처음 마을에 배치함
    /// </summary>
    /// <param name="firstStoreHex">첫 마을 지형</param>
    public void InitPlayerPosition(Hex firstStoreHex)
    {
        var playerCount = GameLogic.Instance.GetMaxPlayer();
        _playerList = new List<WorldMapPlayerCharacter>(playerCount);
        
        for (int i = 0; i < playerCount; i++)
        {
            PlayerStats playerStats = _dataManager.GetPlayerStatsInfo(GameLogic.Instance.GetPlayerCharacterInfo(i).CharacterID); 
            playerStats.BaseClassName = GameLogic.Instance.GetPlayerCharacterInfo(i).NickName; // 새로 추가
            GameObject player = _resourceManager.LoadResource<GameObject>(ResourceManager.ResourcePath.WorldMapPlayer, GameLogic.Instance.GetPlayerCharacterInfo(i).CharacterID + "_World" + Enum.GetValues(typeof(DataManager.PlayerCharacter)).GetValue(playerStats.PlayerID));
         
            _playerList.Add(Instantiate(player, firstStoreHex.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<WorldMapPlayerCharacter>());
            _playerList[i].WorldMapPlayerInit(playerStats);
            
            firstStoreHex.OnPlayerEnterHex(_playerList[i]);
            _playerList[i].LastHex = firstStoreHex;
        }
    }
    
    // 플레이어 생성 (디버깅 용도)
    public void DebugModeInitPlayerPosition(Hex firstStoreHex)
    {
        var playerCount = 3;
        _playerList = new List<WorldMapPlayerCharacter>(playerCount);
        
        for (int i = 0; i < playerCount; i++)
        {
            PlayerStats playerStats = _dataManager.GetPlayerStatsInfo(i);
            GameObject player = _resourceManager.LoadResource<GameObject>(ResourceManager.ResourcePath.WorldMapPlayer, i.ToString() + "_World" + Enum.GetValues(typeof(DataManager.PlayerCharacter)).GetValue(playerStats.PlayerID));

            _playerList.Add(Instantiate(player, firstStoreHex.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<WorldMapPlayerCharacter>());
            _playerList[i].WorldMapPlayerInit(playerStats);
            
            firstStoreHex.OnPlayerEnterHex(_playerList[i]);
            _playerList[i].LastHex = firstStoreHex;
        }
    }
}
