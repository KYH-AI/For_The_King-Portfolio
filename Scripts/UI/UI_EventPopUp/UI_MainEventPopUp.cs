using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UI_MainEventPopUp : UI_PopUp
{
    public UI_EventPopUp ParentEventPopUpUI { get; private set; }

    // ���� �̺�Ʈ�� �۵��Ǵ� Hex
    private Hex _hexInfo;
    
    // �̺�Ʈ ������Ʈ ����Ʈ
    private List<IEventInfo> _eventList;
    
    // �̺�Ʈ ������Ʈ�� ��ȣ�ۿ��ϴ� �÷��̾� ����Ʈ
    private List<WorldMapPlayerCharacter> _playerList;
    
    // �������� ���� �̺�Ʈ ������Ʈ
    private GameObject _rendererObject = null;
    
    // �������� ���� ��ư �׸��� ������Ʈ
    private UI_EventPopUpButtonGrid _lastButtonGrid = null;
    
    
    private readonly string EVENT_UI_PREFAB_PATH = "Prefabs/UI/EventObject/";
    
    
    private readonly string MAIN_EVENT_PREFAB_PATH = "Character/WorldMapEnemyCharacter/";

    #region  Hex�� ���� �̺�Ʈ �ڵ鷯
    
    public event Action CompleteHexEventHandler;        // �ش� Hex �̺�Ʈ�� Ŭ���� �� (�ش� �̺�Ʈ�� �Ϸ��� ��� ȣ��)
    
    public event Action DisableBattleOverEventHandler; // ������ ��ư�� ������ Hex�� ���� Overlay ȿ�� ����
    
    #endregion

    private enum Panels
    {
        PlayerPortraitRoot,      // ���� �̺�Ʈ�� �̿��ϴ� ���� ���� ��ġ
        EnemyPortraitRoot,       // ���� �̺�Ʈ�� ����ؾ��ϴ� ���� ���� ��ġ
        SlotPanel,               // �̺�Ʈ�� �ʿ��ϴ� �ɷ�ġ ������ ��ġ
        ButtonPanel,             // �̺�Ʈ ��ȣ�ۿ��ϴ� ��ư ��ġ
        EventDetailIconRoot,     // �̺�Ʈ Ư������ ������ ��ġ
        EventLevelBackGround,
        EnemySwarmIcon,
        EnemyCampIcon,
        EventNoFocusIcon,
    }

    private enum Texts
    {
        EventNameText, // �̺�Ʈ ����
        EventDetailText, // �̺�Ʈ �� ����
        EventDescriptionText, // �̺�Ʈ ���丮 
        EventLevelText // �̺�Ʈ ���� 
    }

    // �÷��̾�� �̺�Ʈ ������Ʈ �ʻ�ȭ ������Ʈ
    private enum ObjectPortraits
    {
        UI_PlayerObjectPortrait_1,
        UI_PlayerObjectPortrait_2,
        UI_PlayerObjectPortrait_3,
        UI_EventObjectPortrait_1,
        UI_EventObjectPortrait_2,
        UI_EventObjectPortrait_3,
    }
    
    private enum BattleButtonGird
    {
        UI_BattleButtonGrid
    }

    private enum StoreButtonGird
    {
        UI_StoreButtonGrid
    }
    
    private enum CaveButtonGird
    {
        UI_CaveButtonGrid
    }
    
    private enum SanctuaryButtonGird
    {
        UI_SanctuaryButtonGrid
    }

    public void Init(UI_EventPopUp parentUIEventPopUp)
    {
        ParentEventPopUpUI = parentUIEventPopUp;
        Init();
    }
   
    public override void Init()
    {
        // �г� ����
        Bind<GameObject>(typeof(Panels));
        // �ؽ�Ʈ ����
        Bind<TextMeshProUGUI>(typeof(Texts));
        // �ʻ�ȭ ����
        Bind<UI_EventUnitSlot>(typeof(ObjectPortraits));

        Bind<UI_BattleButtonGrid>(typeof(BattleButtonGird));
        
        Bind<UI_StoreButtonGrid>(typeof(StoreButtonGird));
        
        Bind<UI_CaveButtonGrid>(typeof(CaveButtonGird));
        
        Bind<UI_SanctuaryButtonGrid>(typeof(SanctuaryButtonGird));
        

        for (int i = 0; i < Enum.GetValues(typeof(ObjectPortraits)).Length; i++)
        {
            Get<UI_EventUnitSlot>(i).Init();
        }
        
        Get<UI_BattleButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);

        Get<UI_StoreButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);
            
        Get<UI_CaveButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);
        
        Get<UI_SanctuaryButtonGrid>(0).ButtonGridInit(ParentEventPopUpUI);
        
    }

    
    /// <summary>
    /// ����Ǵ� UI ��ҿ� �˸°� ���� ( 1ȸ ȣ���� �ƴ� �Ź� ����� )
    /// </summary>
    /// <param name="hexInfo">Ȯ�ε� ù ��° Hex ����</param>
    /// <param name="eventObjects">������ �̺�Ʈ ������Ʈ ������</param>
    /// <param name="playerList">������ �÷��̾� ����Ʈ</param>
    public void GridInit(Hex hexInfo, List<IEventInfo> eventObjectList, List<WorldMapPlayerCharacter> playerList)
    {
        _hexInfo = hexInfo;
        _eventList = eventObjectList;
        _playerList = playerList;
        
        InitEventTexts(_eventList[0].GetEventName(), _eventList[0].GetEventDetailText(), _eventList[0].GetEventDescriptionText());
        InitUnitSlot();
        InitSlot();
        InitDetailSlot();
        InitButtons();
        SetEventMainObject(_eventList[0].GetEventID());
        
        // ���� EventPopUp ���ӿ�����Ʈ Ȱ��ȭ
        base.Init();
        
        
        /* <�̺�Ʈ UI �ʱ�ȭ ����>
         *  1. �̺�Ʈ �̸�
         *  2. �̺�Ʈ ��� �ʻ�ȭ ����
         *  3. �̺�Ʈ �䱸�Ǵ� �ɷ�ġ ����
         *  4. �̺�Ʈ ��ư
         *  5. �̺�Ʈ 3D ������Ʈ Renderer Texture
         *  6. ���� ������Ʈ Ȱ��ȭ
         */
    }
    

    private void InitEventTexts(string eventNameText, string eventDetailText, string eventStoryText)
    {
        Get<TextMeshProUGUI>((int)Texts.EventNameText).text = eventNameText;
        Get<TextMeshProUGUI>((int)Texts.EventDetailText).text = eventDetailText;
        Get<TextMeshProUGUI>((int)Texts.EventDescriptionText).text = eventStoryText;
    }

    private void InitUnitSlot()
    {
        int count = Enum.GetValues(typeof(ObjectPortraits)).Length;

        // �ʻ�ȭ ���� ��� �ʱ�ȭ
        for (int i = 0; i < count; i++)
        {
            Get<UI_EventUnitSlot>(i).gameObject.SetActive(false);
        }

        // �÷��̾� �ʻ�ȭ �Ҵ�
        for (int playerIndex = 0; playerIndex < _playerList.Count; playerIndex++)
        {
            Get<UI_EventUnitSlot>(playerIndex).SetEventObjectInformation(false, _playerList[playerIndex].PlayerStats.PlayerPortrait, _playerList[playerIndex].PlayerStats.BaseLevel);
            Get<UI_EventUnitSlot>(playerIndex).gameObject.SetActive(true);
        }

        // ������ ������ �̺�Ʈ�� ���� ���Կ� �Ҵ����� ���� (�ش� �̺�Ʈ�� ������ ���ٴ� ��)
        if (_hexInfo.BattleZoneType == BattleZoneType.None) return;
        
        // TODO : ���� �ɹ��� ��� '?'�� ǥ��

        bool isHidden = _hexInfo.BattleZoneType == BattleZoneType.Camp;

        int maxEnemyCount = Mathf.Min(3, _eventList.Count);
        // ���� ���� �Ҵ�
        for (int enemyIndex = 3; enemyIndex < maxEnemyCount + 3; enemyIndex++)
        {
            var portrait = !isHidden ? _eventList[enemyIndex - 3].GetEventPortrait() : null; 
            Get<UI_EventUnitSlot>(enemyIndex).SetEventObjectInformation(isHidden, portrait, _eventList[enemyIndex - 3].GetEventLevel());
            Get<UI_EventUnitSlot>(enemyIndex).gameObject.SetActive(true);
        }
        
    }

    private void InitSlot() 
    {

    }

    private void InitDetailSlot()
    {
        bool isNeedDetail = true;
        
        if (_hexInfo.HexType is HexType.Store or HexType.Sanctuary)
        {
            isNeedDetail = false; 
        }

        if (isNeedDetail)
        {
            Get<TextMeshProUGUI>((int)Texts.EventLevelText).text = _eventList[0].GetEventLevel().ToString();
        }
        else
        {
            foreach (Transform childDetailIcon in Get<GameObject>((int)Panels.EventDetailIconRoot).transform)
            {
                childDetailIcon.gameObject.SetActive(false);
            }
        }
        
  
        /*  ����, ���� �̺�Ʈ�� ���� ǥ��
         *  ���� �߻� �̺�Ʈ�� ���߷� �̻�� ǥ��
         *  ����, ���� �̺�Ʈ�� �ƹ��͵� ǥ������ ����
         *  ����ķ���� ���������� ķ�� �Ǵ� ���� ������ ǥ��
         */ 
        
        
    }
    private void InitButtons()
    {
        switch (_hexInfo.HexType)
        {
            case HexType.BattleZone : InitButtons<UI_BattleButtonGrid>(); break;
            case HexType.Store : InitButtons<UI_StoreButtonGrid>(); break;
            case HexType.Cave : InitButtons<UI_CaveButtonGrid>(); break;
            case HexType.Sanctuary : InitButtons<UI_SanctuaryButtonGrid>(); break;
            case HexType.QuestZone:
                if (_hexInfo.QuestZoneType is QuestZoneType.Battle) InitButtons<UI_BattleButtonGrid>();
                break;
            default: Debug.LogError($"{_hexInfo.HexType.ToString()}�� �������� �ʴ� HexType �Դϴ�!"); break;;
        }

    }
    
    private void InitButtons<T>() where T : UI_EventPopUpButtonGrid
    {
        // �������� ���� ��ư Grid�� ������ ��Ȱ��ȭ
        if (_lastButtonGrid)
        {
            _lastButtonGrid.gameObject.SetActive(false);
        }

        _lastButtonGrid = Get<T>(0);
        _lastButtonGrid.SetRequesterWorldMapPlayCharacter(_playerList[0]);
        _lastButtonGrid.gameObject.SetActive(true);
        
    }

    // �̺�Ʈ ������Ʈ RendererTexture ����
    private void SetEventMainObject(int mainObjectID)
    {
        
        // ������ �ִ� �̺�Ʈ ������Ʈ�� ����
        ClearEventMainObject();
        

        string path = string.Empty;
        ResourceManager.ResourcePath resourcePath = 0;

        switch (_hexInfo.HexType)
        {
            case HexType.BattleZone :
                path = mainObjectID + "_World" + (DataManager.EnemyCharacter)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.WorldMapEnemy;
                break;
            
            case HexType.Store :
                path = mainObjectID + "_" + (DataManager.StoreID)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.Store;
                break;
            
            case HexType.Sanctuary :
                path = mainObjectID + "_" + (DataManager.SanctuaryID)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.Sanctuary;
                break;
            
            case HexType.Cave :
                path = mainObjectID + "_" + (DataManager.CaveID)mainObjectID;
                resourcePath = ResourceManager.ResourcePath.Cave;
                break;
            
            case HexType.QuestZone:
                if (_hexInfo.QuestZoneType is QuestZoneType.Battle)
                {
                    path = mainObjectID + "_World" + (DataManager.EnemyCharacter)mainObjectID;
                    resourcePath = ResourceManager.ResourcePath.WorldMapEnemy;
                }
                break;

            default: Debug.LogError("������ �� ���� Renderer Texture ������Ʈ �Դϴ�!!");
                return;
   
        }

        GameObject newEventObject = Instantiate(Managers.Resource.LoadResource<GameObject>(resourcePath, path, false), 
            Managers.UIManager.UIEventObjectRendererTexturePos);

        newEventObject.GetComponentInChildren<Camera>().enabled = true;

        // Ȥ�� ���� ������Ʈ�� ��� "���� �ִϸ��̼�" ����
        if (newEventObject.TryGetComponent(out WorldMapEnemyCharacter introTrigger))
        {
            introTrigger.PlayAnimationTrigger(CharacterEventListener.AnimationTrigger.Taunt);
        }

           // ���ο� �̺�Ʈ ������Ʈ�� ����
        _rendererObject = newEventObject;
    }

    private void ClearEventMainObject()
    {
        if (!_rendererObject) return;
        Destroy(_rendererObject);
    }

    public void CompleteHexEvent()
    {
        CompleteHexEventHandler?.Invoke();
    }
    
    public void DisableHexBattleOver()
    {
        DisableBattleOverEventHandler?.Invoke();
    }

    /// <summary>
    /// �÷��̾� ������Ʈ �����͸� ����Ʈ�� ����
    /// </summary>
    /// <returns>�÷��̾� ������Ʈ ����Ʈ</returns>
    public List<PlayerStats> GetInteractionPlayerList()
    {
        List<PlayerStats> playerObjectList = new List<PlayerStats>();

        foreach (var playerList in _playerList)
        {
            playerObjectList.Add(playerList.PlayerStats);
        }
        return playerObjectList;
    }

    /// <summary>
    /// �̺�Ʈ ������Ʈ �����͸� ����Ʈ�� ����
    /// </summary>
    /// <returns>�̺�Ʈ ������Ʈ ����Ʈ</returns>
    public List<int> GetInteractionEventList()
    {
        List<int> eventObjectList = new List<int>();

        foreach (var eventList in _eventList)
        {
           eventObjectList.Add(eventList.GetEventID());
        }
        return eventObjectList;
    }

    /// <summary>
    /// �ش� Hex ������ ����Ʈ�� �ִ��� Ȯ��
    /// </summary>
    /// <returns></returns>
    public HexGrid.QuestHexInfo GetHexQuestID()
    {
        return UnitManager.Instacne.HexGrid.GetQuestHexID(_hexInfo);
    }


    public override void ClosedPopUpUI()
    {
        base.ClosedPopUpUI();
        
        // ������ ������ ���� ���ڸ��� ������ ��
        if (_hexInfo.BattleZoneType == BattleZoneType.None)
        {
            // ���� �̺�Ʈ�� ����� �÷��̾��� ���¸� ��� ���·� ����
            Managers.Turn.PlayerTurn.IsPlayerInEvent = false;
            // ���� �̺�Ʈ�� ����� �÷��̾�� �ٽ� ����� �̵������ �ο�
            UnitManager.Instacne.HandleUnitSelected(Managers.Turn.PlayerTurn.gameObject);
            return;
        }
        
        // ���������� ��� �������� ��� ����
        DisableHexBattleOver();
    }
}


