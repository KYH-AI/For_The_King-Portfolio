using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceToken : Token
{
    public ChanceToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.Chance;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
