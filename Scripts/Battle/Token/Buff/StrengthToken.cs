using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthToken : Token
{
    public StrengthToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.TakeDamage;
        tokenType = TokenType.Strength;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
