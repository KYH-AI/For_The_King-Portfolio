using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionToken : Token
{
    public ActionToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Action;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
