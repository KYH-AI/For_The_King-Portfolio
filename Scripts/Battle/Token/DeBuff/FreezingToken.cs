using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezingToken : Token
{
    public FreezingToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Freezing;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
