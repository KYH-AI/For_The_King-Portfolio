using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSanctuary : Sanctuary
{
    private int buffMaxHealth = 5;
    
    public HealthSanctuary(int eventID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null) : base(eventID, eventName, eventDetailText, eventDescriptionText, eventLevel, eventPortrait)
    {
    }

    public void EnableSanctuaryBuff(PlayerStats request)
    {
        // �ִ� ü�� ����
        request.UpdateSanctuaryBuff(SanctuaryType.HealthSanctuary, PlayerStats.ModifyStatType.ModifyMaxHealth, buffMaxHealth, true);
    }

    public void DisableSanctuaryBuff(PlayerStats request)
    {
        // �ִ� ü�� ����
        request.UpdateSanctuaryBuff(SanctuaryType.None, PlayerStats.ModifyStatType.ModifyMaxHealth, buffMaxHealth, false);
    }

    public override void EnableSanctuaryBuff2(PlayerStats request)
    {
        // �ִ� ü�� ����
        request.UpdateSanctuaryBuff(SanctuaryType.HealthSanctuary, PlayerStats.ModifyStatType.ModifyMaxHealth, buffMaxHealth, true);
    }

    public override void DisableSanctuaryBuff2(PlayerStats request)
    {
        // �ִ� ü�� ����
        request.UpdateSanctuaryBuff(SanctuaryType.None, PlayerStats.ModifyStatType.ModifyMaxHealth, buffMaxHealth, false);
    }
}
