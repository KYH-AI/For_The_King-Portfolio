using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductionToken : Token
{
    public ReproductionToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Reproduction;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
