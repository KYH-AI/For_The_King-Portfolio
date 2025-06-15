using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAttackToken : Token
{
    public HunterAttackToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.HunterAttack;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
