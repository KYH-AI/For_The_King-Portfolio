using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusToken : Token
{
    public FocusToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Focus;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
