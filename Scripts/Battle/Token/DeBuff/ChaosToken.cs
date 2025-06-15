using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosToken : Token
{
    public ChaosToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Immediately;
        tokenType = TokenType.Chaos;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
