using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhaustedToken : Token
{
    public ExhaustedToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.Exhausted;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
