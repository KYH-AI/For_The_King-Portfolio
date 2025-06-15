using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeAttackToken : Token
{
    public SawBladeAttackToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.Attack;
        tokenType = TokenType.SawBladeAttack;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
