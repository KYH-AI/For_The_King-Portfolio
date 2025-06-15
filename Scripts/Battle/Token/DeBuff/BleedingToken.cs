using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingToken : Token
{
    public BleedingToken(BattleUnit battleUnit, int count) : base(battleUnit)
    {
        activeTime = ActiveTime.TurnStart;
        tokenType = TokenType.Bleeding;
        CanOverlap = true;
        Count = count;
    }
    public override void Active()
    {
        int damage = Mathf.Max((int)((float)_battleUnit.CurHp * (0.05f) *Count),1);
        _battleUnit.TakeDamage(_battleUnit, damage, false,true);    
        if (Count != 3)
        {
            base.Active();
        }
    }
    public override void Remove()
    {
        _battleUnit.FXList[5].SetActive(false);
    }
    public override void PutToken()
    {
        _battleUnit.FXList[5].SetActive(true);
    }

}
