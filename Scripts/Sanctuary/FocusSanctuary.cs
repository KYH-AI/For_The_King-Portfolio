using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusSanctuary : Sanctuary
{
    private int buffMaxFocusValue = 2;
    
    public FocusSanctuary(int eventID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null) : base(eventID, eventName, eventDetailText, eventDescriptionText, eventLevel, eventPortrait)
    {
    }

    public void EnableSanctuaryBuff(PlayerStats request)
    {
        // 최대 집중력 증가
        request.UpdateSanctuaryBuff(SanctuaryType.FocusSanctuary, PlayerStats.ModifyStatType.ModifyMaxFocus, buffMaxFocusValue, true);
    }

    public void DisableSanctuaryBuff(PlayerStats request)
    {
        // 최대 집중력 감소
        request.UpdateSanctuaryBuff(SanctuaryType.None, PlayerStats.ModifyStatType.ModifyMaxFocus, buffMaxFocusValue, false);
    }

    public override void EnableSanctuaryBuff2(PlayerStats request)
    {
        // 최대 집중력 증가
        request.UpdateSanctuaryBuff(SanctuaryType.FocusSanctuary, PlayerStats.ModifyStatType.ModifyMaxFocus, buffMaxFocusValue, true);
    }

    public override void DisableSanctuaryBuff2(PlayerStats request)
    {
        // 최대 집중력 감소
        request.UpdateSanctuaryBuff(SanctuaryType.None, PlayerStats.ModifyStatType.ModifyMaxFocus, buffMaxFocusValue, false);
    }
}
