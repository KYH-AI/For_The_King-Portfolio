using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindAttackToken : Token
{
    public BlindAttackToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.BlindAttack;
        CanOverlap = false;
    }
    public override void Active()
    {
        _battleUnit.data.Avdalpa = 1f;
    }
    public override void Remove()
    {
        _battleUnit.data.Avdalpa = 1f;
    }
}
