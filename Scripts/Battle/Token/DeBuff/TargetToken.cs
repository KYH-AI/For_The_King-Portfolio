using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetToken : Token
{
    public TargetToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TakeDamage;
        tokenType = TokenType.Target;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
