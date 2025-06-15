using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OnPointerEventPopUp : UI_Scene
{
    
    private enum RootTransform
    {
        DifficultyBackGround = 0,
        SkillBackerBackGround,
        ActiveIcon,
        BrokenSanctumIcon,
        ClearedIcon,
        DestinationIcon,
        LockedIcon,
        StartIcon,
        WarningIcon,
        MultipleEnemyIcon,
        EnemyCampIcon
    }
    
    private enum Texts
    {
        EventTitleText = 0,
        InfoDescriptionText,
        StoryDisplayText,
        DifficultyValueText,
    }
    
    private enum Images
    {
        SkillSymbol = 0
    }

    // 표시할 Hex의 이벤트 정보를 캐싱
    private Hex _hexInfo;

    // 마지막으로 활성화된 아이콘 이미지
    private List<GameObject> _onIconList = new List<GameObject>(2);

    // 이벤트에 대한 각 타입에 대한 상세설명 // TODO : 성소에 대한 버프에 대한 내용을 분리해야함 (ex : 경험치 버프, 이동속도 버프 등)
    private Dictionary<HexType, string> _eventDetailTextDictionary = new Dictionary<HexType, string>()
    {
        //전투
        { HexType.BattleZone, "적과 전투를 진행합니다." },
        // 마을
        { HexType.Store, "안전한 지역이며 휴식과 상점을 이용할 수 있습니다." },
        // 동굴
        { HexType.Cave, "위험한 탐험을 진행합니다." },
        // 성소
        { HexType.Sanctuary, "성소에 숭배하여 버프를 받습니다." },
        // 퀘스트 
        { HexType.QuestZone, "도착하면 퀘스트를 진행합니다."}
    };


    public override void Init()
    {
        Bind<GameObject>(typeof(RootTransform));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        // 모든 아이콘 이미지 비활성화 (초기화 작업)
        for (int i = 0; i < Enum.GetValues(typeof(RootTransform)).Length; i++)
        {
            Get<GameObject>(i).SetActive(false);
        }
    }

    public void GridInit(Hex hexInfo)
    {
        _hexInfo = hexInfo;
        SetEventText(_hexInfo.GetEventInfoList()[0].GetEventName(), _eventDetailTextDictionary[_hexInfo.HexType]);
        InitEventDetailIcon();
      
        // 해당 오브젝트 활성화
        base.Init();
    }

    private void InitEventDetailIcon()
    {
        bool levelIcon = false;

        if (_hexInfo.HexType == HexType.Cave || _hexInfo.BattleZoneType != BattleZoneType.None)
        {
            levelIcon = true;

            if (_hexInfo.BattleZoneType == BattleZoneType.Camp)
            {
                EnableIcon(RootTransform.EnemyCampIcon);
            }
            else if (_hexInfo.BattleZoneType == BattleZoneType.Group)
            {
                EnableIcon(RootTransform.MultipleEnemyIcon);
            }
        }
        
        /*
        switch (_hexInfo.HexType)
        {
            case HexType.Cave: levelIcon = true;
                break;
            
            case HexType.BattleZone :
                levelIcon = true;
                if (_hexInfo.BattleZoneType == BattleZoneType.Camp) EnableIcon(RootTransform.EnemyCampIcon);
                else if(_hexInfo.BattleZoneType == BattleZoneType.Group) EnableIcon(RootTransform.MultipleEnemyIcon);
                break;
            
            /*
            case HexType.EventZone : skillIcon = true;
                Get<Image>((int)Images.SkillSymbol).sprite = "요구하는 스킬 이미지";
                break;
                
                
         }
        */

        // 전투지역이면 레벨텍스트 할당
        if (levelIcon)
        {
            EnableIcon(RootTransform.DifficultyBackGround);
            Get<TextMeshProUGUI>((int)Texts.DifficultyValueText).text =
                _hexInfo.GetEventInfoList()[0].GetEventLevel().ToString();
        }
        
        /*
        if (_hexInfo.isEventClaer)
        {
           EnableIcon(RootTransform.ClearedIcon);
        }
        */
        
     
        
        // 마을 => none
        // 성소 = None 아니면  체크 표시 
        // 광산 => 레벨 아니면 체크 표시
        // 전투지역 => 레벨
            // 전투무리 => 레벨 + 무리표시
            // 전투캠프 => 레벨 + 캠프표시
        // 이벤트지역 => 요구되는 스킬 아이콘 표시    
        

    }

    /// <summary>
    /// 이벤트에 대한 Text 설정
    /// </summary>
    /// <param name="eventNameText">이벤트 이름</param>
    /// <param name="eventDetailText">이벤트 상세 설명</param>
    /// <param name="eventStoryText">이벤트 추가(스토리) 설명</param>
    private void SetEventText(string eventNameText, string eventDetailText, string eventStoryText = null)
    {
        Get<TextMeshProUGUI>((int)Texts.EventTitleText).text = eventNameText;
        Get<TextMeshProUGUI>((int)Texts.InfoDescriptionText).text = eventDetailText;
        Get<TextMeshProUGUI>((int)Texts.StoryDisplayText).text = eventStoryText;
    }

    private void EnableIcon(RootTransform iconGameObject)
    {
        var icon = Get<GameObject>((int)iconGameObject);
        icon.SetActive(true);
        _onIconList.Add(icon);
    }

    public void ClosedOnPointerUI()
    {
        // 활성화된 아이콘 이미지 비활성화
        foreach (var icon in _onIconList)
        {
            icon.gameObject.SetActive(false);
        }
        _onIconList.Clear();
        base.ClosedUI();
    }
}
