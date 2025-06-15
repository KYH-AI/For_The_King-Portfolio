using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopUp : UI_Base
{
    public override void Init()
    {
        Managers.UIManager.ShowPopUI(this); 
    }

    public virtual void ClosedPopUpUI()
    {
        Managers.UIManager.ClosePopUpUI(this);
    }
}
