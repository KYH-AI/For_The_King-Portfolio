
using UnityEngine;

public class ExperienceSanctuary : Sanctuary
{
    public ExperienceSanctuary(int eventID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null) : base(eventID, eventName, eventDetailText, eventDescriptionText, eventLevel, eventPortrait)
    {
    }

    public void EnableSanctuaryBuff(PlayerStats request)
    {
        // °æÇèÄ¡ 25% È¹µæ Áõ°¡ ¹öÇÁ
    }

    public void DisableSanctuaryBuff(PlayerStats request)
    {
        // °æÇèÄ¡ 25% È¹µæ °¨¼Ò ¹öÇÁ
    }

    public override void EnableSanctuaryBuff2(PlayerStats request)
    {
        throw new System.NotImplementedException();
    }

    public override void DisableSanctuaryBuff2(PlayerStats request)
    {
        throw new System.NotImplementedException();
    }
}
