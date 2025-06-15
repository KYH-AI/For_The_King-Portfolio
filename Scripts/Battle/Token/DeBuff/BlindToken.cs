using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindToken : Token
{
    public BlindToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.Blind;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
