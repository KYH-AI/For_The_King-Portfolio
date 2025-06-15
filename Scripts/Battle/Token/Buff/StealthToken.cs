using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthToken : Token
{
    public StealthToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.EnemyTurn;
        tokenType = TokenType.Stealth;
        CanOverlap = false;
    }
    public override void Active()
    {

    }
}
