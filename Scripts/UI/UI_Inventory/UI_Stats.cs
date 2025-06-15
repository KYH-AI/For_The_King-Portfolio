using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Stats : UI_Base, I_Inventory
{
    /*  1. 클래스 이름
     * 
     *  2. 클래스 고유 기술
     * 
     *  3. 아이템으로 얻은 고유 기술
     * 
     *  4. 현재 최종 공격력
     *    4-1. 기본 데미지 (무기 기본 데미지 + 플레이어 기본 데미지) (물리, 마법 색상 구별)
     *    4-2. 크리티컬 확률
     *    4-3. 추가 물리 데미지
     *    4-4. 추가 마법 데미지
     * 
     *  5. 기타 능력치
     *   5-1. 현재 레벨
     *   5-2. XP
     *   5-3. XP 추가 획득량
     *   5-4. 현재 재화
     *   5-5. 재화 추가 획득량
     *   5-6. 최종 이동 거리 (월드맵)
     * 
     *  6. 현재 최종 방어력 (모두 색상 구별)
     *   6-1. 기본 물리 방어력 (플레이어 기본 물리 방어력)
     *   6-2. 기본 마법 방어력 (플레이어 기본 마법 방어력)
     *   6-3. 최종 회피력 
     *   6-4. 추가 물리 방어력  (방어구 물리 방어력 + 추가 물리 방어력)
     *   6-5. 추가 마법 방어력  (방어구 마법 방어력 + 추가 마법 방어력)
     *
     *  7. 현재 체력 / 최대 체력
     *   7-1. 기본 최대 체력 (플레이어 기본 최대 체력 (baseHp + 레벨 당 최대체력)
     *   7-2. 추가 최대 체력 (추가 최대 체력)
     *   7-3. 현재 집중력 포인트 / 최대 집중력 포인트
     *   7-4. 기본 최대 집중력 포인트
     *   7-5. 추가 집중력 포인트
     */


    public enum Texts
    {
        ClassNameText,
        
        MaxDamageText,
        BaseAttackDamageText,
        CritChanceText, // 0%
        ModifyPhysicalDamageText,
        ModifyMagicDamageText,
        
        MaxPhysicalArmorText, // 120
        MaxResistanceText,    //   / 120
        BasePhysicalArmorText,
        BaseResistanceText,
        MaxEvadeText,
        ModifyPhysicalArmorText,
        ModifyResistanceText,
        
        CurrentLevelText,
        CurrentXpText, // 0 / 150
        XpMultiText,   // 0%
        CurrentGoldText,
        GoldMultiText,  // 0%
        CurrentMaxMovementText,
        ModifyMovementText,
        
        MaxHpText,   // 0 / 150
        BaseMaxHpText,
        HpRegenText,
        MaxFocusText,  // 0 / 150
        ModifyMaxHpText
    }

    private enum TextRoot
    {
        ClassAbilitiesPanel,
        EquipAbilitiesPanel
    }
    
    public override void Init()
    {
        Bind<Transform>(typeof(TextRoot));
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void InventoryUiUpdate(PlayerStats requestPlayer)
    {
        var textDisplay = requestPlayer.GetInventoryStatUiDisplay();

        for (int i = 0; i < textDisplay.Length; i++)
        {
            Get<TextMeshProUGUI>(i).text = textDisplay[i].Text;
            Get<TextMeshProUGUI>(i).color = textDisplay[i].Color;
        }
        
        // 스텟 UI 활성화
        this.gameObject.SetActive(true);
    }
}
