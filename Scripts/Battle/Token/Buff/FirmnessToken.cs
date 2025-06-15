using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirmnessToken : Token
{
    public FirmnessToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.GetOtherToken;
        tokenType = TokenType.Firmness;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
