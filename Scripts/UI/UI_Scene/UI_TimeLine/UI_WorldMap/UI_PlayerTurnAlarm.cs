using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PlayerTurnAlarm : UI_Base
{
    #region 상수 변수
    private readonly string _TURN = "의 턴";
    private readonly WaitForSeconds _SPEED_ICON_SEC_DELAY = new WaitForSeconds(0.4f);
    private readonly WaitForSeconds _TURN_ALARM_SEC_DELAY = new WaitForSeconds(2f);
    private Sprite _slotFailIcon;       // 슬롯 실패 아이콘 이미지
    private Sprite _slotQuicknessIcon;  // 슬롯 성공 아이콘 이미지
    private Sprite _slotBaseQuicknessIcon; // 슬로 보정 아이콘 이미지(황금색)
    private Sprite _slotBlankIcon;      // 빈 슬로 아이콘 이미지

    #endregion
    
    // 플레이어 초상화, 슬롯 결과 이미지
    private enum Images
    {
        PlayerPortrait,
        Result0,
        Result1,
        Result2,
        Result3,
        Result4,
        Result5,
        Result6,
        Result7,
        Result8
    }
    
    // 슬롯 게임오브젝트
    private enum SlotGameObjects
    {
        Slot0,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5,
        Slot6,
        Slot7,
        Slot8,
    }
    
    
    // 플레이어 닉네임 및 "이동 가능한 턴 표시"텍스트
    private enum Texts
    {
        PlayerNickNameText,
        PlayerTurnResultText
    }

    
    /// <summary>
    /// UI_WorldMapTimeLineGrid 에서 초기화 진행 (1회 호출)
    /// </summary>
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(SlotGameObjects));
        
        _slotFailIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld, "uiHitMiss", false);
        _slotBlankIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld, "uiSlotBlank", false);
        _slotQuicknessIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld, "uiSlotQuickness", false);
        _slotBaseQuicknessIcon = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld, "uiSlotBaseQuickness", false);
    }

    /// <summary>
    /// 현재 턴을 가진 플레이어 정보를 기반으로 턴 알람 UI를 설정함 (주기적으로 호출)
    /// </summary>
    /// <param name="playerImage">플레이어 초상화</param>
    /// <param name="playerNickName">플레이어 닉네임</param>
    /// <param name="movementPoints">획득한 플레이어 이동력</param>
    /// <param name="movementMaxPoints">플레이어 최대 이동력</param>
    public void SetPlayerInfo(Sprite playerImage, string playerNickName, int movementPoints, int movementMaxPoints, string[] resultMovements)
    {
        // Speed Icon 초기화 작업 진행
        ReSetTurnIcon();
        

        // 플레이어 최대 이동력 만큼 Speed 슬롯 아이콘 활성화 
        for (var i = 0; i < movementMaxPoints; i++)
        {
            Get<GameObject>(i).SetActive(true);
        }
        // 플레이어 초상화 설정
        Get<Image>((int)Images.PlayerPortrait).sprite = playerImage;
        // 플레이어 닉네임 설정
        Get<TextMeshProUGUI>((int)Texts.PlayerNickNameText).text = playerNickName + _TURN;

        // 플레이어가 획득한 턴을 Speed Icon으로 연출함
        //StartCoroutine(EffectTurnIcon(movementPoints, movementMaxPoints));
        StartCoroutine(SetPlayerMovementUI(resultMovements));
    }

   /// <summary>
   /// Speed 슬롯 아이콘 모두 빈 이미지로 교체 후 비활성화
   /// </summary>
    private void ReSetTurnIcon()
    {
        for (var i = 0; i < 9; i++)
        {
            Get<Image>(i).sprite = _slotBlankIcon;
            Get<GameObject>(i).SetActive(false);
        }
        
        // 플레이어 획득한 이동력 텍스트 설정
        Get<TextMeshProUGUI>((int)Texts.PlayerTurnResultText).text = 0.ToString();
    }

    /// <summary>
    /// 획득한 이동력만큼 Speed Icon을 활성화 함 (연출용)
    /// </summary>
    /// <param name="movementPoints">플레이어가 현재 획득한 이동력</param>
    /// <param name="movementMaxPoints">플레이어의 최대 이동력</param>
    private IEnumerator EffectTurnIcon( int movementPoints, int movementMaxPoints)
    {
        // Speed Icon 배열에 접근하기 위한 맵핑용 인덱스
        var i = 0;
        
        // 플레이어가 획득한 이동량 만큼 Speed Icon을 활성화 (성공 했다는 의미)
        for (i = 1; i <= movementPoints; i++)
        {
            // 지정한 만큼 딜레이 후 Speed Icon 색상을 변경함
            yield return _SPEED_ICON_SEC_DELAY;
            Get<Image>(i).sprite = _slotQuicknessIcon;
        }

        // 플레이어가 획득에 실패한 이동량 만큼 Speed Icon을 비활성화 (실패 했다는 의미)
        for (var j = i; j <= movementMaxPoints; j++) // j는 i에서 성공한 Speed Icon 다음 위치를 가르킴
        {
            yield return _SPEED_ICON_SEC_DELAY;
            Get<Image>(j).sprite = _slotFailIcon;
        }

        yield return _TURN_ALARM_SEC_DELAY;
        
        this.gameObject.SetActive(false);

    }

    private IEnumerator SetPlayerMovementUI(string[] resultMovement)
    {
        Sprite slotIcon = _slotBlankIcon;
        int hitCount = 0;
        for (int i = 0; i < resultMovement.Length; i++)
        {
            switch (resultMovement[i])
            {
                case "base" :
                    slotIcon = _slotBaseQuicknessIcon;
                    hitCount++;
                    break;
                case "hit" : 
                    slotIcon = _slotQuicknessIcon;
                    hitCount++;
                    break;
                case "miss" :
                    slotIcon = _slotFailIcon;
                    break;
                case "badWeather" : break;
                default: 
                    Debug.LogError("존재하지 않는 이동슬롯 아이콘입니다!");
                    break;
            }
            Get<TextMeshProUGUI>((int)Texts.PlayerTurnResultText).text = hitCount.ToString();
            Get<Image>(i+1).sprite = slotIcon;
            yield return _SPEED_ICON_SEC_DELAY;
        }
        
        yield return _TURN_ALARM_SEC_DELAY;
        this.gameObject.SetActive(false);
    }
}
