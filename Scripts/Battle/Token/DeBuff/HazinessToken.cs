using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazinessToken : Token
{
    public HazinessToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.Haziness;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
