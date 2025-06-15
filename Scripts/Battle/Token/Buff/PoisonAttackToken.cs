using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAttackToken : Token
{
    public PoisonAttackToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.PoisonAttack;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
