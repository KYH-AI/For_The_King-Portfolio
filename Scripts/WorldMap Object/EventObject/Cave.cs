using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cave : WorldMapEventInfo
{
    public Cave(int eventID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null) 
        : base(eventID, eventName, eventDetailText, eventDescriptionText, eventLevel, eventPortrait)
    {
    }
}
