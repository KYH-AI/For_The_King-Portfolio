using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnrestToken : Token
{
    public UnrestToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Unrest;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
