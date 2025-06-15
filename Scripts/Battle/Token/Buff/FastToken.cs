using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastToken : Token
{
    public FastToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Immediately;
        tokenType = TokenType.Fast;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
