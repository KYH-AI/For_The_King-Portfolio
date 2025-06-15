using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidToken : Token
{
    public AvoidToken(BattleUnit battleUnit, int count) : base(battleUnit)
    {
        activeTime = ActiveTime.TakeDamage;
        tokenType = TokenType.Avoid;
        CanOverlap = false;
        Count = count;
    }
    public override void Active()
    {
        _battleUnit.data.Avdalpa = 1.5f;
    }
    public override void Remove()
    {
        _battleUnit.data.Avdalpa = 1f;
    }
}
