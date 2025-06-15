using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hex : MonoBehaviour
{
    #region Seed 관련
    
    public int Grow { get; set; } = 1;
    public int Frequency { get; set; } = 1;
    public int Width { get; set; } = 40;
    
    #endregion
    
     private SpriteRenderer _battleOverlay;
     
     private GlowHighlight _highlight;
     
     private HexCoordinates _hexCoordinates;

     [SerializeField] private MeshRenderer hexOverlayMaterial;

     // Hex에 들어올 수 있는 최대 이벤트 개수는 3의 크기를 가짐
     private readonly List<IEventInfo> _eventInfoList = new List<IEventInfo>(3);

     // Hex에 들어올 수 있는 최대 플레이어 리스트는 3의 크기를 가짐
     private List<WorldMapPlayerCharacter> _playersInHexList = new List<WorldMapPlayerCharacter>(3);

     private Action _mouseEnterAction;
     
     private Action _mouseExitAction;

     private Action _hexEventFxParticleAction;

     private HexType _hexType = HexType.Water;

     private BattleZoneType _battleZoneType = BattleZoneType.None;

     /* 퀘스트 종류 : 전투, 특정위치 이동 (물품전달)
      * 1. 전투는 무조건 HexType이 Default인 곳에서 발동                                                          (퀘스트를 완료하면 해당 HexType과 QuestZoneType을 초기화)
      *   1-1. 해당 지역은 HexType을 QuestZone으로 변경
      *   1-2. 변경과 동시에 QuestZoneType을 Battle로 변경
      *   1-3. 변경과 동시에 BattleZoneType을 퀘스트 느낌에 맞게 변경
      * 2. 이동 퀘스트는 HexType이 전투지역, 장애물, 바다를 제외한 곳에서 발동
      *   2-1. 마을 위치, 성소, 동굴인 경우 HexType은 그대로, QuestZoneType만 Delivery로 변경                      (퀘스트를 완료하면 해당 QuestZoneType만 초기화)
      *   2-2. HexType이 Default인 경우 HexType을 QuestZone으로 변경, QuestZoneType만 Delivery로 변경            (퀘스트를 완료하면 해당 HexType과 QuestZoneType을 초기화)
      */
     
     private QuestZoneType _questZoneType = QuestZoneType.None;

     public HexType HexType
     {
         get => _hexType;
         set => SetHexType(value);
     }

     public BattleZoneType BattleZoneType
     {
         get => _battleZoneType;

         set => _battleZoneType = value;
     }

     public QuestZoneType QuestZoneType
     {
         get => _questZoneType;

         set => _questZoneType = value;
     }
     
     public int GetCost { get; private set; } = 1;

     // 현재 Hex 지형에 플레이어 Scale 크기조절 필요여부 확인
     public bool IsScaleDownGroundPlayer { get; private set; } = false;
     
     public Vector3Int HexCoords => _hexCoordinates.GetHexCoords();

    private void Awake()
    {
        _hexCoordinates = GetComponent<HexCoordinates>();
        _highlight = GetComponent<GlowHighlight>();
        _battleOverlay = transform.GetChild(3).GetChild(0).GetComponent<SpriteRenderer>();
        DisableBattleOverlay();
    }
    
    private void SetHexType(HexType hexType, BattleZoneType battleZoneType = BattleZoneType.None)
    {
        _hexType = hexType;
        
        ChangeHexOverlay(_hexType);

        // Hex Type이 장애물이거나 기본 지형 또는 물 지형인 경우 이벤트를 해체
        if (IsObstacle() || hexType == HexType.Water || hexType == HexType.Default)
        {
            // 기존에 있는  MouseUIEvent 이벤트를 해체함
            _mouseEnterAction = null;
            _mouseExitAction = null;
            // 빈 지형이니 플레이어 Scale 연출필요 X
            return;
        }

        // Hex 오브젝트가 장애물과 기본 Type이 아닌경우 마우스 Enter 와 Exit 이벤트를 등록함 (이벤트 형식은 마우스 Enter Type 과 Exit Type)
        BindEvent(() =>
        {
            Managers.UIManager.ShowOnPointerEventPopUpUI(this, Input.mousePosition); // 현재 Hex 정보와 마우스 좌표를 전달
        }, UI_Base.MouseUIEvent.Enter);

        BindEvent(() =>
        {
            Managers.UIManager.StopCoroutineOnPointerPopUp(); // UI 마우스 팝업창 종료
        }, UI_Base.MouseUIEvent.Exit);

        // 빈 지형이 아닌 이벤트 지형이니 플레이어 Scale 연출필요 O (전투지역은 제외)
        IsScaleDownGroundPlayer = _battleZoneType == BattleZoneType.None; //hexType != HexType.BattleZone;
        
        GetCost = 1;
    }
    
    public void SetEventList(List<IEventInfo> eventList)
    {
        foreach (var eventInfo in eventList)
        {
            this._eventInfoList.Add(eventInfo);
        }
    }
    
    public List<IEventInfo> GetEventInfoList()
    {
        return _eventInfoList.ToList();
    }

    /// <summary>
    /// 해당 Hex 이벤트 오브젝트 모두 삭제 후 타입도 기본 타입으로 변경
    /// </summary>
    public void DestroyAllEventObject()
    {
        // 전투지역이 아닐경우 아무 동작 X
        //if (_hexType != HexType.BattleZone) return;

        // 전투지역인 경우만 적군 오브젝트 삭제
        if (_hexType is HexType.BattleZone || _questZoneType is QuestZoneType.Battle)
        {   
            foreach (Transform child in transform.GetChild(WorldMapGenerator.HEX_TILE_PROB_ID))
            {
                Destroy(child.gameObject);
            }
            SetHexType(HexType.Default);
            _battleZoneType = BattleZoneType.None;
        }
        
        // 퀘스트 지역인 경우 퀘스트 지역 정보 모두 초기화
        if(_questZoneType != QuestZoneType.None) UnitManager.Instacne.HexGrid.RemoveQuestHexInfo(this);

        // 이벤트와 관련된 지역인 경우 이벤트 완료 연출 진행
        if (_hexType is HexType.Sanctuary || _hexType is HexType.Event)
        {
            _hexEventFxParticleAction?.Invoke();
            _hexEventFxParticleAction = null;
        }

        _eventInfoList.Clear();
    }

    private void BindEvent(Action action, UI_Base.MouseUIEvent mouseEventType)
    {
        switch (mouseEventType)
        {
            case UI_Base.MouseUIEvent.Enter :
                _mouseEnterAction -= action;
                _mouseEnterAction += action;
                break;
            
            case UI_Base.MouseUIEvent.Exit :
                _mouseExitAction -= action;
                _mouseExitAction += action;
                break;
        }   
    }

    public void OnPlayerEnterHex(WorldMapPlayerCharacter player)
    {
        if (!_playersInHexList.Contains(player))
        {
            _playersInHexList.Add(player);
        }

        player.CurrentHex = this;
    }
    
    public void OnPlayerExitHex(WorldMapPlayerCharacter player)
    {
        if (_playersInHexList.Contains(player))
        {
            _playersInHexList.Remove(player);
        }
        player.LastHex = this;
        player.CurrentHex = null;
    }

    public WorldMapPlayerCharacter GetInHexWorldMapPlayer(int index)
    {
        return _playersInHexList[index];
    }

    public int GetInHexPlayerCount()
    {
        return _playersInHexList.Count;
    }
    
    public bool IsObstacle()
    {
        return _hexType is HexType.Obstacle or HexType.Water;
    }

    #region Hex HighLight 연출 효과

    public void EnableHighlight()
    {
        _highlight.ToggleGlow(true);
    }

    public void DisableHighlight()
    { 
        _highlight.ToggleGlow(false);
    }

    public void ResetHighlight()
    {
        _highlight.ResetGlowHighlight();
    }

    public void HighlightPath()
    {
        _highlight.HighlightValidPath();
    }
    
    /// <summary>
    /// 전투 오버레이 이미지 활성화
    /// </summary>
    public void EnableBattleOverlay()
    {
        _battleOverlay.enabled = true;
    }

    /// <summary>
    /// 전투 오버레이 이미지 비활성화
    /// </summary>
    public void DisableBattleOverlay()
    {
        _battleOverlay.enabled = false;
    }

    /// <summary>
    /// Hex 지형 오버레이 머리티얼 변경
    /// </summary>
    /// <param name="hexEventType">Hex 지형 이벤트 종류</param>
    public void ChangeHexOverlay(HexType hexEventType)
    {
        switch (hexEventType)
        {
            case HexType.Default:
                hexOverlayMaterial.material.SetColor("_TintColor", ColorUitl.HEX_DEFAULT_OVERLAY_COLOR); break;
            
            case HexType.Store:
                hexOverlayMaterial.material.SetColor("_TintColor", ColorUitl.HEX_STORE_OVERLAY_COLOR); break;
            
           case HexType.QuestZone: break;
           
           case HexType.Cave:
               hexOverlayMaterial.material.SetColor("_TintColor", ColorUitl.HEX_CAVE_OVERLAY_COLOR); break;
           
            case HexType.BattleZone: 
                hexOverlayMaterial.material.SetColor("_TintColor", ColorUitl.HEX_BATTLE_OVERLAY_COLOR); break;
            
            case HexType.Sanctuary :
            case HexType.Event:
                hexOverlayMaterial.material = Managers.Resource.LoadResource<Material>(ResourceManager.ResourcePath.HexOverlay, 
                                                                            "HexOverlayUnknown", 
                                                                                false);
                GameObject eventFxGameObject = Instantiate(Managers.Resource.LoadResource<GameObject>(ResourceManager.ResourcePath.HexOverlay, 
                                                                  "HexEventFxHighLight", 
                                                                      false), 
                                                                            hexOverlayMaterial.transform);
                
                // 이벤트 클리어 시 Fx 종료 이벤트 등록
                ParticleSystem eventFx = eventFxGameObject.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
                ParticleSystem.MainModule eventFxMainModule = eventFx.main;
                eventFxMainModule.stopAction = ParticleSystemStopAction.Callback;
                eventFx.GetComponent<HexEventFxHandler>().EventAction += () => Destroy(eventFxGameObject);

                // 이벤트 클리어 시 Fx 시작 이벤트 등록
                _hexEventFxParticleAction = null;
                _hexEventFxParticleAction += () =>
                {
                    eventFx.gameObject.SetActive(true);
                    eventFx.Play();
                    hexOverlayMaterial.material = Managers.Resource.LoadResource<Material>(ResourceManager.ResourcePath.HexOverlay, 
                        "HexOverlayDefault", 
                        false);
                };

                
                break;
        }
    }
    
    #endregion
    
  
 

    private void OnMouseEnter()
    {
        // 마우스 현재 커서가 UI 오브젝트에 있으면 이벤트를 처리하지 않음
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        _mouseEnterAction?.Invoke();
        
        /*
       if (UnitManager.Instacne.SelectedWorldMapPlayerCharacter) return;
        EnableHighlight();
        */
        
        Managers.WorldMapGenerator.SetHexCursor(this.transform.position);
    }

    private void OnMouseExit()
    {
        _mouseExitAction?.Invoke();
        
        /*
      if (UnitManager.Instacne.SelectedWorldMapPlayerCharacter) return;
        DisableHighlight();
        */
    }
}

public enum HexType
{
    Water,
    Default,
    Obstacle,
    BattleZone,
    QuestZone,
    Store,
    Cave,
    Sanctuary,
    Event,
}

public enum BattleZoneType
{
    None = -1,
    Solo = 0,
    Camp = 1,
    Group = 2,
}

public enum QuestZoneType
{
    None = -1,
    Battle = 0,
    Delivery = 1,
}
