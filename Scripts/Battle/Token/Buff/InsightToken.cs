using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsightToken : Token
{
    public InsightToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.Insight;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
