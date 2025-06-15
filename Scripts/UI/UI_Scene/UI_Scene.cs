using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    public override void Init()
    {
        gameObject.SetActive(true);
    }

    public void ClosedUI()
    {
        gameObject.SetActive(false);
    }
}
