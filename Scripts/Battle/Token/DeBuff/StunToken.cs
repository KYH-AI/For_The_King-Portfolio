using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunToken : Token
{
    public StunToken(BattleUnit battleUnit,int count) : base(battleUnit)
    {
        activeTime = ActiveTime.MyTurnStart;
        tokenType = TokenType.Stun;
        CanOverlap = false;
        Count = count;
    }
    public override void Active()
    {
        _battleUnit.IsProgress =false;
        Managers.Battle.TurnOver();
        base.Active();
        
    }
    
    public override void Remove()
    {
        _battleUnit.FXList[4].SetActive(false);
    }
    public override void PutToken()
    {
        _battleUnit.FXList[4].SetActive(true);
    }
}
