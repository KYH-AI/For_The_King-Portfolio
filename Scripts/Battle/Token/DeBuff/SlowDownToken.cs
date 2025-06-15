using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownToken : Token
{
    public SlowDownToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Immediately;
        tokenType = TokenType.SlowDown;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
