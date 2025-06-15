using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI_SanctuaryButtonGrid : UI_EventPopUpButtonGrid
{
    
    private enum Buttons
    {
        PrayButton,
        LeaveButton
    }
    
    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Get<Button>((int)Buttons.PrayButton).gameObject.BindEvent(data => { PrayButtonEvent(); });
        BindEventLeaveButton((int)Buttons.LeaveButton);
    }

    private void PrayButtonEvent()
    {
        PlayerStats requestPlayer = ParentUIPopUp.UI_MainEventPopUp.GetInteractionPlayerList()[0];
        int sanctuaryId = ParentUIPopUp.UI_MainEventPopUp.GetInteractionEventList()[0];
        Managers.Data.GetSanctuaryInfo(sanctuaryId).EnableSanctuaryBuff2(requestPlayer);
        
        ParentUIPopUp.UI_MainEventPopUp.CompleteHexEvent();
        
        LeaveButtonEvent();
    }

    protected override void LeaveButtonEvent()
    {
        ParentUIPopUp.UI_MainEventPopUp.ClosedPopUpUI();
    }

}
