using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonToken : Token
{
    public PoisonToken(BattleUnit battleUnit, int count) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Poison;
        CanOverlap = true;
        Count = count;
    }
    public override void Active()
    {
        int damage = Mathf.Max((int)((float)_battleUnit.data.Hp * (0.02f) * Count),1);
        _battleUnit.TakeDamage(_battleUnit, damage, false,true);
        base.Active();
        
    }
    public override void Remove()
    {
        _battleUnit.FXList[3].SetActive(false);
    }
    public override void PutToken()
    {
        _battleUnit.FXList[3].SetActive(true);
    }
}
