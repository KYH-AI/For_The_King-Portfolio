using UnityEngine;

public class WorldMapEventInfo : IEventInfo
{
    private readonly int _eventID;
    
    private string _eventName;
    
    private readonly string _eventDetailText;
    
    private readonly string _eventDescriptionText;
    
    private readonly int _eventLevel;                  // -1은 없는 의미임
    
    private readonly Sprite _eventPortrait;            // null이 존재함
    
    public WorldMapEventInfo(int eventID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null)
    {
        _eventID = eventID;
        _eventName = eventName;
        _eventDetailText = eventDetailText;
        _eventDescriptionText = eventDescriptionText;
        _eventLevel = eventLevel;
        _eventPortrait = eventPortrait;
    }

    public int GetEventID()
    {
       return this._eventID;
    }

    public int GetEventLevel()
    {
        return this._eventLevel;
    }

    public string GetEventName()
    {
        return this._eventName;
    }

    public void SetEventName(string editEventName)
    {
        this._eventName += " "+editEventName;
    }

    public string GetEventDetailText()
    {
        return this._eventDetailText;
    }

    public string GetEventDescriptionText()
    {
        return this._eventDescriptionText;
    }

    public Sprite GetEventPortrait()
    {
        return this._eventPortrait;
    }
}
