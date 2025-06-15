using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyToken : Token
{
    public ReadyToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Ready;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
