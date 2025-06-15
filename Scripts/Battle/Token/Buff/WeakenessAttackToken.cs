using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakenessAttackToken : Token
{
    public WeakenessAttackToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.WeakenessAttack;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
