using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Object = UnityEngine.Object;

 public class UIManager : MonoBehaviour
{

    
    # region 3D UI 연출
    
    [Header("3D 랜더텍스쳐 월드맵 오브젝트 촬영위치")] 
    [SerializeField] private Transform _uiEventObjectRendererTexturePos;
    
    [Header("3D 랜더텍스쳐 아이템 오브젝트 촬영위치")] 
    [SerializeField] private Transform _uiItemObjectRendererTexturePos;
    
    [Header("3D 랜더텍스쳐 플레이어 오브젝트 촬영위치")]
    [SerializeField] private Transform _uiPlayerInventoryRendererTexturePos;
    
    public Transform UIEventObjectRendererTexturePos => _uiEventObjectRendererTexturePos;

    public Transform UIItemObjectRendererTexturePos => _uiItemObjectRendererTexturePos;

    public Transform UIPlayerInventoryRendererTexturePos => _uiPlayerInventoryRendererTexturePos;
    
    # endregion

    #region UI 팝업창
    
    [Header("UI 카메라")]  
    [SerializeField] private Camera uiCamera;
    
    [Header("UI 이벤트 팝업창")] 
    [SerializeField] private UI_EventPopUp _eventPopUp;                           
    public UI_EventPopUp EventPopUp => _eventPopUp;
    
    [Header("UI 마우스 UP 이벤트 팝업창")] 
    [SerializeField] private UI_OnPointerEventPopUp _onPointerEventPopUp;          

    [Header("UI 배틀 팝업창")]
    [SerializeField] private UI_BattleTimeLineGrid _uI_BattleTimeLineGrid;
    public UI_BattleTimeLineGrid UIBattleGrid => _uI_BattleTimeLineGrid;

    [Header("UI 던전 팝업창")]
    [SerializeField] private UI_CaveTImeLineGird _uI_CaveTImeLineGird;
    public UI_CaveTImeLineGird UICave => _uI_CaveTImeLineGird;

    [Header("UI 메인 아이템 카드")]
    [SerializeField] private UI_MainItemCard _uiMainItemCard;
    
    [Header("UI 플레이가 장착한 아이템 카드")]
    [SerializeField] private UI_PlayerEquippedItemCard _uiEquippedItemCard;

    [Header("UI 플레이어 인벤토리창")] 
    [SerializeField] private UI_InventoryBackGround _uiInventoryBackGround;

    [Header("UI 아이템 상호작용 버튼 창")] 
    [SerializeField] private UI_ItemMenuButton _uiItemMenuButton;

    private Stack<UI_PopUp> _popUpStack = new Stack<UI_PopUp>();       // UI PopUp Stack
    
    private RectTransform _onPointerEventUiObject;                     // Mouse Enter UI창을 표시할 RectTransform 위치
    
    private Coroutine _onPointerPopUpCoroutine;                        // onPointerEventPopUp 코루틴
    
    private readonly WaitForSeconds _DELAY = new WaitForSeconds(0.5f);  // onPointerEventPopUp창 코루틴 딜레이
    
    public enum PopUpUI
    {
        None = -1,
        UI_ItemMenuButton,
    }
    
    #endregion

    #region UI Scene 창
    
    [Header("UI Scene 창")] 
    [SerializeField] private UI_BaseScene _uiBaseScene;
    
    #endregion

    /// <summary>
    /// Managers에서 호출
    /// </summary>
    public void UIInit()
    {            
        _uiBaseScene.Init();
        
        _eventPopUp.Init();
        
        _onPointerEventPopUp.Init();
        
        _onPointerEventUiObject = _onPointerEventPopUp.transform.GetChild(0).GetComponent<RectTransform>();
        
        _uiInventoryBackGround.Init();
        
        _onPointerEventPopUp.gameObject.SetActive(false);
        
        _uI_BattleTimeLineGrid.Init();

        if (_uI_CaveTImeLineGird != null)
        {
            _uI_CaveTImeLineGird.Init();    
        }

        // 메인 아이템 카드 초기화
        _uiMainItemCard.Init();
        
        // 플레이어 장착 아이템 카드 초기화
        _uiEquippedItemCard.Init();
        
        // 아이템 상호작용 UI 버튼 초기화
        _uiItemMenuButton.Init();
        
        // 일단 초기 UI는 무조건 월드맵 UI로 설정
        ChangeTimeLineGrid<UI_WorldMapTimeLineGrid>();
    }
    
    // UI 프리팹 동적생성 
    public T MakeUiPrefab<T>(Transform parent = null, string uiName = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(uiName))
        {
            uiName = typeof(T).Name;
        }
        
        GameObject uiObject = Instantiate(Resources.Load<GameObject>(uiName));
        
        if(parent) uiObject.transform.SetParent(parent, false);

        return uiObject.GetOrAddComponent<T>();
    }

    
    public void ShowPopUI(UI_PopUp popup)
    {
        if (popup != _uiItemMenuButton)
        {
            if (_popUpStack.Count != 0)
            {
                _popUpStack.Peek().gameObject.SetActive(false);
            }
        }
        
        _popUpStack.Push(popup);
        
        popup.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        popup.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 팝업 UI를 Close 하기전 예외처리 확인 (오버로딩)
    /// </summary>
    /// <param name="popup">지우고자하는 UI 팝업창</param>
    public void ClosePopUpUI(UI_PopUp popup)
    {
        if (_popUpStack.Count == 0)
            return;

        // 예외처리를 모두 통과하였으니 바로 팝업창 UI 삭제진행
        ClosePopUpUI();
    }

    /// <summary>
    /// 특정 팝업 UI를 찾아서 비활성화
    /// </summary>
    /// <param name="popUpUI">팝업 창 종류 (UI 매니저에서 알아서 팝업 창을 찾아줌)</param>
    /// <param name="popup">특정 팝업 UI</param>
    public void SearchForClosePopUpUI(PopUpUI popUpType, UI_PopUp popup = null)
    {
        UI_PopUp foundPopUp = popup;

        switch (popUpType)
        {
            case PopUpUI.UI_ItemMenuButton : foundPopUp = _uiItemMenuButton;
                break;
          
        }
        
        // 팝업 UI가 이미 비활성화 경우 예외처리
        if (!foundPopUp!.gameObject.activeSelf) return;

        Stack<UI_PopUp> tempStack = new Stack<UI_PopUp>();
        
        while (_popUpStack.Count > 0)
        {
            UI_PopUp targetUI = _popUpStack.Pop();
            
            if (targetUI == foundPopUp) break;
            tempStack.Push(targetUI);
        }

        while (tempStack.Count > 0)
        {
            _popUpStack.Push(tempStack.Pop());
        }

        foundPopUp.gameObject.SetActive(false);
    }

    /// <summary>
    /// 목표 팝업 UI 까지 모든 PopUpUI를 비활성화함
    /// </summary>
    /// <param name="targetPopUp">목표 팝업 UI</param>
    /// <param name="lastPopUpUiShow">목표 팝업 UI 전의 팝업 UI 활성화 유무</param>
    public void RemoveForTargetClosePopUpUI(UI_PopUp targetPopUp, bool lastPopUpUiShow = true)
    {
        while (_popUpStack.Count > 0)
        {
            UI_PopUp closePopUp = _popUpStack.Pop();
            closePopUp.gameObject.SetActive(false);
            if (closePopUp == targetPopUp)
            {
                break;
            }
        }
        
        if(_popUpStack.Count != 0)
        {
            _popUpStack.Peek().gameObject.SetActive(lastPopUpUiShow);
        }
    }
    

    public void ClosePopUpUI()
    {
        if (_popUpStack.Count == 0)
        {
            return;
        }

        UI_PopUp popup = _popUpStack.Pop();
        popup.gameObject.SetActive(false);

        if(_popUpStack.Count != 0)
        {
            _popUpStack.Peek().gameObject.SetActive(true);
        }
    }
    
    public void CloseAllPopUpUI()
    {
        while (_popUpStack.Count > 0)
        {
            ClosePopUpUI();
        }
    }



    #region Top Panel UI 관련

    /// <summary>
    /// 턴 종료 버튼 활성화 또는 비활성화
    /// </summary>
    /// <param name="isEnable">활성화 또는 비활성화</param>
    public void InteractionTurnOverButton(bool isEnable)
    {
       // print($"턴 종료 버튼 : {isEnable}");
        _uiBaseScene.InteractionTurnOverButton = isEnable;
    }

    /// <summary>
    /// 상체 타임라인 Grid 교체
    /// </summary>
    /// <typeparam name="T">UI_TimeLine을 상속받고 있는 전투, 광산, 월드 맵 Grid</typeparam>
    public void ChangeTimeLineGrid<T>() where T : UI_TimeLine
    {
        _uiBaseScene.ChangeTimeLineGrid<T>();
    }

    public void SetQuestChart(bool isOn)
    {
        _uiBaseScene.SetQuestChart(isOn);
    }
    public UI_QuestChart GetQuestChart()
    {
        return _uiBaseScene.GetQuestChart();

    }
    /// <summary>
    /// 플레이어 턴 Text 변경
    /// </summary>
    /// <param name="playerNickName">현재 플레이어 닉네임</param>
    public void ChangePlayerTurnText(string playerNickName)
    {
        _uiBaseScene.SetCurrentPlayerTurnText = playerNickName;
    }

    #endregion

    #region Bottom Panel UI 관련

    public UI_PlayerMainHUD GetPlayerHUD(PlayerStats owner)
    {
       return _uiBaseScene.GetPlayerHUD(owner);
    }

    /// <summary>
    /// 인벤토리 UI 창 활성화
    /// </summary>
    /// <param name="owner">요청한 플레이어</param>
    /// <param name="isShow">인벤토리 UI 비활성화(기본 값 : true)</param>
    /// <typeparam name="T">인벤토리 또는 스텟창 UI 재너릭 타입</typeparam>
    public void ShowPlayerInventory<T>(PlayerStats owner, bool isShow = true) where T : Object, I_Inventory
    {
        if(_uiInventoryBackGround.gameObject.activeSelf) _uiInventoryBackGround.gameObject.SetActive(false);
        
        Managers.Inventory.SetRequestPlayer(owner);
        
        PlayerInventoryUiUpdate<T>(owner, isShow);
    }

    /// <summary>
    /// 인벤토리 UI 업데이트
    /// </summary>
    /// <param name="owner">요청한 플레이어</param>
    /// <param name="isShow">UI 활성화 여부</param>
    /// <typeparam name="T">인벤토리 또는 스텟창 UI 재너릭 타입</typeparam>
    public void PlayerInventoryUiUpdate<T>(PlayerStats owner, bool isShow = false) where T : Object, I_Inventory
    {
        _uiInventoryBackGround.InventoryUiUpdate<T>(owner, isShow);
    }
    

    #endregion

    #region Pop Up UI 관련
    
    
    public void ShowEventPopUpUI(Hex hexInfo, List<IEventInfo> eventObjects, List<WorldMapPlayerCharacter> playerList, Action overlayEvent, Action completeEvent)
    {
        /* 이벤트 등록전 무조건 등록해체하고 진행 ! */
        if (hexInfo.HexType is HexType.BattleZone or HexType.Cave)
        {
            _eventPopUp.UI_MainEventPopUp.DisableBattleOverEventHandler -= overlayEvent;
            _eventPopUp.UI_MainEventPopUp.DisableBattleOverEventHandler += overlayEvent;
        }

        if (hexInfo.HexType is HexType.BattleZone or HexType.Cave or HexType.Sanctuary)
        {
            _eventPopUp.UI_MainEventPopUp.CompleteHexEventHandler -= completeEvent;
            _eventPopUp.UI_MainEventPopUp.CompleteHexEventHandler += completeEvent;
        } 
  
        _eventPopUp.GridInit(hexInfo, eventObjects, playerList);
    }

    public void ShowOnPointerEventPopUpUI(Hex hexInfo, Vector2 mousePosition)
    {
        if (_onPointerPopUpCoroutine == null)
        {
            _onPointerPopUpCoroutine = StartCoroutine(DelayOnPointerEventPopUpProcess(hexInfo, mousePosition));
        }
    }

    private IEnumerator DelayOnPointerEventPopUpProcess(Hex hexInfo, Vector2 mousePosition)
    {
        // 1초 딜레이 후 UI 활성화
        yield return _DELAY;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            (RectTransform)_onPointerEventPopUp.transform, 
            mousePosition,
            uiCamera,
            out Vector2 uiPosition
        );
        
        _onPointerEventUiObject.position = _onPointerEventPopUp.transform.TransformPoint(uiPosition);
        _onPointerEventPopUp.GridInit(hexInfo);
    }
    
    /// <summary>
    /// UI 마우스 UP 이벤트 팝업창 코루틴 중단
    /// </summary>
    public void StopCoroutineOnPointerPopUp()
    {
        if (_onPointerPopUpCoroutine != null)
        {
            StopCoroutine(_onPointerPopUpCoroutine);
            _onPointerPopUpCoroutine = null;
        }

        _onPointerEventPopUp.ClosedOnPointerUI();
    }
    
    public void ShowItemMenuButtonUI(PlayerStats requestPlayer, Item selectItem, UI_ItemMenuButton.ItemMenuType itemMenuType, Vector2 uiPosition)
    {
        _uiItemMenuButton.transform.position = new Vector3(uiPosition.x + 20f, uiPosition.y - 10f);
        _uiItemMenuButton.ActiveItemMenuUI(requestPlayer, selectItem, itemMenuType);
    }

    /// <summary>
    /// 상점 아이템, 드랍 아이템 카드를 활성화 또는 비활성화 경우
    /// </summary>
    /// <param name="item">표시하고자는 아이템</param>
    /// <param name="targetPosition">아이템 카드 위치 선정</param>
    /// <param name="isShow">표시여부</param>
    /// <param name="isReward">보상 아이템 확인유무( 아이템 회전 연출 )</param>
    public void ShowMainItemCardUI(Item item, UI_PlayerEquippedItemCard.EquippedTarget targetPosition, bool isShow, bool isReward = false)
    {
        _uiMainItemCard.gameObject.SetActive(isShow);
        
        if (isShow == false)
        {
            _uiEquippedItemCard.gameObject.SetActive(false);
            return;
        }
        
        // 착용 가능한 아이템인 경우 현재 착용중인 아이템과 비교 UI 출력
        if (item is Equipment equipment && isReward == false)
        {
            ShowEquippedItemCardUI(equipment, targetPosition);
        }
        
        _uiMainItemCard.SetItemInfo(item, isReward);
    }

    /// <summary>
    /// 플레어가 장착한 아이템 카드를 활성화 또는 비활성화 할 경우
    /// </summary>ㅣ
    /// <param name="item">장착한 아이템</param>
    /// <param name="targetPosition">아이템 카드 위치 선정</param>
    private void ShowEquippedItemCardUI(Equipment item, UI_PlayerEquippedItemCard.EquippedTarget targetPosition)  
    {
        /* 1. 인벤토리를 확인하는 경우에만 무조건 장착한 아이템 카드를 왼쪽에 띄운다.
         * 2. 상점에서 확인하는 경우에는 무조건 장착한 아이템 카드를 오른쪽에 띄운다.
         * 3. 그 외에는 무조건 장착한 아이템 카드를 왼쪽에 띄운다. (아이템 드랍 획득 시)
         */
        
        // 착용된 장비가 없으면 장착한 아이템 카드 UI 생략
        if (item is null || !Managers.Inventory.RequestPlayer.PlayerInventory.ContainsEquipmentItemCheck(item))
        {
            _uiEquippedItemCard.gameObject.SetActive(false);
            return;
        }

        // 현재 카드의 활성화 상태 확인
        bool toggle = _uiEquippedItemCard.gameObject.activeSelf;
        
        // 카드의 활성화 상태를 반전
        _uiEquippedItemCard.gameObject.SetActive(!toggle);

        // isShow가 토글 형태로 작동함 (false면 true가 차례임)
        if (!toggle)
        {
            // 현재 비교할 아이템의 파츠 타입과 현재 장착중인 아이템 정보를 가져옴
            Equipment playerEquipment = Managers.Inventory.RequestPlayer.PlayerInventory.FilterEquipmentSlotByItem(item.EquipmentSlottype);
            _uiEquippedItemCard.SetPlayerEquippedItemCardPosition(targetPosition);
            _uiEquippedItemCard.SetItemInfo(playerEquipment, false);
        }
    }

    #endregion
}

