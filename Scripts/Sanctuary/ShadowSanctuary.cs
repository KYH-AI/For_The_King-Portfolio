using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSanctuary : Sanctuary
{

    private int buffAvoidValue = 2;

    
    public ShadowSanctuary(int eventID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null) : base(eventID, eventName, eventDetailText, eventDescriptionText, eventLevel, eventPortrait)
    {
    }

    public void EnableSanctuaryBuff(PlayerStats request)
    {
        // 회피력 증가
        request.UpdateSanctuaryBuff(SanctuaryType.ShadowSanctuary, PlayerStats.ModifyStatType.ModifyAvoid, buffAvoidValue, true);
    }

    public void DisableSanctuaryBuff(PlayerStats request)
    {
        // 회피력 감소
        request.UpdateSanctuaryBuff(SanctuaryType.None, PlayerStats.ModifyStatType.ModifyAvoid, buffAvoidValue, false);
    }

    public override void EnableSanctuaryBuff2(PlayerStats request)
    {
        // 회피력 증가
        request.UpdateSanctuaryBuff(SanctuaryType.ShadowSanctuary, PlayerStats.ModifyStatType.ModifyAvoid, buffAvoidValue, true);
    }

    public override void DisableSanctuaryBuff2(PlayerStats request)
    {
        // 회피력 감소
        request.UpdateSanctuaryBuff(SanctuaryType.None, PlayerStats.ModifyStatType.ModifyAvoid, buffAvoidValue, false);
    }
}
