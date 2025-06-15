using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI_CaveButtonGrid : UI_EventPopUpButtonGrid
{
    private enum Buttons
    {
        EnterButton,
        LeaveButton
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Get<Button>((int)Buttons.EnterButton).gameObject.BindEvent(data => { EntetButtonEvent(); }, MouseUIEvent.Click);
        BindEventLeaveButton((int)Buttons.LeaveButton);
    }

    protected override void LeaveButtonEvent()
    {
        ParentUIPopUp.UI_MainEventPopUp.ClosedPopUpUI();
    }

    void EntetButtonEvent()
    {
        List<PlayerStats> playerObjectList = new List<PlayerStats>();
        for(int i = 0; i < Managers.Instance.GetPlayerCount; i++)
        {
            playerObjectList.Add(Managers.Instance.GetPlayer(i).PlayerStats);
        }
        List<List<int>> temp = new List<List<int>>();

        Managers.Cave.CaveInit(CaveManager.LengthType.SHORT, playerObjectList, temp, "CaveMap");
        ParentUIPopUp.UI_MainEventPopUp.ClosedPopUpUI();
    }
}
