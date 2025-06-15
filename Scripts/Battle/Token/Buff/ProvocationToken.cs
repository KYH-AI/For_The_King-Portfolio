using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvocationToken : Token
{
    public ProvocationToken(BattleUnit battleUnit) : base(battleUnit)
    {
        activeTime = ActiveTime.EnemyTurn;
        tokenType = TokenType.Provocation;
        CanOverlap = true;
    }
    public override void Active()
    {

    }
}
