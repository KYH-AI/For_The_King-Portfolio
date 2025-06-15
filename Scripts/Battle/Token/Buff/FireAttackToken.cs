using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttackToken : Token
{
    public FireAttackToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.FireAttack;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
