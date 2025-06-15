using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAttackToken : Token
{
    public ChainAttackToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.ChainAttack;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
