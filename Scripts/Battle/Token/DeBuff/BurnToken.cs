using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnToken : Token
{
    public BurnToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Burn;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
