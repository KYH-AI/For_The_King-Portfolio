using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sanctuary : WorldMapEventInfo
{
    public enum SanctuaryType
    {
        None = -1,
        ExperienceSanctuary = 0,
        ShadowSanctuary = 1,
        FocusSanctuary = 2,
        HealthSanctuary = 3,
    }
    
    public Sanctuary(int eventID, string eventName, string eventDetailText, string eventDescriptionText,
        int eventLevel = -1, Sprite eventPortrait = null) : base(eventID, eventName, eventDetailText,
        eventDescriptionText, eventLevel, eventPortrait)
    {
    }

    public abstract void EnableSanctuaryBuff2(PlayerStats request);
    
    public abstract void DisableSanctuaryBuff2(PlayerStats request);
}
