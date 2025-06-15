using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnforceToken : Token
{
    public EnforceToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.Enforce;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
