using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestraintToken : Token
{
    public RestraintToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.GetOtherToken;
        tokenType = TokenType.Restraint;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
