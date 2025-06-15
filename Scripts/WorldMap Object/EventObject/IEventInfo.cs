using UnityEngine;

public interface IEventInfo
{
    public int GetEventID();

    public int GetEventLevel();

    public string GetEventName();

    public void SetEventName(string editEventName);

    public string GetEventDetailText();

    public string GetEventDescriptionText();

    public Sprite GetEventPortrait();
}
