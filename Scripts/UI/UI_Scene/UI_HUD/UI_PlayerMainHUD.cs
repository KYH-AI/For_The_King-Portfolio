using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerMainHUD : UI_Scene
{
    
    private PlayerStats _owner;

    // 플레이어 HUD 타이틀 색상
    public Color PlayerTitleColor
    {
        get { return Get<Image>((int)Images.PlayerTitleBoard).color; }
    }


    #region 투표버튼
   
   private enum VoteBtn
   {
        UI_RewardButton,
   }
   
   private enum Token
   {
        UI_PlayerTokens,
   }
   private enum Floating
   {
        UI_FloatingText,
   }

    #endregion

    #region 플레이어 HUD 버튼

    private enum HUDButtons
    {
        InventoryButton,
        StatsButton
    }
    

    #endregion
    

    /* < 객체 타입 >
     *  1. 집중력
     *  2. 상태이상
     *  3. 성소
     *  4. 퀵 슬롯 인벤토리
     *  5. 인벤토리 아이콘
     *  6. 상세 스텟 아이콘
     *  7. 재화 아이콘
     *  8. 플레이어 초상화
     *  9. 플레이어 HUD 타이틀 턴 강조 이미지
     *  10.플레이어 타이틀 배경 색상 (붉은 색, 초록색, 파랑색)
     */


    private enum SliderBar
   {
       HpSliderBar,
       XpSliderBar
   }

   private enum Images
   {
       FocusSlot_1 = 0,
       FocusSlot_2,
       FocusSlot_3,
       FocusSlot_4,
       FocusSlot_5,
       FocusSlot_6,
       FocusSlot_7,
       FocusSlot_8,
       FocusSlot_9,
       FocusSlot_10 = 9,
       PlayerPortraitImage,
       HudTurnGlow,
       PlayerTitleBoard

   }

   /*  < Text 요소 >
    *  1. 물리 방어력
    *  2. 마법 방어력
    *  3. 회피력
    *  4. 공격력 (마법, 물리)
    *  5. 레벨
    *  6. 현재 체력
    *  7. 현재 체력 / 최대 체력
    *  8. 힘, 활력, 지능, 인지, 속도
    *  9. 닉네임
    *  10. 현재 골드
    *  11. 현재 경험치 / 최대 경험치
    *  12. 담배 레벨
    *  13. 현재 진행중인 사항 
    */
   private enum Texts
   {
       PlayerNameText,
       StrengthText,
       VitalityText,
       IntelligenceText,
       AwarenessText,
       QuicknessText,
       AttackDamageText,
       PipeText,
       CurrentHealthText,         // 체력 슬라이더랑 같이 연출됨
       CurrentSliderHealthText,  // 현재체력 / 최대체력
       CurrentSliderXpText,      // 현재경험치 / 최대경험치
       PlayerLevelDisplay,
       PhysicalDefenseDisplay,
       MagicDefenseDisplay,
       EvadeDefenseDisplay,
       CurrentGoldText,
       CurrentProcessText,     // 현재 진행중인 사항 (ex : 상점 이용 중)
   }

   // 플레이어 Hud에 텍스트 초기화 (Slider와 관련된 텍스트 처리는 X)
   public enum TextType
   {
       Stat,
       Gold,
       Level,
       Defense,
       Process,
       AttackDamage,
       PlayerNickName,
   }

   #region 텍스트 색상

   public static readonly Color PHYSICAL_DAMAGE_TYPE_COLOR = new Color(238 / 255f, 231 / 255f, 222 / 255f);
   
   public static readonly Color MAGICAL_DAMAGE_TYPE_COLOR = new Color(140 / 255f, 103 / 255f, 189 / 255f);

   public static readonly Color PHYSICAL_ARMOR_TYPE_COLOR = new Color(102 / 255f, 166 / 255f, 199 / 255f);

   public static readonly Color RESISTANCE_ARMOR_TYPE_COLOR = new Color(142 / 255f, 83 / 255f, 174 / 255f);

   public static readonly Color EVADE_TYPE_COLOR = new Color(83 / 255f, 174 / 255f, 121 / 255f);

   public static readonly Color LEVEL_TYPE_COLOR = new Color(142 / 255f, 169 / 255f, 221 / 255f);

   public static readonly Color HP_TYPE_COLOR = new Color(211 / 255f, 88 / 255f, 83 / 255f);

   public static readonly Color GOLD_TYPE_COLOR = new Color(249 / 255f, 219 / 255f, 154 / 255f);

   #endregion

   private int _activeFocusSlotIndex = (int)Images.FocusSlot_10;
   private int _pointFocusSlotIndex;
   
   public override void Init()
   {
       Bind<Image>(typeof(Images));
       Bind<TextMeshProUGUI>(typeof(Texts));
       Bind<Button>(typeof(HUDButtons));
       Bind<UI_Slider>(typeof(SliderBar));
       Bind<UI_RewardButton>(typeof(VoteBtn));
       Bind<UI_playerToken>(typeof(Token));
       Bind<UI_FloatingText>(typeof(Floating));
       
       // HUD 인벤토리, 스텟창 버튼 이벤트 등록
       Get<Button>((int)HUDButtons.InventoryButton).onClick.AddListener(() => Managers.UIManager.ShowPlayerInventory<UI_Inventory>(_owner));
       Get<Button>((int)HUDButtons.StatsButton).onClick.AddListener(() => Managers.UIManager.ShowPlayerInventory<UI_Stats>(_owner));
       
       Get<UI_Slider>((int)SliderBar.HpSliderBar).Init();
       Get<UI_Slider>((int)SliderBar.XpSliderBar).Init();
       Get<UI_RewardButton>((int)VoteBtn.UI_RewardButton).Init();
       Get<UI_playerToken>((int)Token.UI_PlayerTokens).Init();
       Get<Image>((int)Images.HudTurnGlow).enabled = false;
       
   }

   public void PlayerHudInit(PlayerStats owner, Color hudTitleColor)
   {
       Init();
       this._owner = owner;
       Get<UI_RewardButton>((int)VoteBtn.UI_RewardButton).SetOwner(_owner);
       var textType = Enum.GetValues(typeof(TextType));

       // 텍스트 타입 열거형 만큼 알아서 Hud Text 초기화
       for (int i = 0; i < textType.Length; i++)
       {
           UpdatePlayerMainHudTexts((TextType)textType.GetValue(i));
       }

       Get<Image>((int)Images.PlayerPortraitImage).sprite = _owner.PlayerPortrait;
       // 플레이어 HUD 타이틀 색상 변경
       Get<Image>((int)Images.PlayerTitleBoard).color = hudTitleColor;

       UpdateHealthDisplay(_owner.CurrentHp, _owner.MaxHeath);
       UpdateXpDisplay(_owner.GetXpPercent());

       _pointFocusSlotIndex = _owner.CurrentFocus;
       
       UpdateFocusDisplay(true);
       // UpdateFocusMaxDisplay(_owner.MaxFocus);
   }

   public void UpdatePlayerMainHudTexts(TextType type)
   {
       PlayerStats.PlayerMainHudDisplay playerMainHudDisplay;
       
       switch (type)
       {
           case TextType.Level :
                Get<TextMeshProUGUI>((int)Texts.PlayerLevelDisplay).text = _owner.BaseLevel.ToString();
                break;
           
           case TextType.Gold :
               Get<TextMeshProUGUI>((int)Texts.CurrentGoldText).text = _owner.CurrentGold.ToString();
               break;
           
           case TextType.AttackDamage :
               playerMainHudDisplay = _owner.GetDamageDisplay();
               Get<TextMeshProUGUI>((int)Texts.AttackDamageText).text = playerMainHudDisplay.Text;
               Get<TextMeshProUGUI>((int)Texts.AttackDamageText).color = playerMainHudDisplay.Color;
               
               break;
           
           case TextType.Defense:
               Get<TextMeshProUGUI>((int)Texts.PhysicalDefenseDisplay).text = _owner.GetDefenseDisplay(PlayerStats.DefenseType.PhysicalArmor);
               Get<TextMeshProUGUI>((int)Texts.MagicDefenseDisplay).text = _owner.GetDefenseDisplay(PlayerStats.DefenseType.Resistance);
               Get<TextMeshProUGUI>((int)Texts.EvadeDefenseDisplay).text = _owner.GetDefenseDisplay(PlayerStats.DefenseType.Avoid);
               break;
           
           case TextType.PlayerNickName :
               Get<TextMeshProUGUI>((int)Texts.PlayerNameText).text = _owner.BaseClassName;
               break;
           
           case TextType.Process :
              // Get<TextMeshProUGUI>((int)Texts.CurrentProcessText).text = // "상점 이용 중"
               break;
           
           case TextType.Stat :
               playerMainHudDisplay = _owner.GetStatDisplay(PlayerStats.StatType.Strength);
               Get<TextMeshProUGUI>((int)Texts.StrengthText).text = playerMainHudDisplay.Text;
               Get<TextMeshProUGUI>((int)Texts.StrengthText).color = playerMainHudDisplay.Color;
               
               playerMainHudDisplay = _owner.GetStatDisplay(PlayerStats.StatType.Vitality);
               Get<TextMeshProUGUI>((int)Texts.VitalityText).text = playerMainHudDisplay.Text;
               Get<TextMeshProUGUI>((int)Texts.VitalityText).color = playerMainHudDisplay.Color;
               
               playerMainHudDisplay = _owner.GetStatDisplay(PlayerStats.StatType.Intelligence);
               Get<TextMeshProUGUI>((int)Texts.IntelligenceText).text = playerMainHudDisplay.Text;
               Get<TextMeshProUGUI>((int)Texts.IntelligenceText).color = playerMainHudDisplay.Color;
               
               playerMainHudDisplay = _owner.GetStatDisplay(PlayerStats.StatType.Awareness);
               Get<TextMeshProUGUI>((int)Texts.AwarenessText).text = playerMainHudDisplay.Text;
               Get<TextMeshProUGUI>((int)Texts.AwarenessText).color = playerMainHudDisplay.Color;
               
               playerMainHudDisplay = _owner.GetStatDisplay(PlayerStats.StatType.Quickness);
               Get<TextMeshProUGUI>((int)Texts.QuicknessText).text = playerMainHudDisplay.Text;
               Get<TextMeshProUGUI>((int)Texts.QuicknessText).color = playerMainHudDisplay.Color;
               break;
           
           default: 
               Debug.LogError($"플레이어 HUD에 업데이트 할 수 없는 {type}타입 입니다!"); 
               break;

       }
   }

   public void UpdateSanctuaryIcon(Sanctuary.SanctuaryType sanctuaryType)
   {
       // TODO : 성소 타입에 맞는 이미지 Player HUD 적용
   }

   /// <summary>
   /// 턴 활성화시 HUD 강조 이미지 활성화
   /// </summary>
   /// <param name="isActive"></param>
   public void UpdatePlayerHudTitleGrow(bool isActive)
   {
       Get<Image>((int)Images.HudTurnGlow).enabled = isActive;
   }

   /// <summary>
   /// 체력 슬라이드 업데이트
   /// </summary>
   /// <param name="currentValue"></param>
   /// <param name="maxValue"></param>
   public void UpdateHealthDisplay(int currentValue, int maxValue)
   {
       float value = (float)currentValue / (float)maxValue;
       Get<UI_Slider>((int)SliderBar.HpSliderBar).SetSliderValue
           (value, 
           Get<TextMeshProUGUI>((int)Texts.CurrentHealthText), 
           currentValue, 
            maxValue);
       Get<TextMeshProUGUI>((int)Texts.CurrentSliderHealthText).text = _owner.GetHealthDisplayString();
   }

   /// <summary>
   /// 경험치 슬라이드 업데이트
   /// </summary>
   public void UpdateXpDisplay(float currentValue)
   {
       Get<UI_Slider>((int)SliderBar.XpSliderBar).SetSliderValue(currentValue);
       Get<TextMeshProUGUI>((int)Texts.CurrentSliderXpText).text = _owner.GetXpDisplayString();
   }

    /// <summary>
    /// 현재 집중력 UI 업데이트
    /// </summary>
    /// <param name="onlyMaxFocusUpdate">최대 집중력 UI 업데이트만 여부</param>
    /// <param name="currentFocus">현재 집중력 UI 업데이트 값 (-1 시 진행 안함)</param>
   public void UpdateFocusDisplay(bool onlyMaxFocusUpdate, int currentFocus = -1) 
    {
        int playerMaxFocus = _owner.MaxFocus;
                        // 집중력 최대 개수 - 배열 인덱스 (0 Base Start)
       int totalFocus = playerMaxFocus - _activeFocusSlotIndex;

       if (totalFocus > 0)
       {
           for (; _activeFocusSlotIndex < playerMaxFocus; _activeFocusSlotIndex++)
           {
               Get<Image>(_activeFocusSlotIndex).enabled = true;
           }

       }
       else if (totalFocus < 0)
       {
  
           for (; _activeFocusSlotIndex >= playerMaxFocus; _activeFocusSlotIndex--)
           {
               var focusSlot = Get<Image>(_activeFocusSlotIndex);
               focusSlot.sprite = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld,
                   "uiFocusSlotBlank");

               focusSlot.enabled = false;
           }
       }

       if (onlyMaxFocusUpdate) return;
       
       
        // 현재 집중력이 이전 집중력보다 크거나 같은지 여부를 확인하는 변수
       bool isAdd = currentFocus >= _pointFocusSlotIndex; 
       
        // 현재 집중력과 이전 집중력의 차이값을 계산
       int diff = Mathf.Abs(currentFocus - _pointFocusSlotIndex); 

       // 집중력이 증가하는 경우
       if (isAdd) 
       {
           // 이전 인덱스에서 diff값만큼 증가한 newIndex 값을 계산하고 범위를 넘지 않도록 제한
           int newIndex = Mathf.Min(_pointFocusSlotIndex + diff, (int)Images.FocusSlot_10);

           // 이전 인덱스 다음부터 newIndex까지의 범위에서 이미지를 업데이트
           for (int i = _pointFocusSlotIndex; i < newIndex; i++)
           {
               Get<Image>(i).sprite = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld, "uiFocusSlotFill");
           }

           // 인덱스 값을 업데이트
           _pointFocusSlotIndex = newIndex; 
       }
       else // 집중력이 감소하는 경우
       {
           // 이전 인덱스에서 diff값만큼 감소한 newIndex 값을 계산하고 범위를 넘지 않도록 제한
           int newIndex = Mathf.Max(_pointFocusSlotIndex - diff, (int)Images.FocusSlot_1);

           // 이전 인덱스부터 newIndex 바로 이전까지의 범위에서 이미지를 업데이트
           for (int i = _pointFocusSlotIndex; i >= newIndex; i--)
           {
               Get<Image>(i).sprite = Managers.Resource.LoadResource<Sprite>(ResourceManager.ResourcePath.SlotIconWorld, "uiFocusSlotBlank");
           }

           // 인덱스 값을 업데이트
           _pointFocusSlotIndex = newIndex;
       }
   }

   /// <summary>
   /// 최대 집중력 업데이트
   /// </summary>
   /// <param name="maxFocus"></param>
   public void UpdateFocusMaxDisplay(int maxFocus)
   {
       int activeIndex;
       
       // 집중력 활성화 
       for (activeIndex = 0; activeIndex < maxFocus; activeIndex++)
       {
           Get<Image>(activeIndex).enabled = true;
       }

       // 집중력 비활성화
       for (int removeIndex = activeIndex; removeIndex < (int)Images.FocusSlot_10; removeIndex++)
       {
           Get<Image>(removeIndex).enabled = false;
       }
   }

    /// <summary>
    /// vote버튼 활성화/비활성화
    /// </summary>
    /// <param name="isOn"></param>
    public void UpdateVoteBtn(Item item)
    {
        Get<UI_RewardButton>(0).gameObject.SetActive(true);
        Get<UI_RewardButton>(0).SetRewardBtn(item);
    }
    public void UpdateTrapBtn()
    {
        Get<UI_RewardButton>(0).gameObject.SetActive(true);
        Get<UI_RewardButton>(0).SetTrapBtn();
    }

    public void UpdateCaveClearBtn()
    {
        Get<UI_RewardButton>(0).gameObject.SetActive(true);
        Get<UI_RewardButton>(0).SetClearBtn();
    }

    public void UpdateCaveOnlyMoveBtn()
    {
        Get<UI_RewardButton>(0).gameObject.SetActive(true);
        Get<UI_RewardButton>(0).SetOnlyMoveBtn();
    }


    public void VoteBtnOff()
    {
        Get<UI_RewardButton>(0).gameObject.SetActive(false);
    }
    /// <summary>
    /// 토큰 UI관리
    /// </summary>
    public void UpdateTokenUI(TokenType token, int Count)
    {
        if (Count != 0)
        {
            Get<UI_playerToken>((int)Token.UI_PlayerTokens).PutToken(token, Count);
        }
        else
        {
            Get<UI_playerToken>((int)Token.UI_PlayerTokens).ReMoveToken(token);
        }
    }

    public void InitTokenUI()
    {
        Get<UI_playerToken>((int)Token.UI_PlayerTokens).ReMoveAll();
    }

    public void UpdateFloatingText(string text)
    {
        Get<UI_FloatingText>((int)Floating.UI_FloatingText).Init(text);
    }
  
}
