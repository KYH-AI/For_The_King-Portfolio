
using UnityEngine;

public class ExperienceSanctuary : Sanctuary
{
    public ExperienceSanctuary(int eventID, string eventName, string eventDetailText, string eventDescriptionText, int eventLevel = -1, Sprite eventPortrait = null) : base(eventID, eventName, eventDetailText, eventDescriptionText, eventLevel, eventPortrait)
    {
    }

    public void EnableSanctuaryBuff(PlayerStats request)
    {
        // ����ġ 25% ȹ�� ���� ����
    }

    public void DisableSanctuaryBuff(PlayerStats request)
    {
        // ����ġ 25% ȹ�� ���� ����
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
